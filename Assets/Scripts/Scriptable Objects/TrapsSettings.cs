using UnityEngine;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Traps Settings")]
    public class TrapsSettings : ScriptableObject {

        //@formatter:off
        [Header("Шипы")]
        public int spikeDamage;
        [Tooltip("Повторный урон если игрок стоит на шипах дольше N сек")]
        public float spikeDamagePeriod;

        [Header("Огненный колодец")]
        public int fireballDamage;
        public float fireballSpawnPeriod;
        public float fireballVelocity;
        
        [Header("Сундук ловушка")]
        public int chestTrapDamage = 1;
        public float chestTrapDelay = 2f;
        public float chestTrapDestroyDelay = 0.5f;
        
        [Header("Chest Trap Audio")]
        public AudioClip chestTrapWarningSound;
        public AudioClip chestTrapExplosionSound;
        //@formatter:on

    }

}