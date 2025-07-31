using Game.Player;
using Scriptable_Objects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Интерфейс.Game_UI {

    [RequireComponent(typeof(Button))]
    public class DrinkHpBottle : MonoBehaviour {

        private Button button;

        private PlayerInventory inventory;
        private int HpBottlesCount => inventory.GetItemCount(ItemId.HP_BOTTLE);
        
        private void Awake() {
            button = GetComponent<Button>();

            PlayerChannels.Inventory
                .Where(it => it != null)
                .Take(1)
                .Subscribe(InitInventory);
        }

        private void InitInventory(PlayerInventory inventory) {
            this.inventory = inventory;
            
            this.inventory.InventoryChanged
                .Where(id => id == ItemId.HP_BOTTLE)
                .Subscribe(_ => OnHpBottleQuantityChanged())
                .AddTo(this);
        }

        private void OnHpBottleQuantityChanged() {
            button.interactable = HpBottlesCount > 0;
        }

        private void OnEnable() {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable() {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick() {
            if (inventory.Withdraw(ItemId.HP_BOTTLE, 1)) {
                PlayerChannels.Health.Value++;
            }
        }
        
    }

}