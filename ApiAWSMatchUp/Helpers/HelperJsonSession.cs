using Newtonsoft.Json;

namespace ApiAWSMatchUp.Helpers
{
    public class HelperJsonSession
    {
        //Vamos a utilizar el metodo GetString() como herramienta
        public static string SerializeObject<T>(T data)
        {
            //Convertimos el objeto a string mediante Newton
            string json = JsonConvert.SerializeObject(data);
            return json;
        }

        public static T DeserializeObject<T>(string data)
        {
            T objeto = JsonConvert.DeserializeObject<T>(data);
            return objeto;
        }
    }
}
