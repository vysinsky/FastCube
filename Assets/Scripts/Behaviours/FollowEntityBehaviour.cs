using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace.Behaviours
{
    public class FollowEntityBehaviour : MonoBehaviour
    {
        public Entity EntityToFollow;
        public float3 offset = float3.zero;

        private EntityManager _entityManager;


        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            try
            {
                var entityPosition = _entityManager.GetComponentData<Translation>(EntityToFollow);
                transform.position = entityPosition.Value + offset;
            }
            catch (ArgumentException)
            {
                // Entity does not exist (e.g. when reloading scene)
            }

        }
    }

}
