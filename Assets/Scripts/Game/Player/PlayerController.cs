using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Scriptable_Objects;
using UniRx;
using UnityEngine;
using Конфигурация;
using Random = UnityEngine.Random;

namespace Game.Player {

    public static class PlayerChannels {

        public static ReactiveProperty<int> Health { get; } = new(0);
        public static ReadOnlyReactiveProperty<bool> IsDead { get; } = Health
                .Select(it => it <= 0)
                .ToReadOnlyReactiveProperty();

        public static ReactiveProperty<List<Vector3>> RespawnPositions { get; } = new(new List<Vector3>());

    }
    
    public class PlayerController : MonoBehaviour {

        //@formatter:off
        [SerializeField] private SpriteRenderer playerSR;
        [SerializeField] private AudioSource audioSource;
        //@formatter:on
        
        private PlayerContinuousMoveController moveController;

        private Sequence damageSequence;
        private PlayerSettings playerSettings;

        private void Awake() {
            moveController = GetComponentInChildren<PlayerContinuousMoveController>();


            if (playerSR == null)
                throw new Exception("PlayerController: playerSR is null");
                
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            
            ConfigChannels.PlayerSettings
                .Where(it => it != null)
                .Subscribe(OnPlayerSettingsInitialized)
                .AddTo(this);
            
            PlayerChannels.Health
                .Pairwise()
                .Subscribe(OnPlayerHealthChanged)
                .AddTo(this);
        }

        private void Start() {
            Respawn();
        }

        private void OnDestroy() {
            // Убиваем анимацию при уничтожении объекта
            damageSequence?.Kill();
        }

        public void Respawn() {
            PlayerChannels.Health.Value = playerSettings.maximumLife;
            transform.position = PlayerChannels.RespawnPositions.Value
                .OrderBy(_ => Random.value)
                .Select(pos => pos + new Vector3(-0.5f, 0, 0))
                .First();
            
            moveController.SnapToGrid();
        }
        
        public void TakeHit(int damage) {
            PlayerChannels.Health.Value -= damage;
        }

        private void OnPlayerSettingsInitialized(PlayerSettings settings) {
            playerSettings = settings;
            PlayerChannels.Health.Value = settings.maximumLife;
        }

        private void OnPlayerHealthChanged(Pair<int> health) {
            if (health.Current < health.Previous)
                AnimateTakeHit();
        }

        private void AnimateTakeHit() {
            // Останавливаем предыдущую анимацию, если она еще выполняется
            damageSequence?.Complete();
            
            // Воспроизводим звук урона из настроек
            PlaySound(playerSettings?.hitSound);
            
            // Создаем последовательность анимации мигания
            damageSequence = DOTween.Sequence();
            
            // Сохраняем исходный цвет
            Color originalColor = playerSR.color;
            
            // Создаем 5 циклов мигания за 2 секунды
            // Каждый цикл: исчезновение (0.2с) + появление (0.2с) = 0.4с на цикл
            for (int i = 0; i < 5; i++) {
                damageSequence
                    .Append(playerSR.DOColor(Color.red, 0.2f).SetEase(Ease.InQuad))
                    .Append(playerSR.DOColor(originalColor, 0.2f).SetEase(Ease.OutQuad));
            }
            
            // Убеждаемся, что в конце персонаж полностью видим
            damageSequence.OnComplete(() => {
                playerSR.color = originalColor;
            });
            
            // Запускаем анимацию
            damageSequence.Play();
        }
        
        // Универсальный метод для воспроизведения звуков
        private void PlaySound(AudioClip clip) {
            if (audioSource != null && clip != null) {
                audioSource.PlayOneShot(clip);
            }
        }
        
        // Публичные методы для воспроизведения различных звуков
        public void PlayStepSound() => PlaySound(playerSettings?.stepSound);
        public void PlayDeathSound() => PlaySound(playerSettings?.deathSound);
        public void PlayPickupSound() => PlaySound(playerSettings?.pickupSound);
        public void PlayLightTorchSound() => PlaySound(playerSettings?.lightTorchSound);

    }

}