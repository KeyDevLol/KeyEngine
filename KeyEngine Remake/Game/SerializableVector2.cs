using KeyEngine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KeyEngine.Game
{
    public class SerializableVector2
    {
        [JsonInclude]
        public float X;
        [JsonInclude]
        public float Y;

        public SerializableVector2()
        {

        }

        public SerializableVector2(Vector2 vec)
        {
            X = vec.X;
            Y = vec.Y;
        }

        public Vector2 ToVec() => new Vector2(X, Y);
    }
}
