using ImGuiNET;

namespace KeyEngine_Workshop.GUI
{
    public abstract class GuiWindowBase
    {
        public bool IsVisible { get => _isVisible; set => _isVisible = value; }
        protected bool _isVisible = true;
        public virtual string Title { get; set; } = "ImGuiWindow";
        protected virtual ImGuiWindowFlags WindowFlags { get; set; }

        public void OnRenderFrame()
        {
            if (!IsVisible)
                return;

            PreBegin();
            
            if (Begin())
            {
                DrawContent();
            }

            End();
        }

        protected virtual bool Begin() 
        {
            return ImGui.Begin(Title, ref _isVisible, WindowFlags);
        }

        protected virtual void End()
        {
            ImGui.End();
        }

        public virtual void OnUpdateFrame(float deltaTime) { }
        public virtual void OnRegister() { }
        protected virtual void PreBegin() { }

        protected abstract void DrawContent();
    }
}
