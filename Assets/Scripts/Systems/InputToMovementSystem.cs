using System;
using System.Linq;
using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FastCube.Systems
{
    public class InputToMovementSystem: SystemBase
    {
        private const float MoveDistance = 1.2f;

        private EndSimulationEntityCommandBufferSystem _commandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _commandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.WithNone<MoveToTarget>().WithAll<PlayerTag>().ForEach((int entityInQueryIndex,
                Entity entity, ref Translation translation, ref Grounded grounded, ref DynamicBuffer<InputBuffer> inputBuffer) =>
            {
                if (!grounded.Value) return;
                if (inputBuffer.Length <= 0) return;

                var input = inputBuffer[0].Value;

                if (math.abs(input.x) > .1f)
                    ecb.AddComponent(entityInQueryIndex, entity,
                        CreateMoveToTargetComponent(MovementAxis.Horizontal,
                            translation.Value, input.x));

                if (math.abs(input.y) > .1f)
                    ecb.AddComponent(entityInQueryIndex, entity,
                        CreateMoveToTargetComponent(MovementAxis.Vertical, translation.Value,
                            input.y));

                inputBuffer.RemoveAt(0);
            }).Schedule();

            _commandBufferSystem.AddJobHandleForProducer(Dependency);
        }


        private static MoveToTarget CreateMoveToTargetComponent(MovementAxis axis, float3 position,
            float inputValue)
        {
            var targetPosition = axis switch
            {
                MovementAxis.Horizontal => position +
                                           new float3(
                                               inputValue > 0f ? MoveDistance : -MoveDistance,
                                               float2.zero),
                MovementAxis.Vertical => position + new float3(float2.zero,
                    inputValue > 0f ? MoveDistance : -MoveDistance),

                _ => throw new Exception("Invalid axis")
            };

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
    }
}
