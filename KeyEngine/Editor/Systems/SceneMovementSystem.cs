using KeyEngine.Mathematics;
using KeyEngine.Rendering;

namespace KeyEngine.Editor.Systems
{
    public class SceneMovementSystem : EditorSystem
    {
        private bool isDragged;
        private Vector2 origin;
        private Vector2 originMouseScreenPosition;
        private bool rightButtonIsDown;

        private bool isWasdMoving;

        private readonly float wasdMoveSpeed = 22;

        public override void Update(float deltaTime)
        {
            if (EditorGuiSystem.IsMouseOnGUI || Camera.Main == null)
                return;

            WasdMove();
            UpdateMouseMovement();
            ZoomScroll();
            HideGUI();
        }

        private void UpdateMouseMovement()
        {
            //static bool lol = true;

            if (Input.IsMouseButtonDown(MouseButtonCode.Right))
            {
                isDragged = true;
                origin = Camera.Main!.ScreenToWorldCoords(Input.MousePosition);
            }
            else if (Input.IsMouseButtonUp(MouseButtonCode.Right))
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
                Camera.Main!.Zoom = Mathf.Clamp(Camera.Main.Zoom - Input.MouseScrollDelta.Y * 1.2f, 0.5f, float.MaxValue);
            }
        }

        private void WasdMove()
        {
            if (!Input.IsMouseButtonHold(MouseButtonCode.Right))
                return;

            float xAxis = Input.GetAxisRaw(KeyCode.A, KeyCode.D);
            float yAxis = Input.GetAxisRaw(KeyCode.S, KeyCode.W);
            Vector2 moveVector = new Vector2(xAxis, yAxis).Normalized;

            isWasdMoving = moveVector != Vector2.Zero;

            Camera.Main!.Position += moveVector * wasdMoveSpeed * MainWindow.DeltaTime;
        }
    }
}
