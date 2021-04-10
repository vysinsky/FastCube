using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace FastCube.Systems
{
    public class TouchInputSystem: SystemBase
    {
        private Touch _swipeStartTouch;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
#if  UNITY_EDITOR
            TouchSimulation.Enable();
#endif
            EnhancedTouchSupport.Enable();
        }

        protected override void OnUpdate()
        {
            var (horizontalAxis, verticalAxis) = HandleTouchInput();

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

        private (int, int) HandleTouchInput()
        {
            var verticalAxis = 0;
            var horizontalAxis = 0;

            Debug.Log($"Active fingers: {Touch.activeFingers.Count}");

            if (Touch.activeFingers.Count != 1) return (horizontalAxis, verticalAxis);

            var activeTouch = Touch.activeFingers[0].currentTouch;
            Debug.Log($"Phase: {activeTouch.phase}");

            if (activeTouch.phase == TouchPhase.Began)
            {
                _swipeStartTouch = activeTouch;
            }

            if (activeTouch.phase != TouchPhase.Ended) return (horizontalAxis, verticalAxis);

            var swipeEndTouch = activeTouch;

            var swipeVector = _swipeStartTouch.startScreenPosition -
                              swipeEndTouch.screenPosition;

            Debug.Log($"Swipe vector: {swipeVector}");

            if (swipeVector.x < 0) // Right
            {
                if (swipeVector.y < 0) // Up (move forward)
                {
                    verticalAxis = 1;
                }

                if (swipeVector.y > 0) // Down (move right)
                {
                    horizontalAxis = 1;
                }
            }

            if (swipeVector.x > 0) // Left
            {
                if (swipeVector.y < 0) // Up (move left)
                {
                    horizontalAxis = -1;
                }

                if (swipeVector.y > 0) // Down (move backwards)
                {
                    verticalAxis = -1;
                }
            }

            return (horizontalAxis, verticalAxis);
        }
    }
}
