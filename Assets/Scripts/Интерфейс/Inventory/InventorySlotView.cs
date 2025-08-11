using Game.Player;
using Scriptable_Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Интерфейс.Inventory {

    public class InventorySlotView : MonoBehaviour {

        [Header("Components")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI countText;
        
        [Header("Settings")]
        [SerializeField] private ItemsSettings itemsSettings;

        private InventorySlot currentSlot;

        public void Init(InventorySlot slot) {
            currentSlot = slot;
            UpdateVisuals();
        }

        private void UpdateVisuals() {
            if (currentSlot.IsEmpty) {
                ShowEmptySlot();
            } else {
                ShowFilledSlot();
            }
        }

        private void ShowEmptySlot() {
            itemIcon.enabled = false;
            itemIcon.sprite = null;
            countText.text = "";
            countText.gameObject.SetActive(false);
        }

        private void ShowFilledSlot() {
            var itemConfig = GetItemConfiguration(currentSlot.itemId);
            
            if (itemConfig != null) {
                itemIcon.enabled = true;
                itemIcon.sprite = itemConfig.sprite;
                
                // Показываем количество только если больше 1
                if (currentSlot.count > 1) {
                    countText.text = currentSlot.count.ToString();
                    countText.gameObject.SetActive(true);
                } else {
                    countText.text = "";
                    countText.gameObject.SetActive(false);
                }
            } else {
                // Fallback если конфигурация не найдена
                ShowEmptySlot();
                Debug.LogWarning($"ItemConfiguration not found for ItemId: {currentSlot.itemId}");
            }
        }

        private ItemConfiguration GetItemConfiguration(ItemId itemId) {
            if (itemsSettings == null || itemsSettings.items == null) {
                Debug.LogError("ItemsSettings or items array is null!");
                return null;
            }

            foreach (var item in itemsSettings.items) {
                if (item.id == itemId) {
                    return item;
                }
            }
            
            return null;
        }

        // Для дебага в инспекторе
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnValidate() {
            if (itemIcon == null) {
                itemIcon = GetComponentInChildren<Image>();
            }
            
            if (countText == null) {
                countText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}