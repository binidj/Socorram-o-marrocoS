using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Prototyping.Scripts.ScriptableObjects;

namespace Prototyping.Scripts.Controllers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Text tilesCountTxt;
        [SerializeField] public Button startWaveBtn;
        [SerializeField] private Tilemap pathTileMap;
        [SerializeField] private Tilemap obstaclesTileMap;
        [SerializeField] private Tilemap towersTileMap;
        [SerializeField] private List<Vector3Int> tilesPositions;
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private TrapsController trapsController;
        [SerializeField] private List<Vector3Int> placeableDirections;
        [SerializeField] private List<Tile> placeableTiles;
        [SerializeField] private List<Vector3Int> fixDirections;
        [SerializeField] private List<Tile> fixTiles;
        private Dictionary<Vector3Int, Tile> dirToTile;
        private Dictionary<Vector3Int, Tile> dirFixTile;
        private int tilesLimit;
        private Vector3Int startPosition;
        private Vector3Int endPosition;
        private Vector3Int lowerLimit;
        private Vector3Int upperLimit;
        [SerializeField] private LayerMask[] ignoreLayers;
        private LayerMask ignoreMask = new LayerMask();
        private AudioSource audioSource;
        private List<GameObject> possibleMoves = new List<GameObject>();
        [SerializeField] private GameObject squareZone;
        [SerializeField] Tile towerMarker;
        private bool waveStarted = false;
        private float placeCooldown = 0.1f;
        private float cooldownCounter = 0f;
        private int PathSize
        {
            get { 
                Debug.Assert(tilesPositions.Count != 0);
                return tilesPositions.Count - 1; 
            }
        }
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            startPosition = levelConfig.startPosition;
            endPosition = levelConfig.endPosition;
            lowerLimit = levelConfig.lowerLimit;
            upperLimit = levelConfig.upperLimit;
            tilesLimit = levelConfig.tilesLimit;
            tilesPositions.Add(startPosition);
            // lineFactory = GetComponent<LineFactory>();
            UpdateAvailableTilesText();

            foreach (LayerMask mask in ignoreLayers)
            {
                ignoreMask |= mask.value;
            }

            dirToTile = new Dictionary<Vector3Int, Tile>();
            for (int i = 0; i < placeableTiles.Count; i++)
            {
                dirToTile[placeableDirections[i]] = placeableTiles[i];
            }

            dirFixTile = new Dictionary<Vector3Int, Tile>();
            for (int i = 0; i < fixTiles.Count; i++)
            {
                dirFixTile[fixDirections[i]] = fixTiles[i];
            }

            for (int i = 0; i < placeableTiles.Count; i++)
            {
                possibleMoves.Add(Instantiate(squareZone, new Vector3(-1000, -1000, 0f), Quaternion.identity));
            }
            FixPossibleMoves();

            foreach (Transform child in towersTileMap.gameObject.transform)
            {
                Vector3Int position = towersTileMap.WorldToCell(child.transform.position);
                towersTileMap.SetTile(position, towerMarker);
            }
        }

        private void FixPossibleMoves()
        {
            foreach (var square in possibleMoves)
            {
                square.SetActive(false);
            }

            int squareIndex = 0;
            foreach (var direction in placeableDirections)
            {
                Vector3Int position = tilesPositions.Last() + direction;
                Vector3 center = pathTileMap.GetCellCenterWorld(position);
                if (CanPlaceTile(position) && !CanCollideWithTower(center))
                {
                    possibleMoves[squareIndex].transform.position = center;
                    possibleMoves[squareIndex].SetActive(true);
                    squareIndex += 1;
                }
            }
        }

        private Tile GetTile(Vector3Int tileLocation)
        {
            Vector3Int dir = tileLocation - tilesPositions.Last();
            return dirToTile[dir];
        }

        private void FixPathBend(Vector3Int tileLocation)
        {
            if (tilesPositions.Count < 2) return;
            Vector3Int dir1 = tilesPositions[tilesPositions.Count - 2] - tilesPositions[tilesPositions.Count - 1];
            Vector3Int dir2 = tileLocation - tilesPositions[tilesPositions.Count - 1];
            Vector3Int result = dir1 + dir2;
            if (dirFixTile.ContainsKey(result))
            {
                pathTileMap.SetTile(tilesPositions[tilesPositions.Count - 1], dirFixTile[result]);
            }
            else if (dirToTile.ContainsKey(dir2))
            {
                pathTileMap.SetTile(tilesPositions[tilesPositions.Count - 1], dirToTile[dir2]);
            }
        }

        private void OnEnable() 
        {
            StartWave.startWaveEvent += BeginWave;
        }

        private void OnDisable() 
        {
            StartWave.startWaveEvent -= BeginWave;
        }

        private void Update()
        {
            if (waveStarted) return;

            if (cooldownCounter > 0f)
                cooldownCounter -= Time.deltaTime;

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
                SetTileAtMousePosition();

            if (Input.GetMouseButtonUp(1))
                RemoveTileAtMousePosition();
        }

        private bool CanPlaceTile(Vector3Int tileLocation)
        {
            if (pathTileMap.GetTile(tileLocation) == null && 
                obstaclesTileMap.GetTile(tileLocation) == null &&
                towersTileMap.GetTile(tileLocation) == null &&
                IsLastTileNeighbor(tileLocation) && 
                tileLocation.x <= upperLimit.x &&
                tileLocation.y <= upperLimit.y &&
                tileLocation.x >= lowerLimit.x &&
                tileLocation.y >= lowerLimit.y)
            {
                return true;
            }
            return false;
        }
        private void SetTileAtMousePosition()
        {
            if (trapsController.isPlacingTrap || PathSize >= tilesLimit || IsPathCompleted()) return;
            
            Vector3 mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tileLocation = pathTileMap.WorldToCell(mousePosition);

            if (!CanPlaceTile(tileLocation)) return;
            if (CanCollideWithTower(mousePosition)) return;
            if (cooldownCounter > 0f) return;
            
            // lineFactory.GetLine(pathTileMap.GetCellCenterWorld(tileLocation), pathTileMap.GetCellCenterWorld(tilesPositions.Last()), 0.02f, new Color(255,0,0,1));
            Tile tile = GetTile(tileLocation);
            FixPathBend(tileLocation);
            pathTileMap.SetTile(tileLocation, tile);
            tilesPositions.Add(tileLocation);
            
            FixPossibleMoves();

            if (IsPathCompleted()) 
            {
                ClearPossibleMoves();
                FixPathBend(endPosition);  
            }

            UpdateAvailableTilesText();

            audioSource.Play();
            cooldownCounter = placeCooldown;
        }

        private bool CanCollideWithTower(Vector3 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 15f, ~ignoreMask);
            if (hit.collider != null)
                return true;
            return false;
        }

        private void RemoveTileAtMousePosition()
        {
            var mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            var tileLocation = pathTileMap.WorldToCell(mousePosition);

            if (!pathTileMap.GetTile(tileLocation) || tileLocation == startPosition || trapsController.isPlacingTrap) return;
            
            var removedIndex = tilesPositions.FindIndex(position => position.Equals(tileLocation));

            if (removedIndex < 0) return;

            var lastIndex = PathSize;
            var quantityToRemove = lastIndex - removedIndex;
            
            for (var i = 0; i <= quantityToRemove; i++)
            {
                pathTileMap.SetTile(tilesPositions[lastIndex-i], null);
                RaycastHit2D hit = Physics2D.Raycast(pathTileMap.GetCellCenterWorld(tilesPositions[lastIndex-i]), Vector2.zero, 15f, LayerMask.GetMask("Traps"));
                if (hit.collider != null)
                {
                    GameObject gameObject = hit.collider.gameObject.transform.parent.gameObject;
                    gameObject.transform.position = new Vector3(100f, 100f, 0);
                    gameObject.SetActive(false);
                }
                tilesPositions.RemoveAt(lastIndex-i);
            }
            
            audioSource.Play();
            UpdateAvailableTilesText();
            FixPossibleMoves();
        }
        
        private void UpdateAvailableTilesText()
        {
            var updatedTilesCountTxt = $"Tiles Available: {tilesLimit - PathSize}";
            tilesCountTxt.text = updatedTilesCountTxt;
            
            startWaveBtn.interactable = IsPathCompleted();
        }

        private bool IsLastTileNeighbor(Vector3Int tileLocation)
        {
            var lastTilePosition = tilesPositions.Last();

            return IsNeighbor(lastTilePosition, tileLocation);
        }

        private bool IsPathCompleted()
        {
            if (PathSize == 0) return false;

            var lastTilePosition = tilesPositions.Last();
            
            return IsNeighbor(endPosition, lastTilePosition);
        }

        private static bool IsNeighbor(Vector3Int referencePosition, Vector3Int evaluatedPosition)
        {
            var isHorizontalNeighbor = 
                (evaluatedPosition.x == referencePosition.x - 1 || evaluatedPosition.x == referencePosition.x + 1) && evaluatedPosition.y == referencePosition.y;
            var isVerticalNeighbor = 
                (evaluatedPosition.y == referencePosition.y - 1 || evaluatedPosition.y == referencePosition.y + 1) && evaluatedPosition.x == referencePosition.x;

            return isHorizontalNeighbor || isVerticalNeighbor;
        }

        public List<Vector3> GetPathPositions()
        {
            List<Vector3> pathPositions = new List<Vector3>();
            foreach (Vector3Int position in tilesPositions)
            {
                pathPositions.Add(pathTileMap.GetCellCenterWorld(position));
            }
            pathPositions.Add(pathTileMap.GetCellCenterWorld(endPosition));
            return pathPositions;
        }

        public void BeginWave()
        {
            waveStarted = true;
            ClearPossibleMoves();
        }

        private void ClearPossibleMoves()
        {
            foreach (var square in possibleMoves)
            {
                square.SetActive(false);
            }
        }
    }
}
