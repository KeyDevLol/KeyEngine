using KeyEngine.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Vector2 = KeyEngine.Mathematics.Vector2;

namespace KeyEngine.Rendering.Gizmos
{
    public static class GizmosRendering
    {
        public static Color01 DefaultGizmosColor { get; set; } = new Color01(1, 0.322f, 0.561f);
        private readonly static Shader gizmosMainShader = new Shader("Assets/Shaders/Gizmos.vert", "Assets/Shaders/Gizmos.frag");

        private static float[] squareVertexData =
        [
            0.5f,  0.5f,
            0.5f, -0.5f,
           -0.5f, -0.5f,
           -0.5f,  0.5f,
        ];
        private readonly static VertexAttributeObject squareVAO;
        private readonly static VertexBufferObject squareVBO;


        private static float[] circleVertexData =
        [
           0.5000f,  0.0000f,
           0.4619f,  0.1913f,
           0.3536f,  0.3536f,
           0.1913f,  0.4619f,
           0.0000f,  0.5000f,
          -0.1913f,  0.4619f,
          -0.3536f,  0.3536f,
          -0.4619f,  0.1913f,
          -0.5000f,  0.0000f,
          -0.4619f, -0.1913f,
          -0.3536f, -0.3536f,
          -0.1913f, -0.4619f,
          -0.0000f, -0.5000f,
           0.1913f, -0.4619f,
           0.3536f, -0.3536f,
           0.4619f, -0.1913f,
        ];
        private readonly static VertexAttributeObject circleVAO;
        private readonly static VertexBufferObject circleVBO;

        static GizmosRendering()
        {
            #region Square

            squareVAO = new VertexAttributeObject();
            squareVBO = new VertexBufferObject();

            squareVBO.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, squareVertexData.Length * sizeof(float), squareVertexData, BufferUsageHint.DynamicDraw);

            squareVAO.Bind();

            // Vertex
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            squareVAO.Unbind();
            squareVBO.Unbind();

            #endregion Square

            #region Circle

            circleVAO = new VertexAttributeObject();
            circleVBO = new VertexBufferObject();

            circleVBO.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, circleVertexData.Length * sizeof(float), circleVertexData, BufferUsageHint.DynamicDraw);

            circleVAO.Bind();

            // Vertex
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            circleVAO.Unbind();
            circleVBO.Unbind();

            #endregion Circle
        }

        public static void DrawSquare(Vector2 position, Vector2 size, bool filled = false)
        {
            DrawSquare(position, size, 0, DefaultGizmosColor, filled);
        }

        public static void DrawSquare(Vector2 position, Vector2 size, float rotation, bool filled = false)
        {
            DrawSquare(position, size, rotation, DefaultGizmosColor, filled);
        }

        public static void DrawSquare(Vector2 position, Vector2 size, Color01 color, bool filled = false)
        {
            DrawSquare(position, size, 0, color, filled);
        }

        public static void DrawSquare(Vector2 position, Vector2 size, float rotation, Color01 color, bool filled = false)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Shader.Bind(gizmosMainShader);

            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateScale(size.X, size.Y, 0);
            transform *= Matrix4.CreateRotationZ(rotation);
            transform *= Matrix4.CreateTranslation(position.X, position.Y, 1);

            gizmosMainShader.SetMatrix4("u_ViewProjection", false, transform * Camera.Main!.ViewProjection);
            gizmosMainShader.SetVector4("u_Color", new Vector4(color.R, color.G, color.B, color.A));

            squareVAO.Bind();

            GL.DrawArrays(filled ? PrimitiveType.TriangleFan : PrimitiveType.LineLoop, 0, 4);

            squareVAO.Unbind();
            Shader.Unbind();
            GL.Disable(EnableCap.Blend);
        }

        public static void DrawCircle(Vector2 position, Vector2 size)
        {
            DrawCircle(position, size, 0, DefaultGizmosColor);
        }

        public static void DrawCircle(Vector2 position, Vector2 size, float rotation)
        {
            DrawCircle(position, size, rotation, DefaultGizmosColor);
        }

        public static void DrawCircle(Vector2 position, Vector2 size, Color01 color)
        {
            DrawCircle(position, size, 0, color);
        }

        public static void DrawCircle(Vector2 position, Vector2 size, float rotation, Color01 color)
        {
            Shader.Bind(gizmosMainShader);

            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateScale(size.X, size.Y, 0);
            transform *= Matrix4.CreateTranslation(position.X, position.Y, 1);

            gizmosMainShader.SetMatrix4("u_ViewProjection", false, transform * Camera.Main!.ViewProjection);
            gizmosMainShader.SetVector4("u_Color", new OpenTK.Mathematics.Vector4(color.R, color.G, color.B, color.A));

            circleVAO.Bind();

            GL.DrawArrays(PrimitiveType.LineLoop, 0, 16);

            circleVAO.Unbind();
            Shader.Unbind();
        }
    }
}
