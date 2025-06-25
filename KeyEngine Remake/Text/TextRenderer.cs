using KeyEngine.Rendering;
using OpenTK.Graphics.OpenGL;

namespace KeyEngine
{
    public class TextRenderer : Component
    {
        protected readonly static Shader textShader =  new Shader("Assets/Shaders/Text/Text.vert", "Assets/Shaders/Text/Text.frag");

        public string Text
        {
            get => _text;
            set { _text = value; needRefreshBuffer = true; }
        }
        private string _text = "...";
        public float Scale = 1;
        public float Spacing = 2f;
        public Alignment TextAlignment;

        public Font? Font;

        private readonly VertexAttributeObject vao;
        private readonly VertexBufferObject vbo;
        private readonly VertexBufferObject colorVbo;
        private readonly VertexBufferObject textureVbo;

        private readonly ElementBufferObject ebo;

        public Shader shader;

        private bool needRefreshBuffer;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        protected Color _color = Color.White;

        //private void ColorChanged()
        //{
        //    Color01 color01 = color.ToColor01();

        //    for (int i = 0; i < colors.Length; i += 4)
        //    {
        //        colors[i] = color01.r;
        //        colors[i + 1] = color01.g;
        //        colors[i + 2] = color01.b;
        //        colors[i + 3] = color01.a;
        //    }

        //    ColorVBO.Bind();

        //    GL.BufferSubData(BufferTarget.ArrayBuffer, 0, colors.Length * sizeof(float), colors);

        //    ColorVBO.Unbind();
        //}

        protected uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        protected float[] texCoords =
        {
            //Left - 0, Up = 1
            1, 0,
            0, 0,
            0, 1,
            1, 1
        };

        protected float[] colors =
        {
            1, 1, 1, 1,
            1, 1, 1, 1,
            1, 1, 1, 1,
            1, 1, 1, 1
        };

        public TextRenderer(Entity owner) : base(owner)
        {
            vao = new VertexAttributeObject();
            vbo = new VertexBufferObject();
            colorVbo = new VertexBufferObject();
            textureVbo = new VertexBufferObject();

            shader = textShader;

            vao.Bind();

            // VBO
            vbo.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * 3 * sizeof(float), 0, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            // ColorVBO
            colorVbo.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

            // TextureVBO
            textureVbo.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            textureVbo.Unbind();
            vao.Unbind();

            // EBO
            ebo = new ElementBufferObject();
            ebo.Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            ebo.Unbind();

            needRefreshBuffer = true;
            RefreshBuffer();
        }

        public override void Render()
        {
            if (Font == null || Font.Loaded == false)
                return;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader.Bind();
            shader.SetMatrix4("u_Mvp", false, Owner.Model * Camera.Main!.ViewProjection);

            RefreshBuffer();

            vao.Bind();

            ebo.Bind();
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            ebo.Unbind();

            vao.Unbind();
            Shader.Unbind();
            GL.Disable(EnableCap.Blend);
        }

        private void RefreshBuffer()
        {
            if (!needRefreshBuffer || Font == null)
                return; 

            float x = GetTextWidth();
            float y = 0;

            float[] vertices = new float[12 * Text.Length];
            indices = new uint[6 * Text.Length];
            int count = vertices.Length;
            int index = 0;
            int indexEbo = 0;

            //BIND
            foreach (char c in Text)
            {
                if (c == '\n')
                {
                    y -= Spacing * Scale;
                    x = Spacing;
                    continue;
                }

                if (Font.TryGetGlyph(c, out Glyph glyph) == false)
                {
                    continue;
                }
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, glyph.TextureHandle);

                float xpos = x + glyph.Bearing.X * Scale;
                float ypos = y - (glyph.Size.Y - glyph.Bearing.Y) * Scale;

                float w = glyph.Size.X * Scale;
                float h = glyph.Size.Y * Scale;

                float xSmall = xpos / 100;
                float ySmall = (ypos / 100) + y;

                float wSmall = w / 100;
                float hSmall = h / 100;

                //float[] vertex2 =
                //{
                //    xSmall + wSmall,  ySmall + hSmall,  0,  // Верхний левый угол
                //    xSmall,           ySmall + hSmall,  0,  // Верхний правый угол
                //    xSmall,           ySmall,           0,  // Нижний правый угол
                //    xSmall + wSmall,  ySmall,           0,  // Нижний левый угол
                //};

                vertices[index++] = xSmall + wSmall;
                vertices[index++] = ySmall + hSmall;
                vertices[index++] = 0;

                // Верхний левый угол
                vertices[index++] = xSmall;
                vertices[index++] = ySmall + hSmall;
                vertices[index++] = 0;

                // Нижний левый угол
                vertices[index++] = xSmall;
                vertices[index++] = ySmall;
                vertices[index++] = 0;

                // Нижний правый угол
                vertices[index++] = xSmall + wSmall;
                vertices[index++] = ySmall;
                vertices[index++] = 0;

                indices[indexEbo++] = 0;
                indices[indexEbo++] = 1;
                indices[indexEbo++] = 3;
                indices[indexEbo++] = 1;
                indices[indexEbo++] = 2;
                indices[indexEbo++] = 3;

                x += (glyph.Advance >> 6) * Scale;
            }

            vbo.Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float), vertices);
            vbo.Unbind();

            ebo.Bind();
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, 0, indices.Length * sizeof(uint), indices);
            ebo.Unbind();

            needRefreshBuffer = false;
        }

        private float GetTextWidth()
        {
            if (Font == null)
                return 0;

            float width = 0;

            if (TextAlignment != Alignment.Left)
            {
                foreach (char c in Text)
                {
                    if (Font.TryGetGlyph(c, out Glyph glyph))
                    {
                        width += (glyph.Advance >> 6) * Scale;
                    }
                }
            }

            switch (TextAlignment)
            {
                case Alignment.Center:
                    width = -width / 2f;
                    break;
                case Alignment.Right:
                    width = -width;
                    break;
            }

            return width;
        }

        public override void Deleted()
        {
            vao.Dispose();
            vbo.Dispose();
            colorVbo.Dispose();
            textureVbo.Dispose();
            ebo.Dispose();

            if (Font != null && Font.Loaded)
                Font.Dispose();
        }

        public enum Alignment : byte
        {
            Left,
            Right,
            Center
        }
    }
}
