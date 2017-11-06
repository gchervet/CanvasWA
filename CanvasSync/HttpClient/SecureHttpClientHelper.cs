using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary> 
/// Contiene Tipos para realizar llamadas a clientes REST
/// </summary>
namespace CommonLib.ClientHTTP
{
    /// <summary>
    /// Provee de métodos que facilitan el manejo de peticiones
    /// </summary>
    public class SecureHttpClientHelper
    {
        private static Dictionary<string, SecureHttpClient> secureHttpClients = new Dictionary<string, SecureHttpClient>();
        private static Dictionary<string, string> sessionTokens = new Dictionary<string, string>();

        /// <summary>
        /// Devuelve un objeto SecureHttpClient la ficha de acceso asignada
        /// </summary>
        /// <param name="url">Dirección de cliente REST a consultar</param>
        /// <returns>Objeto SecureHttpClient</returns>
        public static SecureHttpClient GetClientInstance(string url)
        {
            if (!secureHttpClients.ContainsKey(url))
            {
                secureHttpClients.Add(url, new SecureHttpClient(url));
            }
            SecureHttpClient secureHttpClient = secureHttpClients[url];

            string sessionToken = null;
            if (sessionTokens.TryGetValue(url, out sessionToken))
            {
                secureHttpClient.SetSessionToken(sessionToken);
            }

            return secureHttpClient;
        }

        /// <summary>
        /// Establece el ficha de acceso
        /// </summary>
        /// <param name="url">Dirección de cliente REST a consultar</param>
        /// <param name="token">ficha de acceso</param>
        public static void SetToken(string url, string token)
        {
            sessionTokens[url] = token;
        }
    }
}