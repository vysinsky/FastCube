using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

namespace FastCube.Systems
{
    public class GroundCheckSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var buildPhysicsWorld = EntityManager.World.GetOrCreateSystem<BuildPhysicsWorld>();
            var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            Entities.ForEach(
                (ref Grounded grounded, in Translation translation, in RenderBounds renderBounds) =>
                {
                    grounded.Value = DoGroundCheck(translation, renderBounds, collisionWorld);
                }).Run();
        }

        private static bool DoGroundCheck(Translation translation, RenderBounds renderBounds,
            CollisionWorld collisionWorld)
        {
            var raycastInput = new RaycastInput
            {
                Start = translation.Value + new float3(
                    0f, -renderBounds.Value.Extents.y - .01f, 0f),
                End = translation.Value + new float3(
                    0f, -renderBounds.Value.Extents.y - .03f, 0f),
                Filter = CollisionFilter.Default
            };

            var hit = collisionWorld.CastRay(raycastInput);

            return hit;
        }
    }
}
