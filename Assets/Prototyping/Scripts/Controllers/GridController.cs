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
        [SerializeField] private Tile tile;
        [SerializeField] private List<Vector3Int> tilesPositions;
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private TrapsController trapsController;
        private int tilesLimit;
        private Vector3Int startPosition;
        private Vector3Int endPosition;
        private Vector3 worldOffset = new Vector3(0.5f, 0.5f, 0);
        [SerializeField] private LayerMask[] ignoreLayers;
        private LayerMask ignoreMask = new LayerMask();
        private int PathSize
        {
            get { 
                Debug.Assert(tilesPositions.Count != 0);
                return tilesPositions.Count - 1; 
            }
        }
        private void Start()
        {
            startPosition = levelConfig.startPosition;
            endPosition = levelConfig.endPosition;
            tilesLimit = levelConfig.tilesLimit;
            tilesPositions.Add(startPosition);
            UpdateAvailableTilesText();

            foreach (LayerMask mask in ignoreLayers)
            {
                ignoreMask |= mask.value;
            }
        }

        private void Update()
        {
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
            
            pathTileMap.SetTile(tileLocation, tile);
            tilesPositions.Add(tileLocation);
            UpdateAvailableTilesText();
            
        }

        private bool CanCollideWithTower(Vector3 mousePosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 5f, ~ignoreMask);
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
                tilesPositions.RemoveAt(lastIndex-i);
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
                pathPositions.Add(pathTileMap.CellToWorld(position) + worldOffset);
            }
            pathPositions.Add(pathTileMap.CellToWorld(endPosition) + worldOffset);
            return pathPositions;
        }
    }
}
