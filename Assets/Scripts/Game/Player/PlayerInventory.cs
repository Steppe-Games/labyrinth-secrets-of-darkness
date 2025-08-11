using System;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UniRx;
using UnityEngine;

namespace Game.Player {

    [Serializable]
    public struct InventoryEntry {
        public ItemId itemId;
        public int count;

        public InventoryEntry(ItemId itemId, int count) {
            this.itemId = itemId;
            this.count = count;
        }
    }

    [Serializable]
    public struct InventorySlot {
        public ItemId itemId;
        public int count;
        public bool IsEmpty => itemId == ItemId.NONE || count <= 0;

        public static InventorySlot Empty => new() { itemId = ItemId.NONE, count = 0 };
        
        public InventorySlot(ItemId itemId, int count) {
            this.itemId = itemId;
            this.count = count;
        }
    }

    public class PlayerInventory {

        private const int MAX_SLOTS = 12;

        public static ReactiveCommand<ItemId> InventoryChanged { get; } = new();
        public static IReadOnlyReactiveProperty<InventorySlot[]> InventorySlotsChanged => inventorySlots;
        private static ReactiveProperty<InventorySlot[]> inventorySlots = new(Array.Empty<InventorySlot>());

        public Dictionary<ItemId, ItemConfiguration> ItemReference { get; } = new();
        
        private List<InventoryEntry> inventory = new();

        public PlayerInventory(ItemsSettings itemSettings) {
            foreach (ItemConfiguration item in itemSettings.items) {
                ItemReference[item.id] = item;
            }
            
            // Инициализация стартового инвентаря
            inventory.Add(new InventoryEntry(ItemId.TORCH, 3));
            inventory.Add(new InventoryEntry(ItemId.LOCKPICK, 2));
            inventory.Add(new InventoryEntry(ItemId.HP_BOTTLE, 7));
            
            // Инициализация слотов
            UpdateSlots();
        }

        public int GetItemCount(ItemId id) {
            return inventory.Where(entry => entry.itemId == id)
                          .Sum(entry => entry.count);
        }

        public bool Deposit(ItemId id, int amount) {
            if (amount <= 0) 
                return false;

            // Создаем временный инвентарь с добавленным предметом
            var tempInventory = new List<InventoryEntry>(inventory);
            
            // Ищем существующую запись для этого предмета
            var existingIndex = tempInventory.FindIndex(entry => entry.itemId == id);
            if (existingIndex >= 0) {
                var existing = tempInventory[existingIndex];
                tempInventory[existingIndex] = new InventoryEntry(id, existing.count + amount);
            } else {
                tempInventory.Add(new InventoryEntry(id, amount));
            }

            // Проверяем, поместится ли в слоты
            var tempSlots = MapInventoryToSlots(tempInventory);
            if (!CanFitInSlots(tempSlots)) {
                return false; // Не помещается
            }

            // Применяем изменения
            inventory = tempInventory;
            UpdateSlots();
            InventoryChanged.Execute(id);
            return true;
        }

        public bool Withdraw(ItemId id, int amount) {
            if (amount <= 0) 
                return false;
    
            if (GetItemCount(id) < amount)
                return false;
    
            // Убираем предметы из инвентаря
            int remainingToRemove = amount;
            for (int i = inventory.Count - 1; i >= 0 && remainingToRemove > 0; i--) {
                if (inventory[i].itemId != id) continue;

                var entry = inventory[i];
                int removeFromThis = Math.Min(remainingToRemove, entry.count);
                
                if (removeFromThis >= entry.count) {
                    inventory.RemoveAt(i);
                } else {
                    inventory[i] = new InventoryEntry(id, entry.count - removeFromThis);
                }
                
                remainingToRemove -= removeFromThis;
            }

            UpdateSlots();

            InventoryChanged.Execute(id);
            return true;
        }

        private void UpdateSlots() {
            InventorySlot[] mapInventoryToSlots = MapInventoryToSlots(inventory);
            inventorySlots.Value = mapInventoryToSlots;
        }

        private InventorySlot[] MapInventoryToSlots(List<InventoryEntry> inventoryData) {
            // Map фаза: группируем по предметам и сортируем
            var groupedItems = inventoryData
                .Where(entry => entry.count > 0)
                .GroupBy(entry => entry.itemId)
                .Select(group => new {
                    ItemId = group.Key,
                    TotalCount = group.Sum(entry => entry.count),
                    SortOrder = GetSortOrder(group.Key)
                })
                .OrderBy(item => item.SortOrder)
                .ToList();

            // Reduce фаза: разбиваем на стеки и размещаем в слоты
            var slots = new List<InventorySlot>();
            
            foreach (var item in groupedItems) {
                int remainingCount = item.TotalCount;
                
                while (remainingCount > 0) {
                    int stackSize = Math.Min(remainingCount, GetMaxStackSize(item.ItemId));
                    slots.Add(new InventorySlot(item.ItemId, stackSize));
                    remainingCount -= stackSize;
                }
            }

            // Дополняем пустыми слотами до MAX_SLOTS
            while (slots.Count < MAX_SLOTS) {
                slots.Add(InventorySlot.Empty);
            }

            return slots.ToArray();
        }

        private bool CanFitInSlots(InventorySlot[] slots) {
            return slots.Count(slot => !slot.IsEmpty) <= MAX_SLOTS;
        }

        private int GetSortOrder(ItemId itemId) {
            return ItemReference.TryGetValue(itemId, out var config) ? config.sortOrder : 999;
        }

        private int GetMaxStackSize(ItemId itemId) {
            return ItemReference.TryGetValue(itemId, out var config) ? config.maxStackSize : 10;
        }
    }
}