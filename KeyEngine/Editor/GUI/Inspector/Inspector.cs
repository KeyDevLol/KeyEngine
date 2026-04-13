using ImGuiNET;
using System.Reflection;
using KeyEngine.Editor.SupportedTypes;
using NUMVector2 = System.Numerics.Vector2;

namespace KeyEngine.Editor.GUI.Inspector
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
            Title = "Inspector";
        }

        public static Entity? GetCurrentEntity() => instance.currentEntity;

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

                    NUMVector2 pos = ImGui.GetCursorPos();

                    if (ImGui.CollapsingHeader($"{cachedComponent.ComponentType.Name}##{currentEntity.Id}", ImGuiTreeNodeFlags.AllowOverlap))
                    {
                        bool componentEnabled = cachedComponent.Component.Enabled;
                        DrawEnabled(cachedComponent.ComponentType.Name, ref componentEnabled);
                        cachedComponent.Component.Enabled = componentEnabled;

                        foreach (VariableInfo variable in cachedComponent.Variables)
                        {
                            MemberInfo member = variable.MemberInfo;
                            bool disabled = false;

                            if (variable.IsReadonly)
                            {
                                ImGui.BeginDisabled();
                                disabled = true;
                            }

                            if (SupportedTypes.SupportedTypes.TryGetTypeSupport(variable.Type, out TypeSupport? typeSupport))
                            {
                                object? startValue = variable.GetValue(cachedComponent.Component);
                                TypeSupportRenderArgs args = new TypeSupportRenderArgs
                                (
                                    GetVariableDisplayName(variable, currentEntity),
                                    member.Name,
                                    member.ReflectedType?.Name,
                                    currentEntity.Id.ToString(),
                                    startValue
                                );

                                object? value = typeSupport.Render(args);

                                if (value == null)
                                {
                                    ImGui.Text($"{variable.MemberInfo.Name} is NULL");
                                    continue;
                                }

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
                    else
                    {
                        bool componentEnabled = cachedComponent.Component.Enabled;
                        DrawEnabled(cachedComponent.ComponentType.Name, ref componentEnabled);
                        cachedComponent.Component.Enabled = componentEnabled;
                    }
                }
            }
        }

        private static string GetVariableDisplayName(VariableInfo variableInfo, Entity entity)
        {
            return $"{variableInfo.MemberInfo.Name}##{variableInfo.ReflectedType?.Name}_{entity.Id}";
        }

        private void DrawEnabled(string componentName, ref bool enabled)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 25);
            ImGui.Checkbox($"##{componentName}", ref enabled);
        }

        private void DrawTransform()
        {
            if (ImGui.CollapsingHeader($"Transform##{currentEntity!.Id}", ImGuiTreeNodeFlags.AllowOverlap | ImGuiTreeNodeFlags.DefaultOpen))
            {
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

                int layer = currentEntity.Layer;
                ImGui.InputInt("Layer##Inspector", ref layer);
                currentEntity.Layer = layer;

                ImGui.Separator();
            }
        }
    }
}
