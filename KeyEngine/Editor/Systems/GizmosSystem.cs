using KeyEngine.Editor.GUI.Inspector;

namespace KeyEngine.Editor.Systems
{
    public class GizmosSystem : EditorSystem
    {
        public override void Render()
        {
            foreach (Entity entity in ECS.GetAllEntities())
            {
                if (!entity.Active)
                    continue;

                if (Inspector.GetCurrentEntity() == entity)
                    entity.CallRenderSelectedGizmos();

                entity.CallRenderGizmos();
            }
        }
    }
}
