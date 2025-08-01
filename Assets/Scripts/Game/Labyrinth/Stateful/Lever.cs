using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Labyrinth.Stateful {

    public class Lever : MonoBehaviour {

        private static readonly Color ACTIVE  = Color.white;
        private static readonly Color INACTIVE = Color.gray;
        
        //@formatter:off
        [Header("Lever Settings")] 
        public bool isActivated = false;

        [Header("Events")] 
        public UnityEvent<bool> onStateChanged;

        [Header("Visual")] 
        public SpriteRenderer buttonSR;
        //@formatter:on


        private void Start() {
            if (buttonSR == null)
                throw new UnityException("Button sprite renderer not set");

            UpdateVisual();
        }

        public void ToggleLever() {
            isActivated = !isActivated;

            onStateChanged?.Invoke(isActivated);
            UpdateVisual();
        }

        public void SetState(bool state) {
            if (isActivated != state) {
                isActivated = state;

                onStateChanged?.Invoke(isActivated);
                UpdateVisual();
            }
        }

        private void UpdateVisual() {
            buttonSR.color = isActivated switch {
                true => ACTIVE,
                false => INACTIVE,
            };
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player"))
                ToggleLever();
        }

#if UNITY_EDITOR
        // Весь editor-only код
        private GameObject[] cachedConnectedObjects;
        private bool cacheIsValid = false;

        private void OnValidate() {
            RefreshConnectionCache();
            UpdateVisual();
        }

        private void RefreshConnectionCache() {
            var connections = new List<GameObject>();

            for (int i = 0; i < onStateChanged.GetPersistentEventCount(); i++) {
                var target = onStateChanged.GetPersistentTarget(i);
                if (target is GameObject go) {
                    connections.Add(go);
                }

                if (target is Component cmp) {
                    connections.Add(cmp.gameObject);
                }
            }

            cachedConnectedObjects = connections.ToArray();
            cacheIsValid = true;
        }

        private void OnDrawGizmos() {
            if (!cacheIsValid) {
                RefreshConnectionCache();
            }

            if (IsAnyConnectedObjectSelected()) {
                DrawConnectionGizmos();
            }
        }

        private void OnDrawGizmosSelected() {
            DrawConnectionGizmos();
        }

        private bool IsAnyConnectedObjectSelected() {
            if (cachedConnectedObjects == null) return false;

            var selection = Selection.gameObjects;
            foreach (var connectedObj in cachedConnectedObjects) {
                if (connectedObj != null && Array.IndexOf(selection, connectedObj) >= 0) {
                    return true;
                }
            }

            return false;
        }

        private void DrawConnectionGizmos() {
            if (cachedConnectedObjects == null) return;

            foreach (var target in cachedConnectedObjects) {
                if (target != null) {
                    bool targetSelected = Array.IndexOf(Selection.gameObjects, target) >= 0;
                    bool leverSelected = Array.IndexOf(Selection.gameObjects, gameObject) >= 0;

                    if (leverSelected) {
                        Gizmos.color = Color.yellow;
                    }
                    else if (targetSelected) {
                        Gizmos.color = Color.cyan;
                    }
                    else {
                        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
                    }

                    Gizmos.DrawLine(transform.position, target.transform.position);

                    // Стрелка
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    Vector3 arrowHead = target.transform.position - direction * 0.3f;
                    Vector3 right = Vector3.Cross(Vector3.forward, direction) * 0.1f;

                    Gizmos.DrawLine(target.transform.position, arrowHead + right);
                    Gizmos.DrawLine(target.transform.position, arrowHead - right);
                }
            }
        }
#endif

    }

}