using KeyEngine.Editor.Attributes;
using KeyEngine.Serialization;

namespace KeyEngine
{
    /// <summary>
    /// Base class for everything attached to a <see cref="Entity"/>
    /// </summary>
    public class Component : ISerializable
    {
        /// <summary>
        /// The entity that owns this component.
        /// </summary>
        public readonly Entity Owner;
        /// <summary>
        /// Gets or sets a value indicating if the component is active.
        /// </summary>
        [HideInInspector] public bool Enabled { get; set; } = true;

        public Component(Entity owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Initialization method called after the component is attached to an entity.
        /// </summary>
        public virtual void Start() { }
        /// <summary>
        /// Updates the component state each frame.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the previous frame, in seconds.</param>
        public virtual void Update(float deltaTime) { }
        /// <summary>
        /// Called each frame to render the component's visual representation.
        /// </summary>
        public virtual void Render() { }
        /// <summary>
        /// Called when the component is removed from its entity or the entity is destroyed.
        /// </summary>
        public virtual void OnDeleted() { }
        /// <summary>
        /// Invoked when the Enabled property is set to false.
        /// </summary>
        public virtual void OnDisabled() { }
        /// <summary>
        /// Invoked when the Enabled property is set to true.
        /// </summary>
        public virtual void OnEnabled() { }
        /// <summary>
        /// Renders editor-only visualizations when the owning entity is selected.
        /// </summary>
        public virtual void RenderSelectedGizmos() { }
        /// <summary>
        /// Renders editor-only visualizations that are always visible (unselected).
        /// </summary>
        public virtual void RenderGizmos() { }

        public virtual SerializeData Serialize() => new SerializeData();
        public virtual void Deserialize(SerializeData data) { }
    }
}
