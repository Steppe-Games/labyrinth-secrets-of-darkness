using Scriptable_Objects;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Player {

    /// <summary>
    /// Контроллер для реалистичного мерцания факела
    /// Управляет Light2D компонентом для имитации живого огня
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class TorchLightController : MonoBehaviour {

        //@formatter:off
        [Header("Общие настройки игрока")]
        [SerializeField] private PlayerSettings playerSettings;
        
        [Header("Цветовые настройки")] 
        [SerializeField] private Color warmColor = new Color(1f, 0.6f, 0.2f); // Теплый оранжевый
        [SerializeField] private Color coolColor = new Color(1f, 0.8f, 0.4f); // Более холодный желтый

        [Header("Радиус освещения")] 
        [SerializeField] private Vector2 outerRadiusRange = new Vector2(2.0f, 3.0f);
        [SerializeField] private Vector2 innerRadiusRange = new Vector2(0.3f, 0.8f);

        [Header("Интенсивность")] 
        [SerializeField] private Vector2 intensityRange = new Vector2(0.8f, 1.5f);

        [Header("Настройки анимации")] 
        [SerializeField] private float flickerSpeed = 8f;
        [SerializeField] private float colorChangeSpeed = 3f;
        [SerializeField] private AnimationCurve flickerCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Дополнительные эффекты")] 
        [SerializeField] private bool enableWindEffect = true;
        [SerializeField] private float windStrength = 0.3f;
        [SerializeField] private float windSpeed = 2f;

        [Header("Случайные всплески")] 
        [SerializeField] private bool enableRandomFlares = true;
        [SerializeField] private float flareChance = 0.02f; // 2% шанс каждый кадр
        [SerializeField] private float flareDuration = 0.3f;
        [SerializeField] private float flareIntensityMultiplier = 1.8f;
        //@formatter:on

        private Light2D torchLight;
        private float timeOffset;
        private bool isFlaring;
        private float flareTimer;
        private float baseIntensity;
        private float baseOuterRadius;
        private float baseInnerRadius;

        void Start() {
            torchLight = GetComponent<Light2D>();

            // Убеждаемся что тип света правильный
            if (torchLight.lightType != Light2D.LightType.Point) {
                torchLight.lightType = Light2D.LightType.Point;
            }

            // Если заведены настройки игрока, берем радиус освещения оттуда
            if (playerSettings != null) {
                float range = outerRadiusRange.y - outerRadiusRange.x;
                outerRadiusRange.x = playerSettings.torchRadius - range / 2;
                outerRadiusRange.y = playerSettings.torchRadius + range / 2;
            }

            // Случайное смещение времени для каждого факела
            timeOffset = Random.Range(0f, 100f);

            // Сохраняем базовые значения
            baseIntensity = Mathf.Lerp(intensityRange.x, intensityRange.y, 0.5f);
            baseOuterRadius = Mathf.Lerp(outerRadiusRange.x, outerRadiusRange.y, 0.5f);
            baseInnerRadius = Mathf.Lerp(innerRadiusRange.x, innerRadiusRange.y, 0.5f);
        }

        void Update() {
            AnimateTorch();
            HandleRandomFlares();
        }

        void AnimateTorch() {
            float time = Time.time + timeOffset;

            // Основное мерцание с использованием Perlin noise для плавности
            float flickerNoise = Mathf.PerlinNoise(time * flickerSpeed, 0f);
            float smoothFlicker = flickerCurve.Evaluate(flickerNoise);

            // Цветовое изменение
            float colorNoise = Mathf.PerlinNoise(time * colorChangeSpeed, 100f);
            Color currentColor = Color.Lerp(warmColor, coolColor, colorNoise);

            // Эффект ветра (если включен)
            float windEffect = 1f;
            if (enableWindEffect) {
                windEffect = 1f + Mathf.Sin(time * windSpeed) * windStrength * 0.5f;
            }

            // Вычисляем финальные значения
            float finalIntensity = Mathf.Lerp(intensityRange.x, intensityRange.y, smoothFlicker) * windEffect;
            float finalOuterRadius = Mathf.Lerp(outerRadiusRange.x, outerRadiusRange.y, smoothFlicker) * windEffect;
            float finalInnerRadius = Mathf.Lerp(innerRadiusRange.x, innerRadiusRange.y, smoothFlicker);

            // Применяем модификатор всплеска
            if (isFlaring) {
                float flareProgress = 1f - (flareTimer / flareDuration);
                float flareEffect = Mathf.Sin(flareProgress * Mathf.PI) * flareIntensityMultiplier;
                finalIntensity *= (1f + flareEffect);
                finalOuterRadius *= (1f + flareEffect * 0.5f);
            }

            // Применяем к источнику света
            torchLight.color = currentColor;
            torchLight.intensity = finalIntensity;
            torchLight.pointLightOuterRadius = finalOuterRadius;
            torchLight.pointLightInnerRadius = finalInnerRadius;
        }

        void HandleRandomFlares() {
            if (!enableRandomFlares) return;

            if (isFlaring) {
                flareTimer -= Time.deltaTime;
                if (flareTimer <= 0f) {
                    isFlaring = false;
                }
            }
            else {
                // Случайный шанс создать всплеск
                if (Random.value < flareChance * Time.deltaTime * 60f) // Нормализация к 60 FPS
                {
                    StartFlare();
                }
            }
        }

        void StartFlare() {
            isFlaring = true;
            flareTimer = flareDuration;
        }

        // Публичные методы для внешнего управления
        public void SetIntensityRange(Vector2 newRange) {
            intensityRange = newRange;
        }

        public void SetRadiusRange(Vector2 outerRange, Vector2 innerRange) {
            outerRadiusRange = outerRange;
            innerRadiusRange = innerRange;
        }

        public void SetColors(Color warm, Color cool) {
            warmColor = warm;
            coolColor = cool;
        }

        public void TriggerFlare() {
            StartFlare();
        }

        // Для отладки в редакторе
        void OnValidate() {
            // Проверяем что диапазоны корректные
            if (outerRadiusRange.x > outerRadiusRange.y) {
                outerRadiusRange.y = outerRadiusRange.x;
            }

            if (innerRadiusRange.x > innerRadiusRange.y) {
                innerRadiusRange.y = innerRadiusRange.x;
            }

            if (intensityRange.x > intensityRange.y) {
                intensityRange.y = intensityRange.x;
            }
        }

    }

}