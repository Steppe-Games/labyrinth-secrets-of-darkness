using System;
using System.Collections;
using DG.Tweening;
using Scriptable_Objects;
using UnityEngine;

namespace Game.Labyrinth.Objects.Fireball_Trap {

    public class FireballTrap : MonoBehaviour {

        //@formatter:off
        [SerializeField] private TrapsSettings trapSettings;
        
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private float initialSpawnPeriod = 0.2f;
        [SerializeField] private float postSpawnDelay = 0.2f;
        [SerializeField] private float initialScale = 0.2f;
        [SerializeField] private float postScale = 0.5f;
        //@formatter:on

        private Coroutine _spawnCoroutine;

        private void OnDestroy() {
            StopSpawning();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Agro")) {
                StartSpawning();
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Agro")) {
                StopSpawning();
            }
        }

        public void StartSpawning() {
            _spawnCoroutine ??= StartCoroutine(SpawnFireballsCoroutine());
        }

        public void StopSpawning() {
            if (_spawnCoroutine != null) {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnFireballsCoroutine() {
            while (true) {
                yield return StartCoroutine(SpawnSingleFireball());
                yield return new WaitForSeconds(trapSettings.fireballSpawnPeriod);
            }
        }

        private IEnumerator SpawnSingleFireball() {
            // Создаем огненный шар
            var fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);
            
            // Устанавливаем начальный масштаб
            fireball.transform.localScale = Vector3.one * initialScale;
            
            // Анимируем увеличение масштаба
            fireball.transform.DOScale(postScale, initialSpawnPeriod).SetEase(Ease.OutQuad);
            
            // Ждем завершения анимации масштаба
            yield return new WaitForSeconds(initialSpawnPeriod);
            
            // Пауза после увеличения
            yield return new WaitForSeconds(postSpawnDelay);
            
            // Запускаем огненный шар в полет
            LaunchFireball(fireball);
        }

        private void LaunchFireball(GameObject fireball) {
            var rb2d = fireball.GetComponent<Rigidbody2D>();
            if (rb2d != null) {
                // Направление полета - локальная ось Y трансформа ловушки
                Vector2 direction = -transform.up;
                rb2d.linearVelocity = direction * trapSettings.fireballVelocity;
            } else {
                Debug.LogWarning($"Fireball prefab {fireball.name} doesn't have Rigidbody2D component!");
            }
        }
    }
}