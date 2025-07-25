﻿using System.Collections.Generic;
using Common.Input_System;
using DG.Tweening;
using Game.Grid;
using Scriptable_Objects;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Game.Player {

    public class PlayerStepMoveController : MonoBehaviour {

        //@formatter:off
        [Header("Общие настройки игрока")]
        [SerializeField] private PlayerSettings playerSettings;
            
        [Header("Movement Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;
        
        [Header("Grid Settings")]
        [SerializeField] private bool waitForGridManager = true;
        //@formatter:on

        private float stepDuration = 0.3f;

        private Queue<MoveDirection> commandQueue = new Queue<MoveDirection>();
        private bool isMoving = false;
        private bool isGridReady = false;

        // Словарь для противоположных направлений
        private readonly Dictionary<MoveDirection, MoveDirection> oppositeDirections = new Dictionary<MoveDirection, MoveDirection> {
            { MoveDirection.Left, MoveDirection.Right },
            { MoveDirection.Right, MoveDirection.Left },
            { MoveDirection.Up, MoveDirection.Down },
            { MoveDirection.Down, MoveDirection.Up }
        };

        // Словарь для векторов направлений
        private readonly Dictionary<MoveDirection, Vector2> directionVectors = new Dictionary<MoveDirection, Vector2> {
            { MoveDirection.Left, Vector2.left },
            { MoveDirection.Right, Vector2.right },
            { MoveDirection.Up, Vector2.up },
            { MoveDirection.Down, Vector2.down }
        };

        private void Start() {
            // Выравниваем позицию на сетке при старте
            SnapToGrid();

            if (playerSettings != null) {
                stepDuration = playerSettings.stepDuration;
            }

            // Подписываемся на готовность tilemap
            if (waitForGridManager) {
                GridChannels.LevelTilemap
                    .Where(tilemap => tilemap != null)
                    .Subscribe(_ => {
                        isGridReady = true;
                        Debug.Log("Grid готов для использования");
                    })
                    .AddTo(this);
            }
            else {
                isGridReady = true;
            }

            // Подписываемся на каналы ввода
            InputSystemChannels.Left
                .Subscribe(_ => AddCommand(MoveDirection.Left))
                .AddTo(this);

            InputSystemChannels.Up
                .Subscribe(_ => AddCommand(MoveDirection.Up))
                .AddTo(this);

            InputSystemChannels.Right
                .Subscribe(_ => AddCommand(MoveDirection.Right))
                .AddTo(this);

            InputSystemChannels.Down
                .Subscribe(_ => AddCommand(MoveDirection.Down))
                .AddTo(this);
        }

        private void Update() {
            // Обрабатываем очередь команд, если не двигаемся и grid готов
            if (!isMoving && isGridReady && commandQueue.Count > 0) {
                ProcessNextCommand();
            }
        }

        #region Public API

        public void ClearCommands() {
            commandQueue.Clear();
        }

        public int GetQueuedCommandsCount() {
            return commandQueue.Count;
        }

        #endregion

        #region Private Methods

        private void AddCommand(MoveDirection direction) {
            // Проверяем, может ли новая команда отменить последнюю в очереди
            if (commandQueue.Count > 0) {
                var lastCommand = commandQueue.ToArray()[commandQueue.Count - 1];
                if (oppositeDirections[direction] == lastCommand) {
                    // Убираем последнюю команду из очереди
                    var tempQueue = new Queue<MoveDirection>();
                    var commands = commandQueue.ToArray();

                    for (int i = 0; i < commands.Length - 1; i++) {
                        tempQueue.Enqueue(commands[i]);
                    }

                    commandQueue = tempQueue;
                    return;
                }
            }

            // Добавляем команду в очередь
            commandQueue.Enqueue(direction);
        }

        private void ProcessNextCommand() {
            if (commandQueue.Count == 0)
                return;

            var direction = commandQueue.Dequeue();
            AttemptMove(direction);
        }

        private void AttemptMove(MoveDirection direction) {
            Vector2 targetPosition = GetTargetPosition(direction);

            if (IsCellPassable(targetPosition)) {
                StartMove(targetPosition);
            }
            // Если клетка непроходима, команда просто игнорируется
        }

        private Vector2 GetTargetPosition(MoveDirection direction) {
            Vector2 currentPos = transform.position;
            Vector2 directionVector = directionVectors[direction];

            return currentPos + directionVector * cellSize;
        }

        private bool IsCellPassable(Vector2 targetPosition) {
            var tilemap = GridChannels.LevelTilemap.Value;
            if (tilemap == null) {
                Debug.LogWarning("LevelTilemap не инициализирован");
                return false;
            }

            var blockingTiles = GridChannels.BlockingTiles.Value;
            if (blockingTiles == null || blockingTiles.Length == 0) {
                // Если список блокирующих тайлов пуст, считаем все клетки проходимыми
                return true;
            }

            // Преобразуем мировые координаты в координаты клетки
            Vector3Int cellPosition = tilemap.WorldToCell(targetPosition);

            // Получаем тайл в данной позиции
            TileBase tileAtPosition = tilemap.GetTile(cellPosition);

            // Если клетка пустая (нет тайла), считаем проходимой
            if (tileAtPosition == null)
                return true;

            // Проверяем, есть ли тайл в списке блокирующих
            foreach (var blockingTile in blockingTiles) {
                if (tileAtPosition == blockingTile)
                    return false;
            }

            return true;
        }

        private void StartMove(Vector2 targetPosition) {
            isMoving = true;

            transform.DOMove(targetPosition, stepDuration)
                .SetEase(moveEase)
                .OnComplete(() => {
                    isMoving = false;
                    SnapToGrid(); // Гарантируем точное позиционирование
                });
        }

        private void SnapToGrid() {
            var tilemap = GridChannels.LevelTilemap.Value;
            if (tilemap != null) {
                // Используем размер ячейки из tilemap
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);
                transform.position = cellCenterWorld;
            }
            else {
                // Fallback к старому методу, если tilemap недоступен
                Vector2 currentPos = transform.position;
                Vector2 snappedPos = new Vector2(
                    Mathf.Round(currentPos.x / cellSize) * cellSize,
                    Mathf.Round(currentPos.y / cellSize) * cellSize
                );
                transform.position = snappedPos;
            }
        }

        #endregion

    }

}