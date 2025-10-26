using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KeyEngine.Editor.SupportedTypes
{
    internal class ObjectTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            object? value = args.value;

            if (value == null)
            {
                return null;
            }

            Type type = value.GetType();

            if (Supported.TryGetTypeSupport(type, out TypeSupport? result))
            {
                value = result.Render(args);
            }

            return value;
        }
    }
}
