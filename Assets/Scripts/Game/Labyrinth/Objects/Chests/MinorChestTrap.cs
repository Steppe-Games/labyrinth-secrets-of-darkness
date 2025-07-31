using System;
using System.Collections;
using Common.UniRx;
using DG.Tweening;
using Game.Player;
using Scriptable_Objects;
using UniRx;
using UnityEngine;

namespace Game.Labyrinth.Objects {

    public class MinorChestTrap : MonoBehaviour {

        //@formatter:off
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrapsSettings trapsSettings;
        [SerializeField] private CollisionDetector damageZoneDetector;
        [SerializeField] private CollisionDetector immediateDamageZoneDetector;
        [SerializeField] private ParticleSystem explosionPrefab;
        [SerializeField] private AudioSource audioSource;
        //@formatter:on

        private PlayerController player;
        private Sequence blinkSequence;

        private void Awake() {
            // Подписываемся на изменения в зоне поражения
            damageZoneDetector.CollidedObject
                .Where(col2D => col2D != null)
                .Take(1)
                .Subscribe(_ => StartCoroutine(ActivateTrap()));

            // Подписываемся на изменения в зоне немедленного поражения
            immediateDamageZoneDetector.CollidedObject
                .Where(col2D => col2D != null)
                .Take(1)
                .Subscribe(OnImmediateExplosion);
        }

        private void OnDestroy() {
            blinkSequence?.Kill();
            StopAllCoroutines();
        }

        private void OnImmediateExplosion(Collider2D other) {
            player = other.GetComponent<PlayerController>();
            if (player == null)
                throw new Exception("Object tagged as Player doesn't have a PlayerController.");

            Explode();
        }

        private IEnumerator ActivateTrap() {
            // Запускаем звуковой сигнал
            PlayWarningSound();
            
            // Запускаем мигание
            StartBlinking();
            
            // Запускаем таймер взрыва
            yield return new WaitForSeconds(trapsSettings.chestTrapDelay);
            Explode();
        }

        private void Explode() {
            // Останавливаем мигание
            blinkSequence?.Kill();
            
            AnimateExplosion();
            
            damageZoneDetector.CollidedObject.Value
                ?.GetComponent<PlayerController>()
                ?.TakeHit(trapsSettings.chestTrapDamage);

            // Уничтожаем ловушку через небольшую задержку
            Destroy(gameObject, trapsSettings.chestTrapDestroyDelay);
        }

        private void StartBlinking() {
            // Останавливаем предыдущее мигание
            blinkSequence?.Kill();
            
            Color originalColor = spriteRenderer.color;
            
            // Создаем последовательность мигания
            blinkSequence = DOTween.Sequence();
            
            blinkSequence
                .Append(spriteRenderer.DOColor(Color.red, 0.2f).SetEase(Ease.InQuad))
                .Append(spriteRenderer.DOColor(originalColor, 0.2f).SetEase(Ease.OutQuad));

            blinkSequence.SetLoops(-1);
            
            // Запускаем анимацию
            blinkSequence.Play();
        }

        private void AnimateExplosion() {
            PlayExplosionSound();

            ParticleSystem explosion = Instantiate(explosionPrefab, transform);
            explosion.transform.localScale = Vector3.one * .5f;
        }

        private void PlayWarningSound() {
            if (audioSource != null && trapsSettings?.chestTrapWarningSound != null) {
                audioSource.PlayOneShot(trapsSettings.chestTrapWarningSound);
            }
        }

        private void PlayExplosionSound() {
            if (audioSource != null && trapsSettings?.chestTrapExplosionSound != null) {
                audioSource.PlayOneShot(trapsSettings.chestTrapExplosionSound);
            }
        }
    }
}