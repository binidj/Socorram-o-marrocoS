using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Prototyping.Scripts.ScriptableObjects;
using UnityEngine.EventSystems;
using Prototyping.Scripts.Entities;

namespace Prototyping.Scripts.Controllers
{
    public class TrapsController : MonoBehaviour
    {
        private GameObject selectedTrap = null;
        private GameObject currentTrap = null;
        [SerializeField] private Tilemap trapsTilemap;
        [SerializeField] private Tilemap pathTilemap;
        private Vector3 mousePosition = new Vector3();
        private Vector3Int lastPosition = new Vector3Int(100, 100, 0);
        private Vector3Int tilePosition = new Vector3Int();
        private List<Transform> tileCells = new List<Transform>();
        [SerializeField] LevelConfig levelConfig;
        private Dictionary<GameObject, int> trapCount = new Dictionary<GameObject, int>();
        private Dictionary<GameObject, List<GameObject>> trapInstances = new Dictionary<GameObject, List<GameObject>>();
        private Vector3 trapsPosition = new Vector3(100f, 100f, 0f);
        public delegate void UpdateButtonCount(GameObject trap);
        public static event UpdateButtonCount updateButtonCount;
        private AudioSource audioSource;
        [SerializeField] EnemyController enemyController;

        private void Awake()
        {
            foreach (TrapData trapData in levelConfig.trapsAvailable)
            {
                trapCount[trapData.trap] = trapData.limit;
                trapInstances[trapData.trap] = new List<GameObject>();
                
                for (int i = 0; i < trapData.limit; i++)
                {
                    GameObject newInstance = Instantiate(trapData.trap, trapsPosition, Quaternion.identity, gameObject.transform);
                    newInstance.SetActive(false);
                    trapInstances[trapData.trap].Add(newInstance);
                }
            }

            audioSource = GetComponent<AudioSource>();
        }
        
        public bool isPlacingTrap { 
            get { return selectedTrap != null; } 
        }

        private bool CanPlaceTrap()
        {
            if (pathTilemap.GetTile(tilePosition) == null) return false;
            if (currentTrap.GetComponentInChildren<TrapCollision>().isColliding) return false;
            foreach (Transform transform in tileCells)
            {
                if (pathTilemap.GetTile(pathTilemap.WorldToCell(transform.position)) == null)
                {
                    return false;
                }
            }
            return true;
        }
        
        private void PlaceTrap()
        {
            if (!CanPlaceTrap()) return;
            
            CursorController.Instance.SetActiveCursorType(CursorType.CanTriggerTrap);
            updateButtonCount?.Invoke(selectedTrap);
            // SpriteRenderer spriteRenderer = currentTrap.GetComponent<SpriteRenderer>();
            SpriteRenderer[] spriteRenderers = currentTrap.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
            // spriteRenderer.color = color;
            ITrap genericTrap = currentTrap.GetComponentInChildren<ITrap>();
            if (genericTrap != null) genericTrap.isPlacing = false;
            if (genericTrap is TriggeredTrap)
                StartCoroutine((genericTrap as TriggeredTrap).EnableTrap());
            currentTrap = null;
            selectedTrap = null;
            tileCells.Clear();
            audioSource.Play();
            enemyController.SetActivePoints(false);
        }

        private void RotateTrap()
        {
            if (currentTrap.transform.rotation == Quaternion.identity)
                currentTrap.transform.Rotate(0f, 0f, 90f);
            else
                currentTrap.transform.rotation = Quaternion.identity;
        }

        private bool HitUIComponent()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private void Update()
        {
            if (!isPlacingTrap) return;

            bool canPlaceTrap = CanPlaceTrap();

            if (CursorController.Instance.cursorType != CursorType.CanPlaceTrap && canPlaceTrap)
                CursorController.Instance.SetActiveCursorType(CursorType.CanPlaceTrap);

            if (CursorController.Instance.cursorType != CursorType.GrabbingTrap && !canPlaceTrap)
                CursorController.Instance.SetActiveCursorType(CursorType.GrabbingTrap);

            mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            tilePosition = trapsTilemap.WorldToCell(mousePosition);

            if (tilePosition != lastPosition)
                lastPosition = tilePosition;

            if (currentTrap != null)
                currentTrap.transform.position = trapsTilemap.GetCellCenterWorld(lastPosition); // lastPosition + tileOffset;

            if (HitUIComponent()) return;

            if (Input.GetMouseButtonUp(0))
                PlaceTrap();

            if (Input.GetMouseButtonUp(1))
                RotateTrap();
        }

        public void SelectTrap(GameObject trap)
        {
            if (trap == null) return;

            if (currentTrap != null)
            {
                ITrap genericTrap = currentTrap.GetComponentInChildren<ITrap>();
                if (genericTrap != null) genericTrap.isPlacing = false;
                currentTrap.transform.position = trapsPosition;
                currentTrap.SetActive(false);
                tileCells.Clear();
            }
            
            currentTrap = null;
            foreach (GameObject trapInstance in trapInstances[trap])
            {
                if (!trapInstance.activeSelf && trapInstance.tag == "Pool")
                {
                    currentTrap = trapInstance;
                    // SpriteRenderer spriteRenderer = currentTrap.GetComponent<SpriteRenderer>();
                    SpriteRenderer[] spriteRenderers = currentTrap.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        Color color = spriteRenderer.color;
                        color.a = 0.4f;
                        spriteRenderer.color = color;
                    }
                    lastPosition = new Vector3Int(100, 100, 0);
                    currentTrap.SetActive(true);
                    break;
                }
            }

            if (currentTrap != null)
            {
                // for(int i = 0; i < gameobject.transform.GetChildCount(); i++)
                //     Transform Children = gameobject.transform.GetChild(i);
                // }
                CursorController.Instance.SetActiveCursorType(CursorType.GrabbingTrap);
                Transform root = currentTrap.transform.GetChild(1);
                for (int i = 0; i < root.childCount; i++)
                {
                    tileCells.Add(root.GetChild(i).transform);
                }
                selectedTrap = trap;
                ITrap genericTrap = currentTrap.GetComponentInChildren<ITrap>();
                if (genericTrap != null) genericTrap.isPlacing = true;
                enemyController.SetActivePoints(true);
                // TriggeredTrap triggeredTrap = currentTrap.GetComponentInChildren<TriggeredTrap>();
                // if (triggeredTrap != null)
                //     triggeredTrap.isPlacing = true;
            }
                
        }

        public void ClearSelection()
        {
            if (!isPlacingTrap) return;
            CursorController.Instance.SetActiveCursorType(CursorType.Default);
            currentTrap.transform.position = trapsPosition;
            currentTrap.SetActive(false);
            ITrap genericTrap = currentTrap.GetComponentInChildren<ITrap>();
            if (genericTrap != null) genericTrap.isPlacing = false;
            currentTrap = null;
            selectedTrap = null;
            tileCells.Clear();
            enemyController.SetActivePoints(false);
        }
    }
}
