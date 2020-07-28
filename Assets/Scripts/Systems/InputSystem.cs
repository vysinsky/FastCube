using System;
using FastCube.Components;
using FastCube.Generated;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace FastCube.Systems
{
    public class InputSystem : SystemBase, InputActions.IGameplayActions
    {
        private const float MoveDistance = 1.2f;

        private EndSimulationEntityCommandBufferSystem _commandBufferSystem;

        private float2 _input = float2.zero;

        private ScreenInfo _screenInfo;

        private InputActions _inputActions;

        public void OnMove(InputAction.CallbackContext context)
        {
            _input = new float2(context.ReadValue<Vector2>());
        }

        public void OnTouchMove(InputAction.CallbackContext context)
        {
            var rawInput = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log($"Screen: {_screenInfo.HalfWidth}; {_screenInfo.HalfHeight}");
            Debug.Log($"Input: {rawInput.x}; {rawInput.y}");
            // Left part of screen, move down or left
            if (rawInput.x < _screenInfo.HalfWidth)
            {
                if (rawInput.y < _screenInfo.HalfHeight)
                {
                    _input = new float2(0f ,-1f);
                }
                else
                {
                   _input = new float2(-1f ,0f);
                }
            }

            // Right part of screen, move up or right
            if (rawInput.x > _screenInfo.HalfWidth)
            {
                if (rawInput.y < _screenInfo.HalfHeight)
                {
                    _input = new float2(1f ,0f);
                }
                else
                {
                    _input = new float2(0f ,1f);
                }
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            _screenInfo = new ScreenInfo(Screen.width, Screen.height);
            _commandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _inputActions = new InputActions();
            _inputActions.Gameplay.SetCallbacks(this);
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            _inputActions.Enable();
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            _inputActions.Disable();
        }

        protected override void OnUpdate()
        {
            var horizontalAxis = _input.x;
            var verticalAxis = _input.y;
            _input = float2.zero;

            var ecb = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities
                .WithNone<MoveToTarget>()
                .ForEach(
                    (int entityInQueryIndex, in Entity entity, in Translation translation,
                        in PlayerTag playerTag,
                        in Grounded grounded) =>
                    {
                        if (!grounded.Value) return;

                        if (math.abs(horizontalAxis) > .1f)
                            ecb.AddComponent(entityInQueryIndex, entity,
                                CreateMoveToTargetComponent(MovementAxis.Horizontal,
                                    translation.Value, horizontalAxis));

                        if (math.abs(verticalAxis) > .1f)
                            ecb.AddComponent(entityInQueryIndex, entity,
                                CreateMoveToTargetComponent(MovementAxis.Vertical,
                                    translation.Value, verticalAxis));
                    }).Schedule();

            _commandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static MoveToTarget CreateMoveToTargetComponent(MovementAxis axis, float3 position,
            float inputValue)
        {
            float3 targetPosition;
            switch (axis)
            {
                case MovementAxis.Horizontal:
                    targetPosition = position +
                                     new float3(inputValue > 0f ? MoveDistance : -MoveDistance,
                                         float2.zero);
                    break;
                case MovementAxis.Vertical:
                    targetPosition = position + new float3(float2.zero,
                        inputValue > 0f ? MoveDistance : -MoveDistance);
                    break;
                default:
                    throw new Exception("Invalid axis");
            }

            return new MoveToTarget
            {
                OriginalPosition = position,
                TargetPosition = targetPosition,
                TimeElapsed = 0f
            };
        }

        private enum MovementAxis
        {
            Vertical,
            Horizontal
        }

        private struct ScreenInfo
        {
            public readonly float Width;
            public readonly float Height;
            public readonly float HalfWidth;
            public readonly float HalfHeight;

            public ScreenInfo(float width, float height)
            {
                Width = width;
                Height = height;
                HalfWidth = width / 2;
                HalfHeight = height / 2;
            }
        }
    }
}
