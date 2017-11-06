using CommonLib.ClientHTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UKSync
{
    /// <summary>
    /// Clase de ejemplo. Debe crearse una similar a esta para cada api que desee consumirse.
    /// </summary>
    public class CanvasApiClient : WinServiceSecureHttpClient
    {
        /// <summary>
        /// Obtiene una instancia de SecureHttpClient con el Token de sesion y la url seteadas
        /// </summary>
        /// <returns></returns>
        public static SecureHttpClient GetInstance()
        {
            return GetClientInstance(UKSync.Properties.Settings.Default.Url);
        }
    }
}