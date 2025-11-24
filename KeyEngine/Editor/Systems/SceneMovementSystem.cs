using KeyEngine.Mathematics;
using KeyEngine.Rendering;

namespace KeyEngine.Editor.Systems
{
    public class SceneMovementSystem : EditorSystem
    {
        private bool isDragged;
        private bool isWasdMoving;
        private Vector2 origin;

        private readonly float wasdMoveSpeed = 22;

        public override void Update(float deltaTime)
        {
            if (EditorGuiSystem.IsMouseOnGUI || Camera.Main == null)
            {
                isDragged = false;
                isWasdMoving = false;
                return;
            }

            WasdMove();
            ZoomScroll();
            UpdateMouseMovement();
            HideGUI();
        }

        private void UpdateMouseMovement()
        {
            if (Input.IsMouseButtonPressed(MouseButtonCode.Right))
            {
                isDragged = true;
                origin = Camera.Main!.ScreenToWorldCoords(Input.MousePosition);
            }
            else if (Input.IsMouseButtonReleased(MouseButtonCode.Right))
            {
                isDragged = false;
            }

            if (isDragged && !isWasdMoving)
            {
                Vector2 diff = Camera.Main!.ScreenToWorldCoords(Input.MousePosition) - Camera.Main.Position;
                Camera.Main.Position = origin - diff;
            }
            else if (isDragged && isWasdMoving)
            {
                origin = Camera.Main!.ScreenToWorldCoords(Input.MousePosition);
            }
        }

        private static void HideGUI()
        {
            if (Input.IsKeyDown(KeyCode.LeftAlt) && Input.IsKeyPressed(KeyCode.Q))
            {
                EditorGuiSystem.EnableRenderingGUI = !EditorGuiSystem.EnableRenderingGUI;
            }
        }

        private static void ZoomScroll()
        {
            if (Input.MouseScrollDelta != Vector2.Zero)
            {
                float oldZoom = Camera.Main!.Zoom;
                Vector2 oldMouseWorldPos = Camera.Main.ScreenToWorldCoords(Input.MousePosition);

                float newZoom = Mathf.Clamp(oldZoom - Input.MouseScrollDelta.Y * 1.2f, 0.5f, float.MaxValue);
                Camera.Main.Zoom = newZoom;

                Vector2 newMouseWorldPos = Camera.Main.ScreenToWorldCoords(Input.MousePosition);

                Camera.Main.Position += oldMouseWorldPos - newMouseWorldPos;
            }
        }

        private void WasdMove()
        {
            if (!Input.IsMouseButtonDown(MouseButtonCode.Right))
                return;

            float xAxis = Input.GetAxisRaw(KeyCode.A, KeyCode.D);
            float yAxis = Input.GetAxisRaw(KeyCode.S, KeyCode.W);
            Vector2 moveVector = new Vector2(xAxis, yAxis).Normalized;

            isWasdMoving = moveVector != Vector2.Zero;

            Camera.Main!.Position += moveVector * wasdMoveSpeed * MainWindow.DeltaTime;
        }
    }
}
