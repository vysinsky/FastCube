using System;
using FastCube.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;

namespace FastCube.Systems
{
    public class RestartSceneSystem: SystemBase
    {
        protected override void OnUpdate()
        {
            try
            {
                var playerEntity = GetSingletonEntity<PlayerTag>();
                var playerTranslation = EntityManager.GetComponentData<Translation>(playerEntity);

                if (playerTranslation.Value.y < -10f)
                {
                    EntityManager.DestroyEntity(EntityManager.UniversalQuery);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            catch (InvalidOperationException e)
            {
                // No player yet
            }
        }
    }
}
