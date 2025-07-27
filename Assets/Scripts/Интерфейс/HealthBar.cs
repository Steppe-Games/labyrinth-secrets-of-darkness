using System;
using System.Collections.Generic;
using Common.Extensions;
using Game.Player;
using UniRx;
using UnityEngine;

namespace Интерфейс {

    public class HealthBar : MonoBehaviour {

        //@formatter:off
        [SerializeField] private GameObject heartPrefab;
        //@formatter:on

        private List<GameObject> heartObjects = new();

        private void Awake() {
            if (heartPrefab == null)
                throw new Exception("HealthBar: heartPrefab is not assigned!");

            transform.RemoveAllChildren();

            PlayerChannels.Health
                .Subscribe(OnHealthChanged)
                .AddTo(this);
        }

        private void OnHealthChanged(int hp) {
            UpdateHeartsCount(hp);
        }

        private void UpdateHeartsCount(int targetHealth) {
            // Если нужно больше сердечек, создаем их
            while (heartObjects.Count < targetHealth) {
                CreateHeart();
            }

            // Если нужно меньше сердечек, удаляем лишние
            while (heartObjects.Count > Mathf.Max(targetHealth, 0)) {
                RemoveHeart();
            }
        }

        private void CreateHeart() {
            GameObject heart = Instantiate(heartPrefab, transform);
            heartObjects.Add(heart);
        }

        private void RemoveHeart() {
            if (heartObjects.Count <= 0) 
                return;
            
            GameObject lastHeart = heartObjects[0];
            heartObjects.RemoveAt(0);
                
            if (lastHeart != null) {
                Destroy(lastHeart);
            }
        }
    }
}