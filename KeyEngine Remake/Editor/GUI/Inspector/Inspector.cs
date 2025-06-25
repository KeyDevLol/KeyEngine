using ImGuiNET;
using System.Reflection;
using KeyEngine.Editor.SupportedTypes;
using NUMVector2 = System.Numerics.Vector2;
using System.Diagnostics;

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

                    if (ImGui.CollapsingHeader(cachedComponent.ComponentType.Name))
                    {
                        foreach (VariableInfo variable in cachedComponent.Variables)
                        {
                            MemberInfo member = variable.MemberInfo;
                            bool disabled = false;

                            if (variable.IsReadonly)
                            {
                                ImGui.BeginDisabled();
                                disabled = true;
                            }

                            if (Supported.TryGetTypeSupport(variable.Type, out TypeSupport? typeSupport))
                            {
                                object? startValue = variable.GetValue(cachedComponent.Component);
                                object value = typeSupport!.Render(new TypeSupportRenderArgs(GetVariableName(variable),
                                    cachedComponent.Component, startValue, variable.MemberInfo));

                                if (!value.Equals(startValue))
                                {
                                    Log.Print("Changed");
                                    variable.SetValue(cachedComponent.Component, value);
                                }
                            }

                            if (disabled)
                                ImGui.EndDisabled();
                        }

                        ImGui.Separator();
                        ImGui.Dummy(new NUMVector2(0, 25));
                    }
                }
            }
        }

        private static string GetVariableName(VariableInfo variableInfo)
        {
            return $"{variableInfo.MemberInfo.Name}##{variableInfo.ReflectedType?.Name}";
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
