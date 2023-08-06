using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine
{
    public static class KeyTime
    {
        //Фейк
        private static float _deltaTime;
        /// <summary>
        /// Ну значение лучше не устанавливать..
        /// </summary>
        public static float deltaTime
        {
            get
            {
                return _deltaTime;
            }
            set
            {
                _deltaTime = value;
            }
        }
    }
}
