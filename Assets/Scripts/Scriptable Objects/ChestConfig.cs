using Common.Enums;
using UnityEngine;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Конфигурация сундука")]
    public class ChestConfig : ScriptableObject {

        public Vector2Int commonItemsRange;
        public Vector2Int keysRange;
        public KeyType keyType;
        public Vector2Int gemsRange;

        public Sprite lockedChest;
        public Sprite unlockedChest;
        
        public ArtifactType artifactType;

        public KeyType unlockBy;
        public int pickLockChance;

    }

}