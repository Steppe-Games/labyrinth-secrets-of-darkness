using UnityEngine;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Player Settings")]
    public class PlayerSettings : ScriptableObject {

        //@formatter:off
        [Tooltip("Максимальное количество здоровья игрока")]
        public int maximumLife;

        [Tooltip("Радиус освещения факела")]
        public float torchRadius;

        [Tooltip("Время одного шага (сек)")]
        public float stepDuration;
        
        [Tooltip("Агро радиус")]
        public float agroRadius = 15;
        
        [Header("Инвентарь")]
        public ItemsSettings items;

        [Header("Библиотека звуковых эффектов")]
        [Tooltip("Звук получения урона")]
        public AudioClip hitSound;
        
        [Tooltip("Звук шагов")]
        public AudioClip stepSound;
        
        [Tooltip("Звук смерти")]
        public AudioClip deathSound;
        
        [Tooltip("Звук подбора предмета")]
        public AudioClip pickupSound;
        
        [Tooltip("Звук зажигания факела")]
        public AudioClip lightTorchSound;
         //@formatter:on

    }

}