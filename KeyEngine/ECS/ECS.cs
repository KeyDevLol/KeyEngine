namespace KeyEngine
{
    public static class ECS
    {
        private readonly static EntityCollection EntityCollection = [];
        private readonly static Queue<Entity> entitiesToAdd = [];
        //private readonly static Queue<Entity> removeEntitiesQueue = new Queue<Entity>();
        public static int EntitiesCount => EntityCollection.Count;

        #region Add Entity

        public static void AddEntity(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entitiesToAdd.Enqueue(entity);
        }

        public static Entity AddEntity(string? name = null)
        {
            Entity entity = new Entity(name);

            entitiesToAdd.Enqueue(entity);
            return entity;
        }


        //public static void ClearAddQueue() { } //=> addEntitiesQueue.Clear();

        private static void PassEntitiesToAdd()
        {
            foreach (Entity entity in entitiesToAdd)
            {
                EntityCollection.Add(entity);
            }

            entitiesToAdd.Clear();
        }

        #endregion Add Entity

        #region Remove Entity

        //public static void RemoveEntity(Entity entity)
        //{
        //    entity.CallDeleted();
        //    EntityCollection.Remove(entity);
        //}

        //public static void RemoveEntity(int index)
        //{
        //    EntityCollection[index].CallDeleted();
        //    EntityCollection.RemoveAt(index);
        //}

        public static void RemoveAllEntities()
        {
            while (EntityCollection.Count > 0)
            {
                EntityCollection[0].CallDeleted();
                EntityCollection.RemoveAt(0);
            }   
        }

        #endregion Remove Entities

        public static Entity? FindEntityByName(string name)
        {
            foreach (Entity entity in EntityCollection)
            {
                if (entity.Name == name)
                    return entity;
            }

            return null;
        }

        public static IEnumerable<Entity> GetAllEntities()
        {
            return EntityCollection;
        }

        #region Internal Calls

        internal static void CallStart()
        {
            PassEntitiesToAdd();

#if ENABLE_EDITOR
            try
            {
#endif
                for (int i = EntityCollection.Count; i-- > 0;)
                {
                    Entity entity = EntityCollection[i];

                    if (!entity.Active)
                        continue;

                    entity.CallStart();
                }
#if ENABLE_EDITOR
            }
            catch (Exception exc) { Log.Print(exc, LogType.Error); }
#endif
        }

        internal static void CallUpdate(float deltaTime)
        {
            PassEntitiesToAdd();

            if (!SceneManager.IsSceneRunning)
                return;

#if ENABLE_EDITOR
            try
            {
#endif
                for (int i = EntityCollection.Count; i-- > 0;)
                {
                    Entity entity = EntityCollection[i];

                    if (!entity.IsAlive)
                    {
                        entity.CallDeleted();
                        EntityCollection.Remove(entity);
                        continue;
                    }


                    if (!entity.Active)
                        continue;

                    entity.CallUpdate(deltaTime);
                }
#if ENABLE_EDITOR
                //PassAddQueue();
                //PassRemoveQueue();
            }
            catch (Exception exc) { Log.Print(exc, LogType.Error); }
#endif
        }

        internal static void CallRender()
        {
#if ENABLE_EDITOR
            try
            {
#endif
                for (int i = EntityCollection.Count; i-- > 0;)
                {
                    Entity entity = EntityCollection[i];

                    if (!entity.Active)
                        continue;

                    entity.CallRender();
                }
#if ENABLE_EDITOR
            }
            catch (Exception exc) { Log.Print(exc, LogType.Error); }
#endif
        }

        internal static void RefreshLayer(Entity entity)
        {
            EntityCollection.RefreshLayer(entity);
        }

        #endregion Internal Calls
    }
}
