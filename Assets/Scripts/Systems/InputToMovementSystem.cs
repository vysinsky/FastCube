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

                ecb.AddComponent(entityInQueryIndex, entity,
                    CreateMoveToTargetComponent(input, translation.Value));

                inputBuffer.RemoveAt(0);
            }).Schedule();

            _commandBufferSystem.AddJobHandleForProducer(Dependency);
        }


        private static MoveToTarget CreateMoveToTargetComponent(float2 input,
            float3 currentPosition)
        {
            var targetPosition = float3.zero;
            if (math.abs(input.x) > .1f)
            {
                targetPosition = currentPosition +
                                 new float3(input.x > 0f ? MoveDistance : -MoveDistance,
                                     float2.zero);
            }

            if (math.abs(input.y) > .1f)
            {
                targetPosition = currentPosition +
                                 new float3(float2.zero, input.y > 0f ? MoveDistance : -MoveDistance);
            }

            return new MoveToTarget
            {
                OriginalPosition = currentPosition,
                TargetPosition = targetPosition,
                TimeElapsed = 0f
            };
        }
    }
}
