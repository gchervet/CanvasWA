using CommonLib.ClientHTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UKSync
{
    /// <summary>
    /// Wrapper de httpclient para windows service.
    /// </summary>
    public class WinServiceSecureHttpClient
    {
        private static Dictionary<string, SecureHttpClient> secureHttpClients = new Dictionary<string, SecureHttpClient>();
        private static Dictionary<string, string> sessionTokens = new Dictionary<string, string>();

        /// <summary>
        /// Devuelve una instancia de SecureHttpClient con las credenciales (token) ya asignadas.
        /// </summary>
        /// <param name="url">Url de la api a consultar</param>
        /// <returns>SecureHttpClient con credenciales asignadas</returns>
        protected static SecureHttpClient GetClientInstance(string url)
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
        /// Almacena el token en sesión
        /// </summary>
        /// <param name="url">Url asociada</param>
        /// <param name="token">Valor del token</param>
        protected static void SetToken(string url, string token)
        {
            sessionTokens[url] = token;
        }
    }
}