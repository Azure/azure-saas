namespace Saas.SignupAdministration.Web;

public interface IPersistenceProvider
{
    public void Persist(string key, object value);

    public T? Retrieve<T>(string key);
}
