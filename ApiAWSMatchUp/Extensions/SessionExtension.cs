using ApiAWSMatchUp.Helpers;

namespace ApiAWSMatchUp.Extensions
{
    public static class SessionExtension
    {
        //Creamos un metodo para recuperar cualquier objeto
        public static T GetObject<T>(this ISession session, string key)
        {
            //Ya estamos trabajando con HttpContext.Session
            //Debemos recuperar lo que tenemos almacenado dentro de Session
            string json = session.GetString(key);
            //Que sucede si recuperamos algo de session que no existe
            //Si no tenemos nada almacenado debemos devolver un valor por defecto
            if (json == null)
            {
                return default(T);
            }
            else
            {
                T data = HelperJsonSession.DeserializeObject<T>(json);
                return data;
            }
        }

        public static void SetObject(this ISession session, string key, object value)
        {
            string data = HelperJsonSession.SerializeObject(value);
            //Almacenamos el json dentro de session
            session.SetString(key, data);
        }
    }
}
