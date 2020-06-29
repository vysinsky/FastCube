using Unity.Entities;

namespace FastCube.Components
{
    [GenerateAuthoringComponent]
    public struct Lifetime: IComponentData
    {
        public float Value;
    }

}
