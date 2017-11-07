using CanvasWebApi.Common;
using CanvasWebApi.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CanvasWebApi.Controllers
{
    [RoutePrefix("api/Course")]
    public class CourseController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Verifica si el alumno tiene los datos actualizados en función a la cantidad de meses enviada
        /// </summary>
        /// <param name="username">Nombre de usuario del alumno</param>
        /// <param name="cantMeses">Cantidad de meses para verificar, 6 por defecto</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("Create")]
        [HttpPost]
        public object Create([FromBody] Course courseDTO)
        {
            logger.Info("CourseController/Create - Task 'Create course' STARTED");

            if (courseDTO != null)
            {
                try
                {
                    WebRequest request = WebRequest.Create(@"https://apis-kennedy.qailumno.com:443/kennedy/api/lms/v1/courses");
                    request.Method = "POST";

                    request.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());
                    
                    string postData = "{\"course\": {\"account_id\" : \"" + courseDTO.course.account_id +
                                        "\",\"sis_course_id\" : \"" + courseDTO.course.sis_course_id +
                                        "\",\"name\" : \"" + courseDTO.course.name +
                                        "\",\"sis_term_id\": \"" + courseDTO.course.term_id +
                                        //"\",\"sis_master_id\": \"" + courseDTO.course.term_id +
                                        "\",\"import_content:\": false," +
                                        "\",\"code\": \"" + courseDTO.course.code + "\"}}";

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
                    object rtn = JsonConvert.DeserializeObject<CourseReturn>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        logger.Info("CourseController/Create - Task 'Create course' STARTED");
                        return rtn;
                    }

                    rtn = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        ErrorMessage errorMessageDto = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                        if (errorMessageDto != null)
                        {
                            logger.Info("CourseController/Create - Task 'Create course' FINISHED");
                            return new CourseReturn() { error_message = errorMessageDto.errors.First().message };
                        }
                        logger.Info("CourseController/Create - Task 'Create course' FINISHED");
                        return null;
                    }
                    logger.Info("CourseController/Create - Task 'Create course' FINISHED");
                    return null;
                }
                catch (WebException e)
                {
                    logger.Error("CourseController/Create - Task 'Create course' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                    {
                        HttpContext.Current.Request.Headers.Remove("Authorization");
                        HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                        return Create(courseDTO);
                    }

                    if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                    {
                        var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                        JObject obj = JsonConvert.DeserializeObject<JObject>(resp);
                        if (obj["errors"]["pseudonym"] != null)
                        {
                            foreach (var x in obj["errors"]["pseudonym"].First)
                            {
                                if (x.First["message"] != null)
                                    return new CourseReturn() { error_message = x.First["message"].ToString() };
                            }
                        }
                        return new CourseReturn() { error_message = e.ToString() };
                    }
                }
            }
            logger.Info("CourseController/Create - Task 'Create course' FINISHED");
            return null;
        }

        /// <summary>
        /// Verifica si el alumno tiene los datos actualizados en función a la cantidad de meses enviada
        /// </summary>
        /// <param name="username">Nombre de usuario del alumno</param>
        /// <param name="cantMeses">Cantidad de meses para verificar, 6 por defecto</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("Get")]
        [HttpGet]
        public object Get(int course_id)
        {
            string url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/courses/" + course_id;

            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                webReq.Method = "GET";
                webReq.Headers.Add(HttpRequestHeader.Authorization, HttpContext.Current.Request.Headers["Authorization"]);

                HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();

                Stream answer = webResponse.GetResponseStream();
                StreamReader _recivedAnswer = new StreamReader(answer);

                return JsonConvert.DeserializeObject<CourseDTO>(_recivedAnswer.ReadToEnd());
            }
            catch (Exception e)
            {
                if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                {
                    HttpContext.Current.Request.Headers.Remove("Authorization");
                    HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                    return Get(course_id);
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene un usuario
        /// </summary>
        /// <param name="sis_user_id">Identificador de usuario en el sistema</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("GetAll")]
        [HttpGet]
        public object GetAll(int per_page = 0)
        {
            string url = string.Empty;
            if (per_page != 0)
                url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/courses?per_page=" + per_page;
            else
                url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/courses";

            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                webReq.Method = "GET";
                webReq.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();

                Stream answer = webResponse.GetResponseStream();
                StreamReader _recivedAnswer = new StreamReader(answer);

                return JsonConvert.DeserializeObject<List<CourseDTO>>(_recivedAnswer.ReadToEnd());
            }
            catch (Exception e)
            {
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
        /// Obtiene un usuario
        /// </summary>
        /// <param name="sis_user_id">Identificador de usuario en el sistema</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("Sync")]
        [HttpGet]
        public void SyncToCanvas()
        {
            CourseService.SyncToCanvas();
        }
    }
}