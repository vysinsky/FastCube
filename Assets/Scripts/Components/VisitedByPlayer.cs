using Unity.Entities;

namespace FastCube.Components
{
    [GenerateAuthoringComponent]
    public struct VisitedByPlayer: IComponentData
    {
        public bool Value;
    }
}
