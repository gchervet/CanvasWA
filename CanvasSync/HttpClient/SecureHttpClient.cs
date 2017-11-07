using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary> 
/// Contiene Tipos para realizar llamadas a clientes REST
/// </summary>
namespace CommonLib.ClientHTTP
{
    /// <summary>
    /// Permite enviar peticiones HTTP y recibir respuestas HTTP enviando credenciales de autenticación.
    /// Serializa y desrealiza Jon automáticamente.
    /// </summary>
    public class SecureHttpClient : IDisposable
    {
        private string ConfigDistributionServiceUrl = null;
        private System.Net.Http.HttpClient httpClient = null;
        private String token = null;

        /// <summary>
        /// Construye una instancia de SecureHttpClient
        /// </summary>
        /// <param name="url">Url base del servicio que se consultara</param>
        public SecureHttpClient(string url)
        {
            this.ConfigDistributionServiceUrl = url;
        }

        /// <summary>
        /// Construye una instancia de SecureHttpClient y almacena las credenciales que se enviaran en el header de cada consulta
        /// </summary>
        /// <param name="url">Url base del servicio que se consultara</param>
        /// <param name="token">Token del usuario autenticado</param>
        public SecureHttpClient(string url, string token)
        {
            this.ConfigDistributionServiceUrl = url;
            this.token = token;
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP DELETE
        /// </summary>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="id">Id de la entidad que se desea eliminar</param>
        /// <returns>La respuesta devuelta por el servicio deserializada a booleano</returns>
        public bool ApiDelete(string apiAction, int id)
        {
            return ApiDelete<bool>(apiAction, id);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP DELETE
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="id">Id de la entidad que se desea eliminar</param>
        /// <returns>La respuesta devuelta por el servicio deserializada a booleano</returns>
        public T ApiDelete<T>(string apiAction, int id)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("id", id.ToString());

            return ApiDelete<T>(apiAction, values);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP DELETE
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="values">Diccionario de valores que se agregaran a los parametros de la ruta</param>
        /// <returns>La respuesta devuelta por el servicio deserializada a booleano</returns>
        /// <returns></returns>
        public T ApiDelete<T>(string apiAction, Dictionary<string, string> values)
        {
            HttpClient client = this.GetClientWithCredentials();
            string fullRoute = GetRouteWithValues(apiAction, values);
            try
            {
                using (HttpResponseMessage response = client.DeleteAsync(fullRoute).Result)
                {
                    return HandleResponse<T>(response, fullRoute);
                }
            }
            catch (Exception ex)
            {
                throw CreateCustomHttpClientRequestException(ex, fullRoute);
            }
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP GET
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiGet<T>(string apiAction)
        {
            return ApiGet<T>(apiAction, null);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP GET
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="id">Id del elemento que se quiere obtener. Se agregará como un parametro en la ruta.</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiGet<T>(string apiAction, int id)
        {
            var values = new Dictionary<string, string>();
            values.Add("id", id.ToString());

            return ApiGet<T>(apiAction, values);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP GET
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="values">Diccionario de valores que se agregaran a los parametros de la ruta</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiGet<T>(string apiAction, Dictionary<string, string> values)
        {
            HttpClient client = this.GetClientWithCredentials();
            string fullRoute = GetRouteWithValues(apiAction, values);
            try
            {
                using (HttpResponseMessage response = client.GetAsync(fullRoute).Result)
                {
                    return HandleResponse<T>(response, fullRoute);
                }
            }
            catch (Exception ex)
            {
                throw CreateCustomHttpClientRequestException(ex, fullRoute);
            }
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP POST
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="entity">La entidad que se desea postear</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiPost<T>(string apiAction, T entity)
        {
            return ApiPost<T>(apiAction, entity, null);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP POST
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado.</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="entity">La entidad que se desea postear</param>
        /// <param name="values">Diccionario de valores que se agregaran a los parametros de la ruta</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiPost<T>(string apiAction, T entity, Dictionary<string, string> values)
        {
            return ApiPost<T, T>(apiAction, entity, values);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP POST
        /// </summary>
        /// <typeparam name="TR">Tipo de retorno esperado.</typeparam>
        /// <typeparam name="TE">Tipo de entidad posteada.</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="entity">La entidad que se desea postear</param>
        /// <param name="values">Diccionario de valores que se agregaran a los parametros de la ruta</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public TR ApiPost<TR, TE>(string apiAction, TE entity, Dictionary<string, string> values)
        {
            HttpClient client = this.GetClientWithCredentials();
            string fullRoute = GetRouteWithValues(apiAction, values);
            try
            {
                string postBody = JsonConvert.SerializeObject(entity);
                using (StringContent content = new StringContent(postBody, Encoding.UTF8, "application/json"))
                {
                    using (HttpResponseMessage response = client.PostAsync(fullRoute, content).Result)
                    {
                        return HandleResponse<TR>(response, fullRoute);
                    }
                }
            }
            catch (Exception ex)
            {
                throw CreateCustomHttpClientRequestException(ex, fullRoute);
            }
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP PUT
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="entity">La entidad que se desea actualizar</param>
        /// <param name="id">Id de la entidad que se desea actualizar</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiPut<T>(string apiAction, T entity, int id)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("id", id.ToString());

            return ApiPut<T>(apiAction, entity, values);
        }

        /// <summary>
        /// Realiza una petición mediante el método HTTP PUT
        /// </summary>
        /// <typeparam name="T">Tipo de retorno esperado</typeparam>
        /// <param name="apiAction">Ruta de la accion a consultar (eg: "Api/Control/Accion" )</param>
        /// <param name="entity">La entidad que se desea actualizar</param>
        /// <param name="values">Diccionario de valores que se agregaran a los parametros de la ruta</param>
        /// <returns>La respuesta devuelta por el servicio deserializada al tipo T especificado</returns>
        public T ApiPut<T>(string apiAction, T entity, Dictionary<string, string> values)
        {
            HttpClient client = this.GetClientWithCredentials();
            string fullRoute = GetRouteWithValues(apiAction, values);
            try
            {
                string putBody = JsonConvert.SerializeObject(entity);
                using (StringContent content = new StringContent(putBody, Encoding.UTF8, "application/json"))
                {
                    using (HttpResponseMessage result = client.PutAsync(fullRoute, content).Result)
                    {
                        return HandleResponse<T>(result, fullRoute);
                    }
                }
            }
            catch (Exception ex)
            {
                throw CreateCustomHttpClientRequestException(ex, fullRoute);
            }
        }

        /// <summary>
        /// Limpia las credenciales almacenadas
        /// </summary>
        public void ClearSessionToken()
        {
            this.token = null;
        }

        /// <summary>
        /// Hace dispose de los elementos internos necesarios
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Establece las credenciales que se enviaran en el header de cada consulta
        /// </summary>
        /// <param name="token">Token del usuario que intentara autenticarse</param>
        public void SetSessionToken(string token)
        {
            this.token = token;
        }

        /// <summary>
        /// Hace dispose de los elementos internos necesarios
        /// </summary>
        /// <param name="a"></param>
        protected virtual void Dispose(bool a)
        {
            if (httpClient != null)
            {
                this.httpClient.Dispose();
            }
        }

        /// <summary>
        /// Crea una excepción custom a partir de una devuelta por HttpClient
        /// </summary>
        /// <param name="ex">Excepción devuelta por HttpClient</param>
        /// <param name="fullRequestUrl">Url del request que generó la excepción</param>
        /// <returns>SecureHttpRequestException o la misma que se recibió</returns>
        private static Exception CreateCustomHttpClientRequestException(Exception ex, string fullRequestUrl)
        {
            if (typeof(HttpRequestException) == ex.GetType())
            {
                return new SecureHttpRequestException("Ocurrió un error al realizar la petición http a la dirección " + fullRequestUrl, ex);
            }
            else if (typeof(AggregateException) == ex.GetType())
            {
                if (typeof(HttpRequestException) == ex.InnerException.GetType())
                {
                    return new SecureHttpRequestException("Ocurrió un error al realizar la petición http a la dirección " + fullRequestUrl, ex);
                }
            }

            return ex;
        }

        /// <summary>
        /// Maneja la respuesta devuelta por HttpClient, devolviendo el objeto deserializado al tipo T indicado
        /// </summary>
        /// <typeparam name="T">El tipo esperado de respuesta</typeparam>
        /// <param name="response">La respuesta devuelta por HttpClient</param>
        /// <param name="fullRequestUrl">Url del request que generó esta respuesta</param>
        /// <returns>El objeto contenido en la respuesta deserializado al tipo T indicado</returns>
        private static T HandleResponse<T>(HttpResponseMessage response, string fullRequestUrl)
        {
            if (response == null)
            {
                throw new ArgumentNullException();
            }

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                T typedResult = JsonConvert.DeserializeObject<T>(result);
                return typedResult;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("No posee autorización para realizar la petición http solicitada. Url: " + fullRequestUrl);
                }
                else
                {
                    throw new ResponseStatusUnsuccessfulException("La respuesta obtuvo un codigo de estado incorrecto. " + (int)response.StatusCode + " - " + response.StatusCode.ToString() + ". Url: " + fullRequestUrl);
                }
            }
        }

        /// <summary>
        /// Obtiene una instancia de HttpClient con las credenciales seteadas con las establecidas en userModel
        /// </summary>
        /// <returns>Instancia de HttpClient con las credenciales seteadas</returns>
        private HttpClient GetClientWithCredentials()
        {
            if (this.httpClient == null)
            {
                this.httpClient = new HttpClient();
                this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (this.token != null && this.httpClient.DefaultRequestHeaders.Authorization == null)
            {
                string[] splitToken = token.Split(' ');
                if (splitToken.Length > 1)
                    this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(splitToken[0], splitToken[1]);
                else
                    this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            }

            return this.httpClient;
        }

        /// <summary>
        /// Obtiene la ruta completa a la accion con la query string formada correctamente a partir de un diccionario string,string y la acción a consultar
        /// </summary>
        /// <param name="apiAction">La acción de WebApi a consumir</param>
        /// <param name="values">Los valores de la ruta</param>
        /// <returns>La ruta completa a la accion</returns>
        private string GetRouteWithValues(string apiAction, Dictionary<string, string> values = null)
        {
            string restDistApi = this.ConfigDistributionServiceUrl;

            if (!restDistApi.EndsWith("/"))
            {
                restDistApi += "/";
            }

            restDistApi += apiAction;

            if (values != null)
            {
                bool flag = false;
                foreach (string key in values.Keys)
                {
                    if (!flag)
                    {
                        restDistApi += "?";
                        flag = true;
                    }
                    else
                    {
                        restDistApi += "&";
                    }

                    restDistApi += string.Format("{0}={1}", key, values[key]);
                }
            }

            return restDistApi;
        }
    }
}