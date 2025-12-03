namespace KeyEngine.Serialization
{
    public class YamlDictionaryVariable : YamlVariable
    {
        public SerializableVariableType KeyType;

        public YamlDictionaryVariable(SerializableVariableType keyType, SerializableVariableType valueType, object? variableValue) : base(SerializableCollectionType.Dictionary, valueType, variableValue)
        {
            KeyType = keyType;
        }

        public YamlDictionaryVariable() { }
    }
}
