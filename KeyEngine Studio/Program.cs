using KeyEngine.Core;

namespace KeyEngine_Studio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, KeyEngine Studio!");
            Engine.Run(new EmptyScene());
        }
    }
}
