using System;
using Scriptable_Objects;
using UnityEngine;

namespace Game.Labyrinth.Objects {

    public class Chest : MonoBehaviour {

        //@formatter:off
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private ItemsSettings itemSettings;
        [SerializeField] private ChestConfig chestConfig;
        //@formatter:on

        private void Awake() {
            spriteRenderer.sprite = chestConfig.lockedChest;
        }

    }

}