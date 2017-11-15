using CanvasWebApi.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace CanvasWebApi.Controllers
{
    [RoutePrefix("api/Session")]
    public class SessionController : ApiController
    {
        public HttpWebResponse webResponse = null;

        /// <summary>
        /// Verifica si el alumno tiene los datos actualizados en función a la cantidad de meses enviada
        /// </summary>
        /// <param name="username">Nombre de usuario del alumno</param>
        /// <param name="cantMeses">Cantidad de meses para verificar, 6 por defecto</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("GetToken")]
        [HttpGet]
        public static string GetToken()
        {
            // TEST
            string url = "https://apis-lms.qailumno.com/token?grant_type=client_credentials";

            // PROD
            //string url = "https://apis-kennedy.ilumno.com/token?grant_type=client_credentials";
            try
            {
                string ambientPrefix = WebConfigurationManager.AppSettings["AMBIENT_PREFIX"];

                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                webReq.Method = "POST";
                webReq.Headers["Authorization"] = WebConfigurationManager.AppSettings[ambientPrefix + "_BASE_TOKEN"];
                webReq.UseDefaultCredentials = true;
                webReq.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();

                Stream answer = webResponse.GetResponseStream();
                StreamReader _recivedAnswer = new StreamReader(answer);

                TokenDTO tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(_recivedAnswer.ReadToEnd());
                if (tokenDTO != null)
                    return tokenDTO.token_type + " " + tokenDTO.access_token;
                else
                    return null;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}