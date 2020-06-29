using Unity.Entities;
using Unity.Mathematics;

namespace FastCube.Components
{
    [GenerateAuthoringComponent]
    public struct MoveToTarget : IComponentData
    {
        public float3 OriginalPosition;
        public float3 TargetPosition;
        public float TimeElapsed;
    }
}
