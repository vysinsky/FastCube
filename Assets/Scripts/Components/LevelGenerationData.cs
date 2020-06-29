using Unity.Entities;
using Unity.Mathematics;

namespace FastCube.Components
{
    public struct LevelGenerationData: IComponentData
    {
        public float3 LastPosition;
        public Entity GroundEntityPrefab;
        public int CurrentTilesCount;
        public int MinimumTilesCount;
        public float StepSize;
    }
}
