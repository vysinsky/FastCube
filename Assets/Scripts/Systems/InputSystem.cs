using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FastCube.Systems
{
    public class InputSystem : SystemBase
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
            var verticalAxis = Input.GetAxis("Vertical");
            var horizontalAxis = Input.GetAxis("Horizontal");

            var ecb = _commandBufferSystem.CreateCommandBuffer().ToConcurrent();
            Entities
                .WithNone<MoveToTarget>()
                .ForEach(
                    (int entityInQueryIndex, in Entity entity, in Translation translation,
                        in PlayerTag playerTag,
                        in Grounded grounded) =>
                    {
                        if (!grounded.Value) return;

                        if (math.abs(horizontalAxis) > .1f)
                            ecb.AddComponent(entityInQueryIndex, entity, new MoveToTarget
                            {
                                OriginalPosition = translation.Value,
                                TargetPosition = translation.Value +
                                                 new float3(
                                                     horizontalAxis > 0f
                                                         ? MoveDistance
                                                         : -MoveDistance, 0f, 0f),
                                TimeElapsed = 0f
                            });

                        if (math.abs(verticalAxis) > .1f)
                            ecb.AddComponent(entityInQueryIndex, entity, new MoveToTarget
                            {
                                OriginalPosition = translation.Value,
                                TargetPosition = translation.Value +
                                                 new float3(0f, 0f,
                                                     verticalAxis > 0f
                                                         ? MoveDistance
                                                         : -MoveDistance),
                                TimeElapsed = 0f
                            });
                    }).Schedule();

            _commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
