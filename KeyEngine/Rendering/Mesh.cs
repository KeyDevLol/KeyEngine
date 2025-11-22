using KeyEngine.Mathematics;
using System.Numerics;
using Vector2 = KeyEngine.Mathematics.Vector2;

namespace KeyEngine.Rendering
{
    public class Mesh<T> where T: INumber<T>
    {
        private readonly List<Vertex> vertexesList = new List<Vertex>();
        public float[] VertexesArray = [];
        private readonly List<T> indiciesList = new List<T>();
        public T[] Indicies = [];

        public Mesh() { }

        public Mesh(params T[] indicies)
        {
            Indicies = indicies;
        }

        public Mesh(Vector2[] vertexes, Color01[] colors, Vector2[] uvs, T[] indicies)
        {
            if (vertexes.Length != colors.Length)
                throw new ArgumentException("The length of one of the arrays is greater than the others. The lengths must be the same.");

            for (int i = 0; i < vertexes.Length; i++)
            {
                AddVertex(new Vertex(vertexes[i], colors[i], uvs[i]));
            }

            Indicies = indicies;
        }

        public Vertex GetVertex(int index)
        {
            return vertexesList[index];
        }

        public void AddVertex(Vertex vertex)
        {
            vertexesList.Add(vertex);
            AddToArray(vertex);
        }

        public void AddVertex(Vector2 position, Color01 color, Vector2 uv)
        {
            Vertex vertex = new Vertex(position, color, uv);
            vertexesList.Add(vertex);
            AddToArray(vertex);
        }

        public void RemoveVertex(Vertex vertex)
        {
            vertexesList.Remove(vertex);
        }

        public void RemoveVertex(int index)
        {
            vertexesList.RemoveAt(index);
        }

        private void AddToArray(Vertex vertex)
        {
            int arrayLength = VertexesArray.Length;
            Array.Resize(ref VertexesArray, arrayLength + 8);

            VertexesArray[arrayLength] = vertex.Position.X;
            VertexesArray[arrayLength + 1] = vertex.Position.Y;

            VertexesArray[arrayLength + 2] = vertex.Color.R;
            VertexesArray[arrayLength + 3] = vertex.Color.G;
            VertexesArray[arrayLength + 4] = vertex.Color.B;
            VertexesArray[arrayLength + 5] = vertex.Color.A;

            VertexesArray[arrayLength + 6] = vertex.UV.X;
            VertexesArray[arrayLength + 7] = vertex.UV.Y;
        }
    }
}
