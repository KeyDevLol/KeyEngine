namespace KeyEngine.Serialization
{
    public struct YamlVariable
    {
        public SerializableCollectionType CollectionType;
        public SerializableVariableType VariableType;
        public object? VariableValue;

        public YamlVariable(SerializableCollectionType collectionType, SerializableVariableType variableType, object? variableValue)
        {
            CollectionType = collectionType;
            VariableType = variableType;
            VariableValue = variableValue;
        }
    }
}
