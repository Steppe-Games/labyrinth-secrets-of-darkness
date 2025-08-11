using Game.Player;
using UniRx;
using UnityEngine;

namespace Интерфейс.Inventory {

    public class InventoryScreen : MonoBehaviour {

        [Header("Setup")]
        [SerializeField] private Transform slotsContainer;
        [SerializeField] private InventorySlotView slotPrefab;

        private void Awake() {
            PlayerInventory.InventorySlotsChanged
                .Subscribe(OnInventorySlotsChanged)
                .AddTo(this);
        }

        private void OnInventorySlotsChanged(InventorySlot[] slots) {
            UpdateSlotViews(slots);
        }

        private void UpdateSlotViews(InventorySlot[] slots) {
            for (int i = slotsContainer.childCount - 1; i >= 0; i--) {
                Destroy(slotsContainer.GetChild(i).gameObject);
            }

            foreach (InventorySlot slot in slots) {
                InventorySlotView slotView = Instantiate(slotPrefab, slotsContainer);
                slotView.Init(slot);
            }
        }
    }
}