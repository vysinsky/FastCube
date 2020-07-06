using System;
using FastCube.Components;
using FastCube.LevelGenerationUtils;
using Unity.Entities;
using Unity.Transforms;

namespace FastCube.Systems
{
    public class GenerateTileSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            try
            {
                var levelEntity = GetSingletonEntity<LevelGenerationData>();
                var levelData = EntityManager.GetComponentData<LevelGenerationData>(levelEntity);

                var lastPosition = levelData.LastPosition;
                var newTilesCount = 0;
                for (var i = levelData.CurrentTilesCount; i <= levelData.MinimumTilesCount; i++)
                {
                    lastPosition = Generator.PickNextPosition(levelData.StepSize, lastPosition);
                    newTilesCount++;

                    var newGroundEntity = EntityManager.Instantiate(levelData.GroundEntityPrefab);
                    EntityManager.SetComponentData(newGroundEntity, new Translation
                    {
                        Value = lastPosition
                    });
                }

                levelData.CurrentTilesCount += newTilesCount;
                levelData.LastPosition = lastPosition;

                EntityManager.SetComponentData(levelEntity, levelData);
            }
            catch (InvalidOperationException e)
            {
            }
        }
    }
}
