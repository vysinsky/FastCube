using Unity.Entities;
using Unity.Mathematics;

namespace FastCube.Components
{
    public struct InputBuffer: IBufferElementData
    {
        public float2 Value;
    }
}
