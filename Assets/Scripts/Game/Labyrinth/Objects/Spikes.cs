using System;
using System.Collections;
using Game.Player;
using Scriptable_Objects;
using UnityEngine;

namespace Game.Labyrinth.Objects {

    [RequireComponent(typeof(BoxCollider2D))]
    public class Spikes : MonoBehaviour {

        [SerializeField] private TrapsSettings trapsSettings;

        private PlayerController player;
        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void OnDestroy() {
            StopAllCoroutines();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player"))
                return;

            player = other.GetComponent<PlayerController>();
            if (player == null)
                throw new Exception("Object tagged as Player doesn't have a PlayerController.");

            StartCoroutine(DealDamage());
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (!other.CompareTag("Player"))
                return;

            player = null;
            StopAllCoroutines();
        }
        
        private IEnumerator DealDamage() {
            while (player != null) {
                // Проигрываем анимацию при каждом ударе
                animator.Play(0); // Проигрываем состояние по индексу 0
                
                player.TakeHit(trapsSettings.spikeDamage);
                yield return new WaitForSeconds(trapsSettings.spikeDamagePeriod);
            }
        }

    }

}