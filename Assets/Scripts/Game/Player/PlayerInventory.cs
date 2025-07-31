using System.Collections.Generic;
using Scriptable_Objects;
using UniRx;
using UnityEngine;

namespace Game.Player {

    public class PlayerInventory : MonoBehaviour {

        //@formatter:off
        [SerializeField] private ItemsSettings itemSettings;
        //@formatter:on

        public ReactiveCommand<ItemId> InventoryChanged { get; }= new();

        public Dictionary<ItemId, ItemConfiguration> ItemReference { get; } = new();
        
        private Dictionary<ItemId, int> inventory = new();

        private void Awake() {
            foreach (ItemConfiguration item in itemSettings.items) {
                ItemReference[item.id] = item;
            }
            
            inventory[ItemId.TORCH] = 3;
            inventory[ItemId.LOCKPICK] = 2;
            inventory[ItemId.HP_BOTTLE] = 7;
        }

        public int GetItemCount(ItemId id) {
            return inventory.GetValueOrDefault(id, 0);
        }

        public void Deposit(ItemId id, int amount) {
            if (amount <= 0) 
                return;
    
            if (!inventory.TryAdd(id, amount)) {
                inventory[id] += amount;
            }

            InventoryChanged.Execute(id);
        }

        public bool Withdraw(ItemId id, int amount) {
            if (amount <= 0) 
                return false;
    
            if (GetItemCount(id) < amount)
                return false;
    
            inventory[id] -= amount;
    
            if (inventory[id] == 0) 
                inventory.Remove(id);
    
            InventoryChanged.Execute(id);
            return true;
        }

    }

}