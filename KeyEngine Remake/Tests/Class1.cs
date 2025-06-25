using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Tests
{
    //
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ComponentAtt : Attribute
    {
        private Type type;

        public ComponentAtt(Type type)
        {
            Console.WriteLine("ZALUPAAA TYPAYA");
            this.type = type;
        }
    }
}
