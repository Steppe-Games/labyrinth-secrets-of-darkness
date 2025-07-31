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
    }

    public enum ItemId {

        TORCH,
        LOCKPICK,
        HP_BOTTLE,
        KEY_1,
        KEY_2,
        KEY_3,
        GEM_1,
        GEM_2,
        GEM_3,
        GEM_4,
        GEM_5,
        GEM_6,
        GEM_7,

    }
}