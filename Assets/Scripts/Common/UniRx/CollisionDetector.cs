using System;
using UniRx;
using UnityEngine;

namespace Common.UniRx {

    [RequireComponent(typeof(Collider2D))]
    public class CollisionDetector : MonoBehaviour {

        [SerializeField] private string targetTag = "Player";

        public ReactiveProperty<Collider2D> CollidedObject { get; } = new();
        
        private void Awake() {
            if (!GetComponent<Collider2D>().isTrigger)
                throw new Exception($"CollisionDetector on {gameObject.name}: Collider2D is not set as trigger.");
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!other.CompareTag(targetTag))
                return;
            
            CollidedObject.Value = other;
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (!other.CompareTag(targetTag))
                return;
            
            CollidedObject.Value = null;
        }

    }

}