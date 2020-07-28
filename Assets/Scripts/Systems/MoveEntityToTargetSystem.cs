using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace FastCube.Systems
{
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    [UpdateBefore(typeof(EndFramePhysicsSystem))]
    public class MoveEntityToTargetSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _commandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _commandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var ecb = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            var scoreComponentEntities = GetComponentDataFromEntity<Score>();

            Entities
                .ForEach(
                    (int entityInQueryIndex, Entity entity, ref Translation translation,
                        ref MoveToTarget moveToTarget, ref PhysicsVelocity physicsVelocity,
                        in MoveDuration moveDuration) =>
                    {
                        if (moveToTarget.TimeElapsed <= moveDuration.Value)
                        {
                            moveToTarget.TimeElapsed += deltaTime;
                            translation.Value = math.lerp(moveToTarget.OriginalPosition,
                                moveToTarget.TargetPosition,
                                moveToTarget.TimeElapsed / moveDuration.Value);
                        }
                        else
                        {
                            translation.Value = moveToTarget.TargetPosition;
                            physicsVelocity.Linear = float3.zero;
                            physicsVelocity.Angular = float3.zero;
                            ecb.RemoveComponent<MoveToTarget>(entityInQueryIndex, entity);

                            if (scoreComponentEntities.HasComponent(entity))
                            {
                                var score = scoreComponentEntities[entity];
                                score.Value++;
                                scoreComponentEntities[entity] = score;
                            }
                        }
                    }).Schedule();

            _commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
