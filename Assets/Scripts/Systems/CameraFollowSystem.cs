using System;
using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FastCube.Systems
{
    public class CameraFollowSystem: SystemBase
    {
        private float3 _offset = float3.zero;

        protected override void OnUpdate()
        {
            try
            {
                var playerEntity = GetSingletonEntity<PlayerTag>();
                var cameraEntity = GetSingletonEntity<CameraTag>();

                if (playerEntity == Entity.Null || cameraEntity == Entity.Null) return;

                if (_offset.Equals(float3.zero))
                {
                    var playerTranslation =
                        EntityManager.GetComponentData<Translation>(playerEntity);
                    var cameraTranslation =
                        EntityManager.GetComponentData<Translation>(cameraEntity);

                    _offset = playerTranslation.Value - cameraTranslation.Value;
                }

                EntityManager.SetComponentData(cameraEntity, new Translation
                {
                    Value = EntityManager.GetComponentData<Translation>(playerEntity).Value + _offset
                });
            }
            catch (InvalidOperationException e)
            {
                // No player or camera yet
            }
        }
    }
}
