using FastCube.Components;
using Unity.Entities;
using Unity.Transforms;

namespace FastCube.Systems
{
    public class VisualiseLifetimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (ref LocalToWorld localToWorld, in Lifetime lifeTime) =>
                {
                    var scale = localToWorld.Value.c1;

                    var prevY = scale.y;
                    var newY = lifeTime.Value;

                    localToWorld.Value.c3.y += (prevY - newY) / 2;
                    localToWorld.Value.c1.y = newY;
                }).ScheduleParallel();
        }
    }
}
