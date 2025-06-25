using ImGuiNET;
using System.Reflection;
using KeyEngine.Editor.SupportedTypes;
using NUMVector2 = System.Numerics.Vector2;

namespace KeyEngine.Editor.GUI
{
    public class Inspector : EditorWindow
    {
        private static Inspector instance = null!;
        public static Inspector Instance { get => instance; }

        private Entity? currentEntity;
        private CachedComponent[] cachedComponents = [];

        public Inspector()
        {
            if (instance != null)
                return;

            instance = this;
            title = "Inspector";
        }

        public static void RefreshEntity(Entity? entity)
        {
            if (entity == null)
            {
                instance.currentEntity = null;
                instance.cachedComponents = [];
                return;
            }

            if (entity.Equals(instance.currentEntity))
                return;

            instance.currentEntity = entity;

            Component[] components = entity.GetAllComponents().ToArray();
            instance.cachedComponents = new CachedComponent[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                instance.cachedComponents[i] = new CachedComponent(components[i]);
            }
        }

        public override void Render()
        {
            if (currentEntity != null)
            {
                DrawTransform();

                if (cachedComponents.Length == 0)
                    return;

                for (int i = 0; i < cachedComponents.Length; i++)
                {
                    CachedComponent cachedComponent = cachedComponents[i];

                    if (ImGui.CollapsingHeader(cachedComponent.componentType.Name))
                    {
                        for (int j = 0; j < cachedComponent.variables.Length; j++)
                        {
                            MemberInfo variable = cachedComponent.variables[j];

                            if (Supported.TryGetTypeSupport(variable.GetVariableType(), out TypeSupport? typeSupport))
                            {
                                object startValue = variable.GetValue(cachedComponent.component);
                                object value = typeSupport!.Render(new TypeSupportRenderArgs(GetVariableName(variable), 
                                    cachedComponent.component, startValue, variable));

                                if (!value.Equals(startValue))
                                {
                                    Log.Print("Changed");
                                    variable.SetValue(cachedComponent.component, value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetVariableName(MemberInfo memberInfo)
        {
            return $"{memberInfo.Name}##{memberInfo.ReflectedType?.Name}";
        }

        private void DrawTransform()
        {
            ImGui.Text("Transform");

            // Position
            NUMVector2 position = currentEntity!.Position;
            ImGui.DragFloat2("Position##Inspector", ref position, 0.1f);
            currentEntity.Position = position;

            // Scale
            NUMVector2 scale = currentEntity.Scale;
            ImGui.DragFloat2("Scale##Inspector", ref scale, 0.1f);
            currentEntity.Scale = scale;

            // Rotation
            float rotation = currentEntity.Rotation;
            ImGui.DragFloat("Rotation##Inspector", ref rotation, 0.1f);
            currentEntity.Rotation = rotation;

            ImGui.Separator();
        }
    }
}
