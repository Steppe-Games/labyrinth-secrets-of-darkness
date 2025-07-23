using UnityEngine;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Game Settings")]
    public class GameSettings : ScriptableObject {

        [Tooltip("Охват камеры")]
        public int cameraSize = 3;

    }

}