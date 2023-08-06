using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.GUI
{
    public class Button : Component
    {
        public EventHandler OnButtonPressed;

        public override void Start()
        {
            MainClass.window.MouseButtonPressed += OnMouseButtonDown;
        }

        public override void OnObjectDestroy()
        {
            MainClass.window.MouseButtonPressed -= OnMouseButtonDown;
        }

        private void OnMouseButtonDown(object sender, EventArgs e)
        {
            Vector2f mousePos = MainClass.window.MapPixelToCoords(Mouse.GetPosition(MainClass.window), MainClass.Camera);

            if (gameObject.shape.GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
            {
                Console.WriteLine("Тык");
                OnButtonPressed?.Invoke(this, new EventArgs());
            }
        }
    }
}
