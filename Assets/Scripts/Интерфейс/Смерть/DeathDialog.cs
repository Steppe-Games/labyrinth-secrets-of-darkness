using System;
using Game.Player;
using UniRx;
using UnityEngine;

namespace Интерфейс.Смерть {

    public class DeathDialog : MonoBehaviour {

        //@formatter:off
        [SerializeField] private GameObject deathDialog;
        //@formatter:off
        
        private void Awake() {
            PlayerChannels.IsDead
                .Throttle(TimeSpan.FromSeconds(0.1f))
                .Subscribe(OnPlayerDeath)
                .AddTo(this);
        }

        private void OnPlayerDeath(bool dead) {
            deathDialog.SetActive(dead);
        }

    }

}