using System;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Grid {

    public static class GridChannels {

        public static ReactiveProperty<Tilemap> LevelTilemap { get; } = new(null);
        public static ReactiveProperty<TileBase[]> BlockingTiles { get; } = new(Array.Empty<TileBase>());
        public static ReactiveProperty<TileBase[]> ShadowTiles { get; } = new(Array.Empty<TileBase>());

    }

    public class GridManager : MonoBehaviour {

        //@formatter:off
        [SerializeField] private TileBase[] blockingTiles;
        [SerializeField] private TileBase[] shadowTiles;
        //@formatter:on

        private void Awake() {
            if (blockingTiles != null) 
                GridChannels.BlockingTiles.Value = blockingTiles;

            if (shadowTiles != null)
                GridChannels.ShadowTiles.Value = shadowTiles;

            var tilemap = GetComponentInChildren<Tilemap>();
            if (tilemap == null) {
                Debug.LogError("Компонент Tilemap не найден");
                return;
            }

            GridChannels.LevelTilemap.Value = tilemap;
        }

    }

}