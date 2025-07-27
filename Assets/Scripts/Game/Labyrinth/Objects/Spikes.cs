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
            if (player == null) {
                yield break;
            }

            player.TakeHit(trapsSettings.spikeDamage);
            yield return new WaitForSeconds(trapsSettings.spikeDamagePeriod);
            StartCoroutine(DealDamage());
        }

    }

}