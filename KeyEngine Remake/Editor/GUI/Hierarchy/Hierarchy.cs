using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.GUI
{
    public class Hierarchy : EditorWindow
    {
        private static Hierarchy instance = null!;
        public static Hierarchy Instance { get => instance; }

        private string[] entityNames = [];
        private Entity[] entities = [];

        private int currentListIndex = -1;

        public Hierarchy()
        {
            if (instance != null)
                return;

            instance = this;
            title = "Hierachy";
        }

        public override void Render()
        {
            // Баг, если на сцене только одна сущность, то на нее нельзя переключится
            RefreshEntitiesList();
            int temp = currentListIndex;
            ImGui.ListBox($"##{nameof(Hierarchy)}_ListBox", ref currentListIndex, entityNames, entityNames.Length);
            if (temp != currentListIndex)
            {
                Inspector.RefreshEntity(entities[currentListIndex]);
            }
        }

        public void RefreshEntitiesList()
        {
            Entity[] entities = ECS.GetAllEntities();
            string[] newNames = new string[entities.Length];

            int i = 0;

            foreach (Entity entity in entities)
            {
                newNames[i] = entity.Name;
                i++;
            }

            entityNames = newNames;
            this.entities = entities;
        }
    }
}
