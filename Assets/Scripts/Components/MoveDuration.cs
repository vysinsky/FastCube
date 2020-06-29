using Unity.Entities;

namespace FastCube.Components
{
    [GenerateAuthoringComponent]
    public struct MoveDuration : IComponentData
    {
        public float Value;
    }
}
