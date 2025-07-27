using System;
using System.Collections.Generic;
using System.Linq;
using Common.Input_System;
using DG.Tweening;
using Game.Labyrinth;
using Scriptable_Objects;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Конфигурация;

namespace Game.Player {

    public class PlayerContinuousMoveController : MonoBehaviour {

        //@formatter:off
        [Header("Общие настройки игрока")]
        [SerializeField] private PlayerSettings playerSettings;
            
        [Header("Movement Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Ease moveEase = Ease.Linear;
        
        [Header("Grid Settings")]
        [SerializeField] private bool waitForGridManager = true;
        //@formatter:on

        private float stepDuration = 0.3f;
        private bool isMoving = false;
        private bool isGridReady = false;
        private Vector2 currentInputDirection = Vector2.zero;
        private Tweener currentMoveTween;

        // Словарь для векторов направлений
        private readonly Dictionary<MoveDirection, Vector2> directionVectors = new Dictionary<MoveDirection, Vector2> {
            { MoveDirection.Left, Vector2.left },
            { MoveDirection.Right, Vector2.right },
            { MoveDirection.Up, Vector2.up },
            { MoveDirection.Down, Vector2.down }
        };

        private HashSet<TileBase> blockingTiles = new();

        private void Awake() {
            ConfigChannels.LabyrinthSettings
                .Where(it => it != null)
                .Subscribe(settings => {
                    blockingTiles = settings.tiles
                        .Where(it => it.blocking)
                        .Select(it => it.tileBase)
                        .ToHashSet();
                })
                .AddTo(this);
        }

        private void Start() {
            // Выравниваем позицию на сетке при старте
            SnapToGrid();

            if (playerSettings != null) {
                stepDuration = playerSettings.stepDuration;
            }

            // Подписываемся на готовность tilemap
            if (waitForGridManager) {
                LabyrinthChannels.Geometry
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

            // Настраиваем ввод
            SetupInput();
        }

        private void SetupInput() {
            // Используем ReactiveProperty каналы
            SetupReactiveChannels();
        }

        private void SetupReactiveChannels() {
            // Подписываемся на изменения состояния кнопок
            Observable.CombineLatest(
                InputSystemChannels.LeftPressed,
                InputSystemChannels.RightPressed,
                InputSystemChannels.UpPressed,
                InputSystemChannels.DownPressed,
                (left, right, up, down) => {
                    Vector2 input = Vector2.zero;
                    if (left) input.x -= 1;
                    if (right) input.x += 1;
                    if (down) input.y -= 1;
                    if (up) input.y += 1;
                    return input.normalized;
                })
                .Subscribe(input => {
                    currentInputDirection = input;
                    TryStartMove();
                })
                .AddTo(this);
        }

        private void Update() {
            // Проверяем, нужно ли начать новое движение
            if (!isMoving && isGridReady && currentInputDirection != Vector2.zero) {
                TryStartMove();
            }
        }

        private void TryStartMove() {
            if (isMoving || !isGridReady || currentInputDirection == Vector2.zero) {
                return;
            }

            // Определяем направление движения
            MoveDirection direction = GetMoveDirection(currentInputDirection);
            if (direction == MoveDirection.None) {
                return;
            }

            Vector2 targetPosition = GetTargetPosition(direction);

            if (IsCellPassable(targetPosition)) {
                StartMove(targetPosition);
            }
            else {
                // Если клетка непроходима, останавливаемся в центре текущей
                SnapToGrid();
            }
        }

        private MoveDirection GetMoveDirection(Vector2 input) {
            // Определяем доминирующее направление
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
                return input.x > 0 ? MoveDirection.Right : MoveDirection.Left;
            }
            else if (Mathf.Abs(input.y) > 0.01f) {
                return input.y > 0 ? MoveDirection.Up : MoveDirection.Down;
            }

            return MoveDirection.None;
        }

        private Vector2 GetTargetPosition(MoveDirection direction) {
            if (direction == MoveDirection.None) {
                return transform.position;
            }

            Vector2 currentPos = transform.position;
            Vector2 directionVector = directionVectors[direction];

            return currentPos + directionVector * cellSize;
        }

        private bool IsCellPassable(Vector2 targetPosition) {
            var tilemap = LabyrinthChannels.Geometry.Value;
            if (tilemap == null) {
                Debug.LogWarning("LevelTilemap не инициализирован");
                return false;
            }

            if (blockingTiles.Count == 0) {
                return true;
            }

            Vector3Int cellPosition = tilemap.WorldToCell(targetPosition);
            TileBase tileAtPosition = tilemap.GetTile(cellPosition);

            if (tileAtPosition == null)
                return true;
            
            return !blockingTiles.Contains(tileAtPosition);
        }

        private void StartMove(Vector2 targetPosition) {
            isMoving = true;

            currentMoveTween = transform.DOMove(targetPosition, stepDuration)
                .SetEase(moveEase)
                .OnUpdate(() => {
                    // Проверяем, прошли ли мы центр текущего тайла
                    float progress = currentMoveTween.ElapsedPercentage();
                    if (progress >= 0.5f && currentInputDirection != Vector2.zero) {
                        // После прохождения центра можем планировать следующее движение
                        // Но пока просто ждем завершения текущего
                    }
                })
                .OnComplete(() => {
                    isMoving = false;
                    SnapToGrid();
                    
                    // Сразу пытаемся начать следующее движение, если кнопка всё ещё нажата
                    if (currentInputDirection != Vector2.zero) {
                        TryStartMove();
                    }
                });
        }

        public void SnapToGrid() {
            var tilemap = LabyrinthChannels.Geometry.Value;
            if (tilemap != null) {
                Vector3Int cellPosition = tilemap.WorldToCell(transform.position);
                Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);
                transform.position = cellCenterWorld;
            }
            else {
                Vector2 currentPos = transform.position;
                Vector2 snappedPos = new Vector2(
                    Mathf.Round(currentPos.x / cellSize) * cellSize,
                    Mathf.Round(currentPos.y / cellSize) * cellSize
                );
                transform.position = snappedPos;
            }
        }

        private void OnDestroy() {
            // Останавливаем текущее движение
            currentMoveTween?.Kill();
        }

        private void OnDisable() {
            // Останавливаем движение при отключении компонента
            currentMoveTween?.Kill();
            isMoving = false;
            currentInputDirection = Vector2.zero;
        }

        // Вспомогательное перечисление для направлений
        private enum MoveDirection {
            None,
            Left,
            Right,
            Up,
            Down
        }
    }
}