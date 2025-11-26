using KeyEngine.Mathematics;
using System.Runtime.CompilerServices;
using AetherVector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace KeyEngine.Physics.Extensions
{
    public static class PhysicsVector2Extensions
    {
        extension(AetherVector2 aetherVector)
        {
            public Vector2 AsEngineVector()
            {
                return Unsafe.BitCast<AetherVector2, Vector2>(aetherVector);
            }
        }

        extension(Vector2 vector)
        {
            public AetherVector2 AsPhysicsVector()
            {
                return Unsafe.BitCast<Vector2, AetherVector2>(vector);
            }
        }
    }
}
