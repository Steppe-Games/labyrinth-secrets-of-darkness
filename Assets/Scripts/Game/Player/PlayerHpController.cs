using Scriptable_Objects;
using UniRx;
using UnityEngine;
using Конфигурация;

namespace Game.Player {

    public class PlayerHpController : MonoBehaviour {

        private PlayerSettings playerSettings;

        private void Awake() {
            ConfigChannels.PlayerSettings
                .Where(it => it != null)
                .Take(1)
                .Subscribe(settings => {
                    playerSettings = settings;

                    PlayerChannels.Health
                        .Subscribe(OnHealthChanged)
                        .AddTo(this);
                });
        }

        private void OnHealthChanged(int hp) {
            if (hp > playerSettings.maximumLife)
                PlayerChannels.Health.Value = playerSettings.maximumLife;
        }

    }

}