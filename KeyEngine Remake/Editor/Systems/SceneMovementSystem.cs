using KeyEngine.Mathematics;
using KeyEngine.Rendering;

namespace KeyEngine.Editor.Systems
{
    public class SceneMovementSystem : EditorSystem
    {
        private static bool isDragged;
        private static Vector2 origin;

        private float wasdMoveSpeed = 22;

        public override void Update(float deltaTime)
        {
            UpdateMouseMovement();
        }

        private void UpdateMouseMovement()
        {
            if (Input.IsMouseButtonDown(MouseButtonCode.Right))
            {
                isDragged = true;

                if (Camera.Main != null)
                    origin = Camera.Main.ScreenToWorldCoords(Input.MousePosition);
            }
            else if (Input.IsMouseButtonUp(MouseButtonCode.Right))
            {
                isDragged = false;
            }

            if (EditorGuiSystem.IsMouseOnGUI || Camera.Main == null)
                return;

            if (Input.MouseScrollDelta != Vector2.Zero)
            {
                Camera.Main.Zoom = Mathf.Clamp(Camera.Main.Zoom - Input.MouseScrollDelta.Y * 1.2f, 0.5f, float.MaxValue);
            }

            if (Input.IsKeyDown(KeyCode.LeftAlt) && Input.IsKeyPressed(KeyCode.Q))
            {
                EditorGuiSystem.EnableRenderingGUI = !EditorGuiSystem.EnableRenderingGUI;
            }

            if (Input.IsKeyDown(KeyCode.LeftShift))
            {
                float xAxis = Input.GetAxisRaw(KeyCode.A, KeyCode.D);
                float yAxis = Input.GetAxisRaw(KeyCode.S, KeyCode.W);

                Camera.Main.Position += new Vector2(xAxis, yAxis) * wasdMoveSpeed * MainWindow.DeltaTime;
            }

            if (isDragged)
            {
                Vector2 diff = Camera.Main.ScreenToWorldCoords(Input.MousePosition) - Camera.Main.Position;
                Camera.Main.Position = origin - diff;
            }
        }
    }
}
