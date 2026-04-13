using KeyEngine.Assets;
using KeyEngine.Serialization;
using KeyEngine.Editor.SupportedTypes;

namespace KeyEngine.Editor.GUI.AssetEditor
{
    public class AssetEditorWindow : EditorWindow
    {
        public AssetInfo? CurrentAsset;

        public static AssetEditorWindow Singleton { get; set; } = null!;

        public AssetEditorWindow()
        {
            if (Singleton != null)
                throw new InvalidOperationException();

            Singleton = this;

            Title = "Asset Editor";
        }

        public override void Render()
        {
            if (CurrentAsset == null)
                return;

            SerializeData serializeData;

            if (CurrentAsset.Instance != null)
            {
                serializeData = CurrentAsset.Instance!.Serialize();
            }
            else
            {
                serializeData = SerializationManager.DeserializeFile(CurrentAsset.DataPath) ?? throw new NullReferenceException();
            }

            foreach (KeyValuePair<string, YamlVariable> pair in serializeData.DataPairs)
            {
                if (pair.Value.VariableValue == null)
                    continue;

                if (SupportedTypes.SupportedTypes.TryGetTypeSupport(pair.Value.VariableValue.GetType(), out TypeSupport? typeSupport))
                {
                    TypeSupportRenderArgs args = new TypeSupportRenderArgs(
                        pair.Key,
                        pair.Key,
                        null,
                        CurrentAsset.Path,
                        pair.Value.VariableValue);

                    object? value = serializeData.DataPairs[pair.Key].VariableValue;
                    object? newValue = typeSupport.Render(args);

                    if (value != null && !value.Equals(newValue))
                    {
                        serializeData.DataPairs[pair.Key].VariableValue = newValue;
                        SerializationManager.SerializeToFile(CurrentAsset.DataPath, serializeData);
                        CurrentAsset.Instance?.Deserialize(serializeData);
                    }
                }
            }
        }
    }
}
