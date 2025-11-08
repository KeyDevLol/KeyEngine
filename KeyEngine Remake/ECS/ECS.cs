using KeyEngine.Rendering;

namespace KeyEngine
{
    public static class ECS
    {
        public readonly static EntityCollection EntityCollection = [];
        //private readonly static Queue<Entity> addEntitiesQueue = new Queue<Entity>();
        //private readonly static Queue<Entity> removeEntitiesQueue = new Queue<Entity>();
        public static int EntitiesCount => EntityCollection.Count;

        #region Add Entity
        public static void AddEntity(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            //addEntitiesQueue.Enqueue(entity);
            EntityCollection.Add(entity);
        }

        public static Entity AddEntity(string? name = null)
        {
            Entity entity = new Entity(name);
            AddEntity(entity);

            return entity;
        }


        public static void ClearAddQueue() { } //=> addEntitiesQueue.Clear();

        public static void PassAddQueue()
        {
            //while (addEntitiesQueue.Count > 0)
            //{
            //    Entity entity = addEntitiesQueue.Dequeue();
            //    entityCollection.Add(entity);
            //}
        }
        #endregion

        #region Remove Entity

        public static void RemoveEntity(Entity entity)
        {
            //removeEntitiesQueue.Enqueue(entity);

            entity.CallDeleted();
            EntityCollection.Remove(entity);
        }

        public static void ClearRemoveQueue() { } //=> removeEntitiesQueue.Clear();

        public static void PassRemoveQueue()
        {
            //while (removeEntitiesQueue.Count > 0)
            //{
            //    Entity entity = removeEntitiesQueue.Dequeue();

            //    entity.CallDeleted();
            //    entityCollection.Remove(entity);
            //}
        }
        #endregion

        public static Entity? FindEntityByName(string name)
        {
            foreach (Entity entity in EntityCollection)
            {
                if (entity.Name == name)
                    return entity;
            }

            return null;
        }

        public static Entity[] GetAllEntities()
        {
            return [.. EntityCollection.Entities];
        }

        public static Entity Get()
        {
            return EntityCollection.Entities[0];
        }

        public static void DeleteAllEntities()
        {
            while (EntityCollection.Count > 0)
            {
                RemoveEntity(EntityCollection[0]);
            }
        }

        internal static void RefreshLayer(Entity entity)
        {
            EntityCollection.RefreshLayer(entity);
        }

        internal static void CallStart()
        {
            for (int i = EntityCollection.Count; i-- > 0;)
            {
                Entity entity = EntityCollection[i];

                if (!entity.Active)
                    continue;

                entity.CallStart();
            }
        }

        internal static void CallUpdate(float deltaTime)
        {
            if (!SceneManager.SceneIsRunning)
                return;

            try
            {
                for (int i = EntityCollection.Count; i-- > 0;)
                {
                    Entity entity = EntityCollection[i];

                    if (!entity.Active)
                        continue;

                    entity.CallUpdate(deltaTime);
                }

                PassAddQueue();
                PassRemoveQueue();
            }
            catch(Exception exc) { Log.Print(exc, LogType.Error); }
        }

        internal static void CallRender()
        {
            for (int i = EntityCollection.Count; i-- > 0;)
            {
                Entity entity = EntityCollection[i];

                if (!entity.Active)
                    continue;

                entity.CallRender();
            }
        }
    }
}
