using System;
using FastCube.Components;
using Unity.Entities;
using Unity.Transforms;

namespace FastCube.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
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

                    if (Math.Abs(prevY - newY) < 0.001f)
                    {
                        return;
                    }

                    localToWorld.Value.c3.y += (prevY - newY) / 2;
                    localToWorld.Value.c1.y = newY;
                }).Schedule();
        }
    }
}
