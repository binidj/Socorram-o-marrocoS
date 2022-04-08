using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Prototyping.Scripts
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Text tilesCountTxt;
        [SerializeField] public Button startWaveBtn;

        [SerializeField] private Tilemap pathTileMap;
        [SerializeField] private Tile tile;

        [SerializeField] private int tilesLimit;
        [SerializeField] private List<Tile> path;
        [SerializeField] private List<Vector3Int> tilesPositions;
        
        private void Start()
        {
            path = new List<Tile>();
            UpdateAvailableTilesText();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
                SetTileAtMousePosition();

            if (Input.GetMouseButtonUp(1))
                RemoveTileAtMousePosition();
        }

        private void SetTileAtMousePosition()
        {
            if (path.Count >= tilesLimit || IsPathCompleted()) return;
            
            var mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            var tileLocation = pathTileMap.WorldToCell(mousePosition);

            if (pathTileMap.GetTile(tileLocation) || !IsLastTileNeighbor(tileLocation)) return;
            
            pathTileMap.SetTile(tileLocation, tile);
            path.Add(pathTileMap.GetTile<Tile>(tileLocation));
            tilesPositions.Add(tileLocation);
            UpdateAvailableTilesText();
            
        }

        private void RemoveTileAtMousePosition()
        {
            var mousePosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            var tileLocation = pathTileMap.WorldToCell(mousePosition);

            if (!pathTileMap.GetTile(tileLocation)) return;
            
            var removedIndex = tilesPositions.FindIndex(position => position.Equals(tileLocation));
            var lastIndex = tilesPositions.Count - 1;
            var quantityToRemove = lastIndex - removedIndex;
            for (var i = 0; i <= quantityToRemove; i++)
            {
                pathTileMap.SetTile(tilesPositions[lastIndex-i], null);
                tilesPositions.RemoveAt(lastIndex-i);
                path.RemoveAt(lastIndex-i);
            }
            UpdateAvailableTilesText();
        }
        
        private void UpdateAvailableTilesText()
        {
            var updatedTilesCountTxt = $"Available tiles: {tilesLimit - path.Count}";
            tilesCountTxt.text = updatedTilesCountTxt;
            startWaveBtn.interactable = IsPathCompleted();
        }

        private bool IsLastTileNeighbor(Vector3Int tileLocation)
        {
            if (path.Count == 0) return true;

            var lastTilePosition = tilesPositions.Last();

            return IsNeighbor(lastTilePosition, tileLocation);
        }

        private bool IsPathCompleted()
        {
            if (tilesPositions.Count == 0) return false;
            
            var startPosition = new Vector3Int(-9, -1, 0);
            var endPosition = new Vector3Int(8, -1, 0);

            var firstTilePosition = tilesPositions.First();
            var lastTilePosition = tilesPositions.Last();
            
            return IsNeighbor(startPosition, firstTilePosition) && IsNeighbor(endPosition, lastTilePosition);
        }
        
        private static bool IsNeighbor(Vector3Int referencePosition, Vector3Int evaluatedPosition)
        {
            var isHorizontalNeighbor = 
                (evaluatedPosition.x == referencePosition.x - 1 || evaluatedPosition.x == referencePosition.x + 1) && evaluatedPosition.y == referencePosition.y;
            var isVerticalNeighbor = 
                (evaluatedPosition.y == referencePosition.y - 1 || evaluatedPosition.y == referencePosition.y + 1) && evaluatedPosition.x == referencePosition.x;

            return isHorizontalNeighbor || isVerticalNeighbor;
        }
    }
}
