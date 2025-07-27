using System.Linq;
using Game.Player;
using UnityEngine;

namespace Game.Labyrinth.Objects {

    public class RespawnPoint : MonoBehaviour {

        private void Awake() {
            PlayerChannels.RespawnPositions.Value = PlayerChannels.RespawnPositions.Value
                .Concat(new[] { transform.position })
                .ToList();
        }

        private void OnDestroy() {
            PlayerChannels.RespawnPositions.Value = PlayerChannels.RespawnPositions.Value
                .Where(it => it != transform.position)
                .ToList();
        }

    }

}