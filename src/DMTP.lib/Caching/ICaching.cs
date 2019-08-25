namespace DMTP.lib.Caching
{
    public interface ICaching
    {
        (T Object, bool NotFound) Get<T>(string key);

        bool InsertOrUpdate<T>(string key, T objectValue);
    }
}