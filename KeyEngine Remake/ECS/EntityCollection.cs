using System.Collections;

namespace KeyEngine
{
    public class EntityCollection : IEnumerable
    {
        public List<Entity> Entities;

        public EntityCollection()
        {
            Entities = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            int index = Entities.BinarySearch(entity);
            if (index < 0)
                index = ~index;

            Entities.Insert(index, entity);
        }

        public void RefreshLayer(Entity entity)
        {
            Remove(entity);

            int index = Entities.BinarySearch(entity);
            if (index < 0)
                index = ~index;

            Entities.Insert(index, entity);
        }

        public Entity? Get(Entity entity)
        {
            int index = Entities.IndexOf(entity);

            if (index != -1)
                return Entities[index];

            return null;
        }

        public bool Get(Entity entity, out Entity? result)
        {
            result = Get(entity);
            return result != null;
        }

        public void RemoveAt(int index) => Entities.RemoveAt(index);
        public void Remove(Entity entity) => Entities.Remove(entity);
        public bool Contains(Entity entity) => Entities.Contains(entity);
        public Entity Find(string name) => Entities[Entities.FindIndex(e => e.Name == name)];
        public Entity Find(Guid id) => Entities[Entities.FindIndex(e => e.Id == id)];
        public int FindIndex(string name) => Entities.FindIndex(e => e.Name == name);
        public int FindIndex(Guid id) => Entities.FindIndex(e => e.Id == id);
        public int Count => Entities.Count;
        public IEnumerator GetEnumerator() => Entities.GetEnumerator();

        public Entity this[int index]
        {
            get => Entities[index];
            set => Entities[index] = value;
        }

        //private struct LayredEntity : IComparable<LayredEntity>
        //{
        //    public Entity Entity;
        //    public int Layer;

        //    public LayredEntity(Entity entity, int layer)
        //    {
        //        Entity = entity;
        //        Layer = layer;
        //    }

        //    public int CompareTo(LayredEntity other)
        //    {
        //        return Layer.CompareTo(other.Layer);
        //    }
        //}
    }
}
