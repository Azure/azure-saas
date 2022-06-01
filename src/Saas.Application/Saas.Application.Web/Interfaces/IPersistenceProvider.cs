namespace Saas.Application.Web.Interfaces;

public interface IPersistenceProvider
{
    public void Persist(string key, object value);

    public T Retrieve<T>(string key);
}
