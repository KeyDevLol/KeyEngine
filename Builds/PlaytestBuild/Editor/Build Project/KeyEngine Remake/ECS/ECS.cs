namespace KeyEngine
{
    public static class ECS
    {
        private readonly static EntityCollection entityCollection = new EntityCollection();
        public static uint EntitiesHashIdCounter { get; private set; }

        public static Entity AddEntity(string? name)
        {
            EntitiesHashIdCounter++;
            Entity entity = new Entity(entityCollection.Count.GetHashCode(), name);
            entityCollection.Add(entity);

            return entity;
        }

        public static Entity AddEntity()
        {
            return AddEntity(null);
        }

        public static void RemoveEntity(Entity entity)
        {
            entity.CallDeleted();
            entityCollection.Remove(entity);
            EntitiesHashIdCounter = 0;
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
            entityCollection.Entities.Clear();

            EntitiesHashIdCounter = 0;
        }

        internal static void RefreshLayer(Entity entity)
        {
            entityCollection.RefreshLayer(entity);
        }

        internal static void CallStart()
        {
            for (int i = entityCollection.Count; i-- > 0;)
            {
                Entity entity = entityCollection[i];

                if (!entity.Active)
                    continue;

                entity.CallStart();
            }
        }

        internal static void CallUpdate(float deltaTime)
        {
            try
            {
                for (int i = entityCollection.Count; i-- > 0;)
                {
                    Entity entity = entityCollection[i];

                    if (!entity.Active)
                        continue;

                    entity.CallUpdate(deltaTime);
                }
            }
            catch { }
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
