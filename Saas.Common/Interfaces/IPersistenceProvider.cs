namespace Saas.Common.Interface;

public interface IPersistenceProvider
{
    public void Persist(string key, object value);

    public T Retrieve<T>(string key);
}
