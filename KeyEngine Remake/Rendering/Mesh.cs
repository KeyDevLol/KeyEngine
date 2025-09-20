using KeyEngine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Rendering
{
    public class Mesh
    {
        public IEnumerable<float> Vertexes => vertexes;
        private List<float> vertexes;

        public void AddVertex(Vector2 position, Color01 color, Vector2 uv)
        {

        }
    }
}
