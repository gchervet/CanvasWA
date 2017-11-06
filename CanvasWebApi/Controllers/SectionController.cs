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
    [RoutePrefix("api/Section")]
    public class SectionController : ApiController
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
        public object Create([FromBody] Section sectionDTO, string sis_course_id, Nullable<int> course_id = 0)
        {
            logger.Info("SectionController/Create - Task 'Create section' STARTED");

            if (sectionDTO != null)
            {
                string url = string.Empty;

                if (course_id != 0 && sis_course_id == null)
                    url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/courses/" + course_id + "/sections";
                if (sis_course_id != null && (course_id == 0 || course_id == null))
                    url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/courses/sis_course_id:" + sis_course_id + "/sections";

                try
                {
                    WebRequest request = WebRequest.Create(url);
                    request.Method = "POST";

                    request.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                    string postData = "{\"course_section\": {\"sis_course_id\" : \"" + sectionDTO.course_section.sis_course_id +
                                        "\",\"name\" : \"" + sectionDTO.course_section.name +
                                        "\",\"sis_section_id\":\"" + sectionDTO.course_section.sis_section_id +
                                        "\",\"isolate_section\": false}}";

                    //string postData = new JavaScriptSerializer().Serialize(sectionDTO);
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
                    object rtn = JsonConvert.DeserializeObject<SectionDTO>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        logger.Info("SectionController/Create - Task 'Create section' FINISHED");
                        return rtn;
                    }

                    rtn = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        ErrorMessage errorMessageDto = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                        if (errorMessageDto != null)
                        {
                            logger.Info("SectionController/Create - Task 'Create section' FINISHED");
                            return new SectionDTO() { error_message = errorMessageDto.errors.First().message };
                        }
                        logger.Info("SectionController/Create - Task 'Create section' FINISHED");
                        return null;
                    }
                    logger.Info("SectionController/Create - Task 'Create section' FINISHED");
                    return null;
                }
                catch (WebException e)
                {
                    logger.Error("SectionController/Create - Task 'Create section' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                    {
                        HttpContext.Current.Request.Headers.Remove("Authorization");
                        HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                        return Create(sectionDTO, sis_course_id, course_id);
                    }
                    if (e.Message.Contains(HttpStatusCode.BadRequest.GetHashCode().ToString()))
                    {
                        var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                        JObject obj = JsonConvert.DeserializeObject<JObject>(resp);
                        if (obj["errors"]["pseudonym"] != null)
                        {
                            foreach (var x in obj["errors"].First)
                            {
                                if (x.First["message"] != null)
                                    return new SectionDTO() { error_message = x.First["message"].ToString() };
                            }
                        }
                        return new SectionDTO() { error_message = e.ToString() };
                    }
                    return new SectionDTO() { error_message = e.ToString() };
                }
            }
            logger.Info("SectionController/Create - Task 'Create section' FINISHED");
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
        public object Get(int sis_user_id)
        {
            return null;
        }

        /// <summary>
        /// Sincroniza los usuarios de Académico con los de Canvas
        /// </summary>
        [Route("Sync")]
        [HttpPost]
        public void SyncToCanvas()
        {
            logger.Info("SectionController/SyncToCanvas - Task 'Sync section' STARTED");

            SectionService.SyncToCanvas();

            logger.Info("SectionController/SyncToCanvas - Task 'Sync section' FINISHED");
        }
    }
}