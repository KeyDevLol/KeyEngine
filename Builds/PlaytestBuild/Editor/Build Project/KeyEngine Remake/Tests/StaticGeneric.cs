using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Tests
{
    public static class Counter
    {
        public static List<string> RegisteredComponents = new List<string>();
    }

    public static class StaticGeneric<T>
    {
        public static void Register() { Counter.RegisteredComponents.Add(typeof(T).Name); }
    }
}
