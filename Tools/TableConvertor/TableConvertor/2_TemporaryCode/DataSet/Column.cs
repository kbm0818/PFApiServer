namespace TableConvertor
{
    internal class Column
    {
        public string Name { get; protected set; }
        public IType? Type { get; protected set; }

        public Column(string columnName, string typeName)
        {
            this.Name = columnName;
            this.Type = TypeFactory.Get(typeName);
            if (Type is null)
            {
                throw new InvalidDataException(string.Format("{0}-{1} don't support", columnName, typeName));
            }
        }
    }
}
