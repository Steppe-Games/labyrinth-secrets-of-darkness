using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Labyrinth.Objects {

    public class TilemapObjectSpawner : MonoBehaviour {
    
        [SerializeField] private LabyrinthSettings labyrinthSettings;
    
        private Tilemap tilemap;
        private Dictionary<TileBase, TileProperties> tilePropertiesDict;
    
        private void Awake() {
            tilemap = GetComponent<Tilemap>();
            BuildTilePropertiesDictionary();
        }
    
        private void Start() {
            SpawnObjects();
        }
    
        private void BuildTilePropertiesDictionary() {
            tilePropertiesDict = new Dictionary<TileBase, TileProperties>();

            if (labyrinthSettings?.tiles == null) 
                return;

            foreach (TileProperties tileProperty in labyrinthSettings.tiles) {
                if (tileProperty.spawnObject == null)
                    continue;
                
                if (tileProperty.tileBase != null) {
                    tilePropertiesDict[tileProperty.tileBase] = tileProperty;
                }
            }
        }
    
        private void SpawnObjects() {
            BoundsInt bounds = tilemap.cellBounds;
        
            for (int x = bounds.xMin; x < bounds.xMax; x++) {
                for (int y = bounds.yMin; y < bounds.yMax; y++) {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(position);
                
                    if (tile != null && tilePropertiesDict.TryGetValue(tile, out TileProperties properties)) {
                        if (properties.spawnObject != null) {
                            SpawnObjectAtPosition(position, properties.spawnObject);
                        }
                    }
                }
            }
        }
    
        private void SpawnObjectAtPosition(Vector3Int cellPosition, GameObject prefab) {
            Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
            worldPosition += tilemap.cellSize * 0.5f; // Центрируем объект в клетке
        
            Matrix4x4 tileTransform = tilemap.GetTransformMatrix(cellPosition);
            Quaternion rotation = tileTransform.rotation;
        
            Instantiate(prefab, worldPosition, rotation, transform);
        }
    }

}