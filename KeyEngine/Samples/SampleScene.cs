using KeyEngine.Physics;
using KeyEngine.Mathematics;

namespace KeyEngine.Samples
{
    public class SampleScene : IScene
    {
        public void Load()
        {
            // Creating a new entity
            Entity entity = ECS.AddEntity("Cute White Square");

            // Adding new component to entity
            entity.AddComponent<SpriteRenderer>();
            entity.AddComponent<RigidBody>();

            // Getting a component
            SpriteRenderer? spriteRenderer = entity.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.Color = Color32.White;
            else
                Log.Print("SpriteRenderer is null.", LogType.Error);

            Log.Print("SampleScene Loaded.");
        }

        public void Unload()
        {
            // Called when the scene is unloaded
            Log.Print("SampleScene Unloaded.");
        }
    }
}
