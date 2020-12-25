using FastCube.Components;
using FastCube.Generated;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FastCube.Systems
{
    public class InputSystem : SystemBase, InputActions.IGameplayActions
    {
        private float2 _input = float2.zero;

        private InputActions _inputActions;

        public void OnMove(InputAction.CallbackContext context)
        {
            _input = new float2(context.ReadValue<Vector2>());
        }

        protected override void OnCreate()
        {
            base.OnCreate();
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

            Entities.ForEach(
                (ref DynamicBuffer<InputBuffer> inputBuffer) =>
                {
                    if (horizontalAxis == 0 && verticalAxis == 0) return;

                    inputBuffer.Add(new InputBuffer
                    {
                        Value = new float2(horizontalAxis, verticalAxis)
                    });
                }).Schedule();
        }
    }
}
