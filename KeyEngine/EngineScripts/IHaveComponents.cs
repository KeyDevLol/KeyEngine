
namespace KeyEngine
{
    public interface IHaveComponents
    {
        T GetComponent<T>() where T : Component;

        void AddComponent(Component component);

        void AddComponent<T>() where T : Component;
    }
}
