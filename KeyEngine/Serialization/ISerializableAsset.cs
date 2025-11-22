using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Serialization
{
    public interface ISerializableAsset
    {
        public SerializeData SceneSerialize();
        public void SceneDeserialize(SerializeData serializeData);
    }
}
