using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace KeyEngine.Rendering
{
    public class Camera
    {
        public static Camera? Main { get; private set; } = new Camera();

        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    viewIsDirty = true;
                    viewProjectionIsDirty = true;
                }
            }
        }
        private Vector2 _position;

        public float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    viewIsDirty = true;
                    viewProjectionIsDirty = true;
                }
            }
        }
        private float _rotation;

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;
                    projectionIsDirty = true;
                    viewProjectionIsDirty = true;
                }
            }
        }
        private float _zoom;

        public Matrix4 Projection
        {
            get
            {
                if (projectionIsDirty)
                    RefreshProjection();

                return _projection;
            }
        }
        private Matrix4 _projection;

        public Matrix4 View
        {
            get
            {
                if (viewIsDirty)
                    RefreshView();

                return _view;
            }
        }
        private Matrix4 _view;

        public Matrix4 ViewProjection
        {
            get
            {
                if (viewProjectionIsDirty)
                    RefreshProjectionView();

                return _viewProjection;
            }
        }
        private Matrix4 _viewProjection;

        public Matrix4 InvertedViewProjection
        {
            get
            {
                if (viewProjectionIsDirty)
                    RefreshProjectionView();

                return _invertedViewProjection;
            }
        }
        private Matrix4 _invertedViewProjection;

        private bool projectionIsDirty;
        private bool viewIsDirty;
        private bool viewProjectionIsDirty;

        public Camera()
        {
            Position = Vector2.Zero;
            Zoom = 20;
            RefreshProjection();
            RefreshView();
            RefreshProjectionView();

            if (Main == null)
                SetMainCamera(this);
        }

        private void RefreshProjection()
        {
            float x = (float)MainWindow.Instance.ClientSize.X;
            float y = (float)MainWindow.Instance.ClientSize.Y;

            float aspectRatio = x / y;

            float orthoLeft = -_zoom * aspectRatio * 0.5f;
            float orthoRight = _zoom * aspectRatio * 0.5f;
            float orthoBottom = -_zoom * 0.5f;
            float orthoTop = _zoom * 0.5f;

            _projection = Matrix4.CreateOrthographicOffCenter(orthoLeft, orthoRight, orthoBottom, orthoTop, -1, 1);

            projectionIsDirty = false;
        }

        private void RefreshView()
        {
            Matrix4 transform = Matrix4.Identity;

            transform *= Matrix4.CreateTranslation(-_position.X, -_position.Y, 0);
            transform *= Matrix4.CreateRotationZ(-_rotation * Mathf.Deg2Rad);

            _view = transform;

            viewIsDirty = false;
        }

        private void RefreshProjectionView()
        {
            _viewProjection = View * Projection;
            _invertedViewProjection = _viewProjection.Inverted();

            viewProjectionIsDirty = false;
        }

        public static void SetMainCamera(Camera? camera)
        {
            // Unsub old camera
            if (Main != null)
                MainWindow.Instance.Resize -= Main.WindowResized;

            // Sub new camera
            if (camera != null)
                MainWindow.Instance.Resize += camera.WindowResized;
            Main = camera;
        }

        private void WindowResized(ResizeEventArgs args)
        {
            projectionIsDirty = true;
            viewProjectionIsDirty = true;
        }

        /// <summary>
        /// Converts screen coordinates to world coordinates
        /// </summary>
        /// <param name="screenPosition">Screen coordinates</param>
        public Vector2 ScreenToWorldCoords(Vector2 screenPosition)
        {
            float x = (screenPosition.X / (float)MainWindow.Instance.ClientSize.X) * 2 - 1;
            float y = (screenPosition.Y / (float)MainWindow.Instance.ClientSize.Y) * 2 - 1;

            Vector4 temp = new Vector4(x, -y, 10, 1);

            temp *= InvertedViewProjection;

            return new Vector2(temp.X, temp.Y);
        }

        /// <summary>
        /// Converts screen coordinates to world coordinates
        /// </summary>
        /// <param name="screenX">Screen X coordinate</param>
        /// <param name="screenY">Screen Y coordinate</param>
        public Vector2 ScreenToWorldCoords(float screenX, float screenY)
        {
            return ScreenToWorldCoords(new Vector2(screenX, screenY));
        }
    }
}
