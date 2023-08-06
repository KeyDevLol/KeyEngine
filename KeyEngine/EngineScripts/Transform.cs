using SFML.System;

namespace KeyEngine
{

    public struct Transform
    {
        public Vector2f position;
        public Vector2f scale;
        public float rotation;

        public Transform(Vector2f pos, float rot, Vector2f scale)
        {
            position = pos;
            rotation = rot;
            this.scale = scale;
        }
    }
}
