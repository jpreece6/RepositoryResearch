namespace DataEngine.RepositoryCore
{
    public interface IRepositoryKeyValue
    {
        void Save<T>(string key, T tValue);
        void Remove(string key);
        T Get<T>(string key);
    }
}
