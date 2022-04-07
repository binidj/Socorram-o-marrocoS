using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Prototyping.Scripts
{
    public class GridController : MonoBehaviour
    {
        
        [SerializeField] private int tilesLimit;
        [SerializeField] private int tilesCount;
        [SerializeField] private Text tilesCountTxt;
        
        [SerializeField] private Tilemap pathTileMap;
        [SerializeField] private Tile tile;

        private void Start()
        {
            tilesCount = 0;
            UpdateAvailableTilesText();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
                SetTileAtMousePosition();
        }

        private void SetTileAtMousePosition()
        {
            if (tilesCount >= tilesLimit) 
                return;
            
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var tileLocation = pathTileMap.WorldToCell(mousePosition);

            if (pathTileMap.GetTile(tileLocation))
                return;
            
            pathTileMap.SetTile(tileLocation, tile);
            tilesCount++;
            UpdateAvailableTilesText();
        }

        private void IsValidTilePosition()
        {
            
        }
        
        private void UpdateAvailableTilesText()
        {
            var updatedTilesCountTxt = $"Available tiles: {tilesLimit - tilesCount}";
            tilesCountTxt.text = updatedTilesCountTxt;
        }
    }
}
