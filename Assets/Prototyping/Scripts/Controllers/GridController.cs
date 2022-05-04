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
        // TODO : Move UI code to UI class
        [SerializeField] private Text tilesCountTxt;
        [SerializeField] public Button startWaveBtn;
        [SerializeField] private Tilemap pathTileMap;
        [SerializeField] private Tilemap obstaclesTileMap;
        // [SerializeField] private Tile tile;
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
        [SerializeField] private LayerMask[] ignoreLayers;
        private LayerMask ignoreMask = new LayerMask();
        private AudioSource audioSource;
        // private LineFactory lineFactory;
        private bool waveStarted = false;
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

            if (Input.GetMouseButtonUp(0))
                SetTileAtMousePosition();

            if (Input.GetMouseButtonUp(1))
                RemoveTileAtMousePosition();
        }

        private bool CanPlaceTile(Vector3Int tileLocation)
        {
            if (pathTileMap.GetTile(tileLocation) == null && 
                obstaclesTileMap.GetTile(tileLocation) == null &&
                IsLastTileNeighbor(tileLocation))
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
            
            // lineFactory.GetLine(pathTileMap.GetCellCenterWorld(tileLocation), pathTileMap.GetCellCenterWorld(tilesPositions.Last()), 0.02f, new Color(255,0,0,1));
            Tile tile = GetTile(tileLocation);
            FixPathBend(tileLocation);
            pathTileMap.SetTile(tileLocation, tile);
            tilesPositions.Add(tileLocation);
            
            if (IsPathCompleted()) FixPathBend(endPosition);

            UpdateAvailableTilesText();

            audioSource.Play();
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
                // lineFactory.RemoveLine();
            }
            
            UpdateAvailableTilesText();
        }
        
        private void UpdateAvailableTilesText()
        {
            var updatedTilesCountTxt = $"Available tiles: {tilesLimit - PathSize}";
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
            // foreach (var position in tilesPositions)
            // {
            //     lineFactory.RemoveLine();
            // }
        }
    }
}
