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
        //@formatter:on

    }

}