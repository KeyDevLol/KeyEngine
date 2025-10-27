using KeyEngine.Graphics;
using KeyEngine.Mathematics;
using KeyEngine.Rendering;
using OpenTK.Graphics.OpenGL;

namespace KeyEngine
{
    public class SpriteRenderer : Component
    {
        public Texture? Texture
        {
            get
            {
                return assetTexture.Value;
            }
            set
            {
                assetTexture.Value = value;
            }
        }
        private readonly AssetReference<Texture> assetTexture = new AssetReference<Texture>(Texture.Square);

        public Shader? Shader = Shader.Default;

        public Color32 Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    ColorChanged();
                }
            }
        }
        private Color32 _color = Color32.White;

        public Vector2 TileSize
        {
            get { return _tileSize; }
            set { _tileSize = value; TileSizeChanged(); }
        }

        protected Vector2 _tileSize = Vector2.One;

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
        protected Mesh<sbyte> mesh;

        protected sbyte[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        public SpriteRenderer(Entity owner) : base(owner)
        {
            mesh = new Mesh<sbyte>();
            Color01 color = new Color01(1f, 1f, 1f, 1f);
            mesh.AddVertex(new Vector2  (0.5f, 0.5f), color, new Vector2(1, 1));
            mesh.AddVertex(new Vector2  (0.5f, -0.5f), color, new Vector2(1, 0));
            mesh.AddVertex(new Vector2 (-0.5f, -0.5f), color, new Vector2(0, 0));
            mesh.AddVertex(new Vector2  (-0.5f,  0.5f), color, new Vector2(0, 1));

            int index = 0;

            foreach (float f in mesh.VertexesArray)
            {
                if (index == 8)
                {
                    index = 0;
                }

                //Console.WriteLine(f);
                index++;
            }

            vao = new VertexAttributeObject();
            vbo = new VertexBufferObject();
            ebo = new ElementBufferObject();
            //AssetReference<Texture> assetReference = new AssetReference<Texture>();
            InitGlObjects();
        }

        public override void Render()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            if (Shader == null)
            {
                return;
            }
            else
            {
                Texture?.Bind();
                Shader.Bind(Shader);
            }

            Shader.SetMatrix4("u_ViewProjection", false, Owner.Model * Camera.Main!.ViewProjection);

            vao.Bind();
            ebo.Bind();

            // Draw
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedByte, 0);

            GL.Disable(EnableCap.Blend);
            vao.Unbind();
            ebo.Unbind();
            Shader.Unbind();
        }

        public override void OnDeleted()
        {
            vao.Dispose();
            vbo.Dispose();
            ebo.Dispose();
        }

        private void InitGlObjects()
        {
            vbo.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.VertexesArray.Length * sizeof(float), mesh.VertexesArray, BufferUsageHint.DynamicDraw);

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

        private void ColorChanged()
        {
            Color01 converted = _color.AsColor01();
            float r = converted.R,
                  g = converted.G,
                  b = converted.B,
                  a = converted.A;

            for (int i = 0; i < 4; i++)
            {
                vertexData[2 + i * 8] = r;
                vertexData[3 + i * 8] = g;
                vertexData[4 + i * 8] = b;
                vertexData[5 + i * 8] = a;
            }

            vbo.Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertexData.Length * sizeof(float), vertexData);
            vbo.Unbind();
        }

        private void TileSizeChanged()
        {
            vertexData[6] = _tileSize.X;
            vertexData[7] = _tileSize.Y;
            vertexData[14] = _tileSize.X;
            vertexData[31] = _tileSize.Y;

            vbo.Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertexData.Length * sizeof(float), vertexData);
            vbo.Unbind();
        }
    }
}