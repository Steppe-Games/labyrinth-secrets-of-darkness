using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Grid {

    public class TilemapShadowCasterSpawner : MonoBehaviour {

        //@formatter:off
        [Header("Настройки")] 
        [SerializeField] private Tilemap sourceTilemap;
        [SerializeField] private Transform shadowContainer;
        [SerializeField] private GameObject shadowCasterPrefab;

        [SerializeField] private TileBase[] shadowTiles; 
        //@formatter:on
        
        private void Awake() {
            GridChannels.ShadowTiles
                .Subscribe(shadowTiles => {
                    this.shadowTiles = shadowTiles;
                    SpawnShadowCasters();
                })
                .AddTo(this);
        }

        private void SpawnShadowCasters() {
            if (sourceTilemap == null) {
                Debug.LogError("Source Tilemap не назначен!");
                return;
            }

            if (shadowContainer == null) {
                Debug.LogError("Shadow Container не назначен!");
                return;
            }

            if (shadowCasterPrefab == null) {
                Debug.LogError("Shadow Caster Prefab не назначен!");
                return;
            }

            // Получаем границы тайлмапа
            BoundsInt bounds = sourceTilemap.cellBounds;

            // Проходим по всем ячейкам
            for (int x = bounds.xMin; x < bounds.xMax; x++) {
                for (int y = bounds.yMin; y < bounds.yMax; y++) {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    TileBase currentTile = sourceTilemap.GetTile(cellPosition);

                    // Проверяем, нужно ли создавать shadow caster для этого тайла
                    if (ShouldProcessTile(currentTile)) {
                        CreateShadowCaster(cellPosition);
                    }
                }
            }

            Debug.Log($"Создано shadow caster объектов в контейнере: {shadowContainer.childCount}");
        }

        private bool ShouldProcessTile(TileBase tile) {
            if (tile == null) 
                return false;

            return shadowTiles
                .Any(targetTile => tile == targetTile);
        }

        private void CreateShadowCaster(Vector3Int cellPosition) {
            // Конвертируем позицию ячейки в мировые координаты
            Vector3 worldPosition = sourceTilemap.CellToWorld(cellPosition);

            // Добавляем смещение к центру ячейки
            worldPosition += sourceTilemap.tileAnchor;

            // Создаем объект
            GameObject shadowCaster = Instantiate(shadowCasterPrefab, worldPosition, Quaternion.identity, shadowContainer);

            // Опционально: даем осмысленное имя
            shadowCaster.name = $"ShadowCaster_{cellPosition.x}_{cellPosition.y}";
        }

        // Метод для тестирования в редакторе
        [ContextMenu("Пересоздать Shadow Casters")]
        private void RecreateDebug() {
            // Удаляем существующие
            for (int i = shadowContainer.childCount - 1; i >= 0; i--) {
                DestroyImmediate(shadowContainer.GetChild(i).gameObject);
            }

            // Создаем заново
            SpawnShadowCasters();
        }

    }

}