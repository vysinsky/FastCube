using System;
using FastCube.Components;
using Unity.Entities;

namespace FastCube.Systems
{
    public class DestroyOnNoLifetimeSystem : SystemBase
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
            try
            {
                var levelEntity = GetSingletonEntity<LevelGenerationData>();
                var levelData = EntityManager.GetComponentData<LevelGenerationData>(levelEntity);

                var cb = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
                Entities.ForEach(
                    (Entity entity, int entityInQueryIndex, in Lifetime lifetime) =>
                    {
                        if (lifetime.Value >= 0.0001f)
                            return;

                        levelData.CurrentTilesCount -= 1;
                        cb.SetComponent(entityInQueryIndex, levelEntity, levelData);

                        cb.DestroyEntity(entityInQueryIndex, entity);
                    }).Schedule();

                _commandBufferSystem.AddJobHandleForProducer(Dependency);
            }
            catch (InvalidOperationException e)
            {
                // No level data yet
            }
        }
    }
}
