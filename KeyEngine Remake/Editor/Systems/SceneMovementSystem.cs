using KeyEngine.Rendering;

namespace KeyEngine.Editor.Systems
{
    public class SceneMovementSystem : EditorSystem
    {
        private static bool isDragged;
        private static Vector2 origin;

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
                    origin = Camera.Main.ScreenToWorldCoords(Input.mousePosition);
            }
            else if (Input.IsMouseButtonUp(MouseButtonCode.Right))
            {
                isDragged = false;
            }

            if (EditorGuiSystem.IsMouseOnGUI == false && Camera.Main != null)
            {
                if (isDragged)
                {
                    Vector2 diff = Camera.Main.ScreenToWorldCoords(Input.mousePosition) - Camera.Main.Position;
                    Camera.Main.Position = origin - diff;
                }
            }
        }
    }
}
