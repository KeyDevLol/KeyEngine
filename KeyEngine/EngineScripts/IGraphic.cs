using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine
{
    interface IGraphic
    {
        RectangleShape sharedShape { get; }

        bool isActive { get; }

        int renderLayer { get; }
    }
}
