using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine
{
    public static class Input
    {
        public static Dictionary<Keyboard.Key, bool> keyDictionary = new Dictionary<Keyboard.Key, bool>();

        private static Stopwatch timer = new Stopwatch();
        private static bool keyWasPressed;

        public static bool IsKeyDown(Keyboard.Key key)
        {
            if (Keyboard.IsKeyPressed(key) && keyWasPressed == false)
            {
                keyWasPressed = true;

                return true;
            }

            return false;
        }

        public static bool IsKeyUp(Keyboard.Key key)
        {
            if (Keyboard.IsKeyPressed(key) == false && keyWasPressed == true)
            {
                keyWasPressed = false;

                return true;
            }

            return false;
        }

        //public static bool IsKeyUpped()
        //{

        //}

        public static bool IsKeyPressed(Keyboard.Key key)
        {
            return Keyboard.IsKeyPressed(key);
        }

        /// <summary>
        /// Системный метод, не используйте его.
        /// </summary>
        public static void systemKeyUp(Keyboard.Key key)
        {
            keyWasPressed = false;
        }       

        /// <summary>
        /// Системный метод, не используйте его.
        /// </summary>
        public static void systemKeyDown(Keyboard.Key key)
        {

        }


        //Axis

        public static float horizontalAxis;
        public static float verticalAxis;

        public static float LerpTime = 0.005f;

        public static float AxisX = 1f;
        public static float AxisY = 1f;

        public static void UpdateAxis()
        {
            //Vertical
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                verticalAxis = KeyMath.Lerp(verticalAxis, -AxisY, GetDeltaLerpTime());
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                verticalAxis = KeyMath.Lerp(verticalAxis, AxisY, GetDeltaLerpTime());
            }
            else
            {
                verticalAxis = KeyMath.Lerp(verticalAxis, 0, GetDeltaLerpTime());
            }

            //Horizontal
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                horizontalAxis = KeyMath.Lerp(horizontalAxis, -AxisX, GetDeltaLerpTime());
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                horizontalAxis = KeyMath.Lerp(horizontalAxis, AxisX, GetDeltaLerpTime());
            }
            else
            {
                horizontalAxis = KeyMath.Lerp(horizontalAxis, 0, GetDeltaLerpTime());
            }
        }

        private static float GetDeltaLerpTime()
        {
            return LerpTime * KeyTime.deltaTime;
        }
    }
}
