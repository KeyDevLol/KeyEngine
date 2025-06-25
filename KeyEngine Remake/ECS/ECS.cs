using KeyEngine.Rendering;

namespace KeyEngine
{
    public static class ECS
    {
        private readonly static EntityCollection entityCollection = new EntityCollection();
        //private readonly static Queue<Entity> addEntitiesQueue = new Queue<Entity>();
        //private readonly static Queue<Entity> removeEntitiesQueue = new Queue<Entity>();

        #region Add Entity
        public static void AddEntity(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            //addEntitiesQueue.Enqueue(entity);
            entityCollection.Add(entity);
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
            entityCollection.Remove(entity);
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
            foreach (Entity entity in entityCollection)
            {
                if (entity.Name == name)
                    return entity;
            }

            return null;
        }

        public static Entity[] GetAllEntities()
        {
            return [.. entityCollection.Entities];
        }

        public static Entity Get()
        {
            return entityCollection.Entities[0];
        }

        public static void DeleteAllEntities()
        {
            while (entityCollection.Count > 0)
            {
                RemoveEntity(entityCollection[0]);
            }
        }

        internal static void RefreshLayer(Entity entity)
        {
            entityCollection.RefreshLayer(entity);
        }

        internal static void CallStart()
        {
            Log.Print("Called start");
            Log.Print(entityCollection.Count);
            Log.Print(SceneManager.SceneIsRunning);

            for (int i = entityCollection.Count; i-- > 0;)
            {
                Entity entity = entityCollection[i];

                if (!entity.Active)
                    continue;

                Log.Print($"{entity.Name} called start");
                entity.CallStart();
            }
        }

        internal static void CallUpdate(float deltaTime)
        {
            if (!SceneManager.SceneIsRunning)
                return;

            try
            {
                for (int i = entityCollection.Count; i-- > 0;)
                {
                    Entity entity = entityCollection[i];

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
            for (int i = entityCollection.Count; i-- > 0;)
            {
                Entity entity = entityCollection[i];

                if (!entity.Active)
                    continue;

                entity.CallRender();
            }
        }
    }
}
