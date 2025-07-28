using System;
using UnityEngine;

namespace Game.Labyrinth.Objects.Lantern {

    public class Lantern : MonoBehaviour {

        //@formatter:off
        [SerializeField] private GameObject lanternOff;
        [SerializeField] private GameObject lanternOn;
        //@formatter:on

        private void Awake() {
            lanternOff.SetActive(true);
            lanternOn.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag("Player"))
                return;
            
            lanternOff.SetActive(false);
            lanternOn.SetActive(true);
        }

    }

}