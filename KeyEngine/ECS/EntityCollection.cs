using System.Collections;

namespace KeyEngine
{
    public class EntityCollection : IReadOnlyList<Entity>
    {
        private readonly List<Entity> entities;

        public EntityCollection()
        {
            entities = [];
        }

        public void Add(Entity entity)
        {
            int index = entities.BinarySearch(entity, EntityLayerComparer.Instance);
            if (index < 0)
                index = ~index;

            entities.Insert(index, entity);
        }

        public void RefreshLayer(Entity entity)
        {
            Remove(entity);

            int index = entities.BinarySearch(entity, EntityLayerComparer.Instance);
            if (index < 0)
                index = ~index;

            entities.Insert(index, entity);
        }

        public Entity? Get(Entity entity)
        {
            int index = entities.BinarySearch(entity, EntityLayerComparer.Instance);

            if (index < 0)
                return null;

            return entities[index];
        }

        public bool Get(Entity entity, out Entity? result)
        {
            result = Get(entity);
            return result != null;
        }

        public void RemoveAt(int index) => entities.RemoveAt(index);
        public void Remove(Entity entity) => entities.Remove(entity);
        public bool Contains(Entity entity) => entities.Contains(entity);
        [Obsolete(message:$"Use {nameof(FindByName)}", DiagnosticId ="223")]
        public Entity Find(string name) => entities[entities.FindIndex(e => e.Name == name)];
        public Entity? FindByName(string name) => entities.FirstOrDefault(e => e.Name == name);
        [Obsolete($"Use {nameof(FindById)} instead.")]
        public Entity Find(Guid id) => entities[entities.FindIndex(e => e.Id == id)];
        public Entity? FindById(Guid id) => entities.FirstOrDefault(e => e.Id == id);
        public int FindIndex(string name) => entities.FindIndex(e => e.Name == name);
        public int FindIndex(Guid id) => entities.FindIndex(e => e.Id == id);
        public int Count => entities.Count;
        public IEnumerator GetEnumerator() => entities.GetEnumerator();

        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        public Entity this[int index]
        {
            get => entities[index];
            set => entities[index] = value;
        }

        private class EntityLayerComparer : IComparer<Entity>
        {
            public static readonly EntityLayerComparer Instance = new();

            public int Compare(Entity? first, Entity? second)
            {
                if (ReferenceEquals(first, second)) return 0;
                if (first is null) return -1;
                if (second is null) return 1;

                if (first.Layer != second.Layer)
                    return first.Layer.CompareTo(second.Layer);

                return first.Id.CompareTo(second.Id);
            }
        }
    }
}
