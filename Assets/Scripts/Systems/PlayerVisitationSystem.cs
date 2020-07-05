using FastCube.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace FastCube.Systems
{
    public class PlayerVisitationSystem : JobComponentSystem
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepsPhysicsWorld;

        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepsPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var triggerJob = new TriggerJob
            {
                VisitedByPlayerEntities = GetComponentDataFromEntity<VisitedByPlayer>()
            };

            return triggerJob.Schedule(_stepsPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        }

        private struct TriggerJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<VisitedByPlayer> VisitedByPlayerEntities;

            public void Execute(TriggerEvent triggerEvent)
            {
                var targetEntity = Entity.Null;
                if (VisitedByPlayerEntities.HasComponent(triggerEvent.EntityA))
                    targetEntity = triggerEvent.EntityA;
                if (VisitedByPlayerEntities.HasComponent(triggerEvent.EntityB))
                    targetEntity = triggerEvent.EntityB;

                // Tile was already destroyed
                if (!VisitedByPlayerEntities.Exists(targetEntity))
                    return;

                var visitedByPlayerEntity = VisitedByPlayerEntities[targetEntity];

                if (visitedByPlayerEntity.Value)
                    return;

                visitedByPlayerEntity.Value = true;
                VisitedByPlayerEntities[targetEntity] = visitedByPlayerEntity;
            }
        }
    }
}
