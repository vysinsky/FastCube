using FastCube.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace FastCube.Systems
{
    public class DecreaseLifetimeSystem: SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Lifetime lifetime, in VisitedByPlayer visitedByPlayer) =>
            {
                if (!visitedByPlayer.Value)
                    return;

                lifetime.Value = math.max(lifetime.Value - deltaTime, 0f);
            }).ScheduleParallel();
        }
    }
}
