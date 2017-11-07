using CanvasWebApi.Common;
using CanvasWebApi.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CanvasWebApi.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Crea el usuario enviado
        /// </summary>
        /// <param name="userDTO">Tipo que representa a un objeto de usuario</param>
        /// <returns>JSON con la información del usuario generado</returns>
        [Route("Create")]
        [HttpPost]
        public object Create([FromBody] User userDTO)
        {
            logger.Info("UserController/Create - Task 'Create user' STARTED");

            try
            {
                if (userDTO != null)
                {
                    WebRequest request = WebRequest.Create(WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/users");
                    request.Method = "POST";

                    request.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                    string postData = "{\"user\": {\"short_name\" : \"" + userDTO.user.full_name +
                                      "\",\"sortable_name\" : \"" + userDTO.user.full_name +
                                      "\",\"full_name\" : \"" + userDTO.user.full_name +
                                      "\",\"login\": \"" + userDTO.user.login +
                                      "\",\"email\":\"" + userDTO.user.email +
                                      "\",\"sis_user_id\": \"" + userDTO.user.sis_user_id + "\"}}";

                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                    request.ContentType = "application/json";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                    dataStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(dataStream);
                    object rtn = JsonConvert.DeserializeObject<UserReturn>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        return rtn;
                    }

                    rtn = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        ErrorMessage errorMessageDto = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                        if (errorMessageDto != null)
                        {
                            return new UserReturn() { error_message = errorMessageDto.errors.First().message };
                        }
                        logger.Info("UserController/Create - Task 'Create user' FINISHED");
                        return null;
                    }
                    logger.Info("UserController/Create - Task 'Create user' FINISHED");
                    return null;
                }
                logger.Info("UserController/Create - Task 'Create user' FINISHED");
            }
            catch (WebException e)
            {
                logger.Error("UserController/Create - Task 'Create user' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                {
                    HttpContext.Current.Request.Headers.Remove("Authorization");
                    HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                    return Create(userDTO);
                }
                if (e.Message.Contains(HttpStatusCode.BadRequest.ToString()))
                {
                    var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                    JObject obj = JsonConvert.DeserializeObject<JObject>(resp);
                    if (obj["errors"]["pseudonym"] != null)
                    {
                        foreach (var x in obj["errors"]["pseudonym"].First)
                        {
                            if (x.First["message"] != null)
                                return new UserReturn() { error_message = x.First["message"].ToString() };
                        }
                    }
                    return new UserReturn() { error_message = e.ToString() };
                }
            }
            logger.Info("UserController/Create - Task 'Create user' FINISHED");
            return null;
        }

        /// <summary>
        /// Obtiene un usuario
        /// </summary>
        /// <param name="sis_user_id">Identificador de usuario en el sistema</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("Get")]
        [HttpGet]
        public object Get(int sis_user_id)
        {
            return null;
        }

        /// <summary>
        /// Obtiene el listado de usuarios de Canvas
        /// </summary>
        /// <param name="per_page">Cantidad de usuarios por página, opcional.</param>
        /// <returns>Listado de usuarios de Canvas</returns>
        [Route("GetAll")]
        [HttpGet]
        public object GetAll(int per_page = 0)
        {
            logger.Info("UserController/GetAll - Task 'Get all users' STARTED");

            string url = string.Empty;
            if (per_page == 0)
                url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/users?per_page=" + per_page;
            else
                url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/users";

            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                webReq.Method = "GET";
                webReq.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();

                Stream answer = webResponse.GetResponseStream();
                StreamReader _recivedAnswer = new StreamReader(answer);

                logger.Info("UserController/GetAll - Task 'Get all users' FINISHED");
                return JsonConvert.DeserializeObject<List<UserDTO>>(_recivedAnswer.ReadToEnd());
            }
            catch (Exception e)
            {
                logger.Error("UserController/GetAll - Task 'Get all users' FINISHED WITH ERROR: \n" + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                {
                    HttpContext.Current.Request.Headers.Remove("Authorization");
                    HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                    return GetAll(per_page);
                }
                return e.ToString();
            }
        }

        /// <summary>
        /// Sincroniza los usuarios de Académico con los de Canvas
        /// </summary>
        [Route("Sync")]
        [HttpPost]
        public void SyncToCanvas()
        {
            logger.Info("UserController/SyncToCanvas - Task 'Create user' STARTED");

            UserService.SyncToCanvas();

            logger.Info("UserController/SyncToCanvas - Task 'Create user' FINISHED");
        }

        /// <summary>
        /// Busca un valor en objetos JSON genéricos
        /// </summary>
        /// <param name="jsonObject">Objeto JSON genérico</param>
        /// <param name="nameFilter">Nombre del campo que se filtrará</param>
        /// <returns>El valor string del campo, o null en su defecto</returns>
        [Route("SearchValueInJObject")]
        [HttpPost]
        public string SearchValueInJObject([FromBody]JObject jsonObject, string nameFilter)
        {
            foreach (JProperty jsonRootProperty in jsonObject.Properties())
            {
                if (jsonRootProperty.Name.Equals(nameFilter))
                {
                    return jsonRootProperty.Value.ToString();
                }
                JToken value = jsonRootProperty.Value;
                if (value.Type == JTokenType.Object)
                {
                    SearchValueInJObject((JObject)value, nameFilter);
                }
                else if (value.Type == JTokenType.Array)
                {
                    foreach (JObject jsonArrayProperty in value)
                    {
                        SearchValueInJObject((JObject)jsonArrayProperty, nameFilter);
                    }
                }
            }
            return null;
        }
    }
}