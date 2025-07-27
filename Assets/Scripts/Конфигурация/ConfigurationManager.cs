using System;
using Scriptable_Objects;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Конфигурация {

    public static class ConfigChannels {

        public static ReactiveProperty<GameSettings> GameSettings { get; } = new();
        public static ReactiveProperty<LabyrinthSettings> LabyrinthSettings { get; } = new();
        public static ReactiveProperty<PlayerSettings> PlayerSettings { get; } = new();

    }
    
    public class ConfigurationManager : MonoBehaviour {

        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private LabyrinthSettings labyrinthSettings;
        [SerializeField] private PlayerSettings playerSettings;

        private void Awake() {
            ConfigChannels.GameSettings.Value = gameSettings;
            ConfigChannels.LabyrinthSettings.Value = labyrinthSettings;
            ConfigChannels.PlayerSettings.Value = playerSettings;
        }

    }

}