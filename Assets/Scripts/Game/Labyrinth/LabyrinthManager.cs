using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Labyrinth {

    public static class LabyrinthChannels {

        public static ReactiveProperty<Tilemap> Geometry { get; } = new(null);

    }

    public class LabyrinthManager : MonoBehaviour {

        //@formatter:off
        [SerializeField] private Tilemap labyrinthGeometry;
        [SerializeField] private Tilemap labyrinthObjects;
        //@formatter:on

        private void Awake() {
            LabyrinthChannels.Geometry.Value = labyrinthGeometry;
        }

    }

}