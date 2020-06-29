using Unity.Entities;

namespace FastCube.Components
{
    [GenerateAuthoringComponent]
    public struct Grounded: IComponentData
    {
        public bool Value;
    }
}
