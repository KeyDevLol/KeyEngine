namespace KeyEngine
{
    public class SampleScene : IScene
    {
        public void Load()
        {
            // Creating a new entity
            Entity entity = ECS.AddEntity("Entity Name");

            // Adding new component to entity
            entity.AddComponent<SpriteRenderer>();

            // Getting a component and changing its variable
            SpriteRenderer? spriteRenderer = entity.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.Color = Color.Red;
            else
                Log.Print("SpriteRenderer is null!", LogType.Error);

            Log.Print("SampleScene Loaded!");
        }

        public void Unload()
        {
            // Called when the scene is unloaded.
            Log.Print("SampleScene Unloaded.");
        }
    }
}
