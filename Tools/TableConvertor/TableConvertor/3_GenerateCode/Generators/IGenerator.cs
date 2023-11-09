namespace TableConvertor
{
    internal interface IGenerator
    {
        void Generate(string fileName, params Sheet[] sheets);
    }
}
