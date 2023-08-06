
namespace KeyEngine
{
    public class Component
    {
        public GameObject gameObject;

        public virtual void Start() { }

        public virtual void Update() { }

        /// <summary>
        /// Вызывается когда удаляется объект на котором находится компонент, или же когда удаляется сам компонент
        /// </summary>
        public virtual void OnObjectDestroy() { }

        public virtual void FixedUpdate() { }
    }
}
