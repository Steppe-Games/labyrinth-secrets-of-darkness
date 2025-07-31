using Game.Player;
using Scriptable_Objects;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Интерфейс.Game_UI {

    public class InventorySlot : MonoBehaviour {

        //@formatter:off
        [SerializeField] private ItemId id;
        
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI quantityText;
        //@formatter:on
        
        private void Awake() {
            PlayerChannels.Inventory
                .Where(it => it != null)
                .Take(1)
                .Subscribe(inventory => {
                    inventory.InventoryChanged
                        .Where(eventItemId => eventItemId == id)
                        .Select(inventory.GetItemCount)
                        .Subscribe(OnInventoryChanged)
                        .AddTo(this);

                    icon.sprite = inventory.ItemReference[id].sprite;

                    OnInventoryChanged(inventory.GetItemCount(id));
                });
        }

        private void OnInventoryChanged(int quantity) {
            quantityText.text = quantity >= 10 ? "*" : quantity.ToString();
        }

    }

}