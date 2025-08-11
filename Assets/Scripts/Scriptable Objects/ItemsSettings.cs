using System;
using UnityEngine;

namespace Scriptable_Objects {

    [CreateAssetMenu(menuName = "Labyrinth/Items Settings")] 

    public class ItemsSettings : ScriptableObject {
        
        public ItemConfiguration[] items;
        
    }

    [Serializable]
    public class ItemConfiguration {
        [Tooltip("Уникальный идентификатор предмета")]
        public ItemId id;
        [Tooltip("Техническое название предмета (только для разработки)")]
        public string name;
        public Sprite sprite;

        [Header("Сортировка и хранение")]
        public int sortOrder;
        [Range(1, 999)]
        public int maxStackSize = 10;

    }

    public enum ItemId {

        NONE = 0,
        TORCH = 1,
        LOCKPICK = 2,
        HP_BOTTLE = 3,
        BRONZE_KEY = 4,
        SILVER_KEY = 5,
        GOLDEN_KEY = 6,
        MITHRIL_KEY = 7,
        GEM_1,
        GEM_2,
        GEM_3,
        GEM_4,
        GEM_5,
        GEM_6,
        GEM_7,
    }
}