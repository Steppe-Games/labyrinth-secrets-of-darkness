using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Labyrinth.Objects.Fireball_Trap {

    public class Fireball : MonoBehaviour {

        private Rigidbody2D rb2d;
        private BoxCollider2D col2d;
        private void Awake() {
            rb2d = GetComponent<Rigidbody2D>();
            col2d = GetComponent<BoxCollider2D>();

            rb2d.linearVelocityX = 0.1f;
        }
        
        private void OnTriggerEnter2D(Collider2D other) {
            Debug.Log($"Unity Trigger Enter: {other.name} (Type: {other.GetType().Name})");
        }
        
        private void OnTriggerExit2D(Collider2D other) {
            Debug.Log($"Unity Trigger Exit: {other.name} (Type: {other.GetType().Name})");
        }
   }
}