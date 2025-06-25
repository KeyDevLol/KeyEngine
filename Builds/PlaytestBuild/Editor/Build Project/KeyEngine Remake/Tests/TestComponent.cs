using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine
{
    public class TestComponent : Component
    {
        public TestComponent(Entity owner) : base(owner) { }

        public override void Update(float deltaTime)
        {
            Log.Print("Hello!");
        }
    }
}
