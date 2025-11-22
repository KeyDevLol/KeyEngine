using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine
{
    public static class ComponentDatabase
    {
        public static ReadOnlyDictionary<int, Type> components;

        public static void Lol()
        {
            components.ContainsKey(111);
        }

        static ComponentDatabase()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            components = new ReadOnlyDictionary<int, Type>(Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(type => type.IsClass).Where(type => type.IsSubclassOf(typeof(Component))).ToDictionary(k => k.GetHashCode(), v => v));
            stopwatch.Stop();

            foreach (var l in components)
            {
                Log.Print($"Name: {l.Value.Name} Hash: {l.Key}");
            }

            Log.Print($"Time: {stopwatch.ElapsedMilliseconds}");
        }
    }
}
