using KeyEngine.Editor.GUI;

namespace KeyEngine.Editor.Systems
{
    public class GizmosSystem : EditorSystem
    {
        public override void Render()
        {
            for (int i = ECS.EntityCollection.Count; i-- > 0;)
            {
                Entity entity = ECS.EntityCollection[i];

                if (!entity.Active)
                    continue;

                if (Inspector.GetCurrentEntity() == entity)
                    entity.CallRenderSelectedGizmos();

                entity.CallRenderGizmos();
            }
        }
    }
}
