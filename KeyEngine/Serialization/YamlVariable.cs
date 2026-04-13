namespace KeyEngine.Serialization
{
    public class YamlVariable
    {
        public SerializableCollectionType CollectionType;
        public SerializableVariableType SerializableType;
        public object? VariableValue;
        public Type? SystemType;

        public YamlVariable(SerializableCollectionType collectionType, SerializableVariableType variableType, object? variableValue)
        {
            CollectionType = collectionType;
            SerializableType = variableType;
            VariableValue = variableValue;
            SystemType = variableValue?.GetType();
        }

        public YamlVariable() { }
    }
}
