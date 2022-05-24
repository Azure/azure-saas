using Newtonsoft.Json;
using Saas.Common.Interface;

namespace Saas.SignupAdministration.Web
{
    public class JsonSessionPersistenceProvider : IPersistenceProvider
    { 
        private ISession Session
        {
            get
            {
                return AppHttpContext.Current.Session;
            }
        }
   
        public void Persist(string key, object value)
        {
            Session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public T Retrieve<T>(string key)
        {
            var value = Session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
