using UnityEngine;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Player Settings")]
    public class PlayerSettings : ScriptableObject {

        [Tooltip("Максимальное количество здоровья игрока")]
        public int maximumLife;

        [Tooltip("Радиус освещения факела")]
        public float torchRadius;

        [Tooltip("Время одного шага (сек)")]
        public float stepDuration;

    }

}