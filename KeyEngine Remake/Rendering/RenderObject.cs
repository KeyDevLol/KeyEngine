using KeyEngine.Graphics;
using KeyEngine.Renderer;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace KeyEngine.Rendering
{
    public class RenderObject : Transformable
    {
        public Texture? Texture = Texture.Square;
        public Shader? Shader = Shader.Default;

        public Color Color;

        protected readonly VertexAttributeObject vao;
        protected readonly VertexBufferObject vbo;
        protected readonly ElementBufferObject ebo;

        protected float[] vertexData =
        [
             //  __________
             // |          |
             // |          |
             // |          |
             // |          |
             // |__________|
             //
             // Pos         // Color        // UV
             0.5f,  0.5f,   1, 1, 1, 1,     1, 1, // Up right
             0.5f, -0.5f,   1, 1, 1, 1,     1, 0, // Up left
            -0.5f, -0.5f,   1, 1, 1, 1,     0, 0, // Down left
            -0.5f,  0.5f,   1, 1, 1, 1,     0, 1, // Down right
        ];

        protected sbyte[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        public RenderObject()
        {
            vao = new VertexAttributeObject();
            vbo = new VertexBufferObject();
            ebo = new ElementBufferObject();

            vbo.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.DynamicDraw);

            vao.Bind();

            // Vertex
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            // Color
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), 2 * sizeof(float));

            // UV
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            vbo.Unbind();
            vao.Unbind();

            ebo.Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(sbyte), indices, BufferUsageHint.StaticDraw);
            ebo.Unbind();
        }

        public void Render()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // ToDo: Bind Texture
            Texture?.Bind();

            if (Shader == null)
                return;
            else
                Shader.Bind(Shader);

            // Mark: Может быть сделать проверку на нулл камеры, фиг знает :/
            Shader.SetMatrix4("u_ViewProjection", false, Model * Camera.Main!.ViewProjection);

            vao.Bind();
            ebo.Bind();

            // Draw
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedByte, 0);

            GL.Disable(EnableCap.Blend);
            vao.Unbind();
            ebo.Unbind();
            Shader.Unbind();
        }
    }
}
