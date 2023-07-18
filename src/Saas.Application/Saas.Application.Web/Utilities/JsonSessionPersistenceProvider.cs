using Newtonsoft.Json;
using Saas.Application.Web.Interfaces;

namespace Saas.Application.Web
{
    public class JsonSessionPersistenceProvider : IPersistenceProvider
    {
        private ISession? Session => AppHttpContext.Current?.Session;

        public void Persist(string key, object value)
        {
            Session?.SetString(key, JsonConvert.SerializeObject(value));
        }

        public T? Retrieve<T>(string key)
        {
            var value = Session?.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
