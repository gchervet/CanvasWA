using CanvasWebApi.App_Start;
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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CanvasWebApi.Controllers
{
    [RoutePrefix("api/Inscription")]
    public class InscriptionController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public HttpWebResponse webResponse = null;
        /// <summary>
        /// Verifica si el alumno tiene los datos actualizados en función a la cantidad de meses enviada
        /// </summary>
        /// <param name="username">Nombre de usuario del alumno</param>
        /// <param name="cantMeses">Cantidad de meses para verificar, 6 por defecto</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("Create")]
        [HttpPost]
        public object CreateBySectionId([FromBody]Inscription inscriptionDTO, string sis_section_id, Nullable<int> section_id = 0)
        {
            logger.Info("InscriptionService/CreateBySectionId - Task 'Create inscription' STARTED");

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            if (section_id != 0 || sis_section_id != null)
            {
                string url = string.Empty;

                url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/sections/sis_section_id:" + sis_section_id + "/enrollments";

                try
                {
                    string jsonStr = "{\"enrollment\":{\"user_id\":\"" + inscriptionDTO.enrollment.user_id +
                                     "\",\"type\":\"StudentEnrollment\",\"sis_section_id\":\"" + inscriptionDTO.enrollment.sis_section_id +
                                     "\",\"state\":\"active\",\"send_notification\":false}}";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                    byte[] formData = UTF8Encoding.UTF8.GetBytes(jsonStr);

                    request.ContentLength = formData.Length;
                    request.ContentType = "application/json";

                    using (Stream post = request.GetRequestStream())
                    {
                        post.Write(formData, 0, formData.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream);
                    string contents = reader.ReadToEnd();


                    object rtn = JsonConvert.DeserializeObject<InscriptionReturn>(contents);
                    if (rtn != null)
                    {
                        logger.Info("InscriptionService/CreateBySectionId - Task 'Create inscription' FINISHED");
                        return rtn;
                    }

                    rtn = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                    if (rtn != null)
                    {
                        ErrorMessage errorMessageDto = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                        if (errorMessageDto != null)
                        {
                            logger.Info("InscriptionService/CreateBySectionId - Task 'Create inscription' FINISHED");
                            return new InscriptionReturn() { error_message = errorMessageDto.errors.First().message };
                        }
                        logger.Info("InscriptionService/CreateBySectionId - Task 'Create inscription' FINISHED");
                        return null;
                    }
                    logger.Info("InscriptionService/CreateBySectionId - Task 'Create inscription' FINISHED");
                    return null;
                }
                catch (WebException e)
                {
                    logger.Error("InscriptionService/CreateBySectionId - Task 'Create inscription' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                    {
                        HttpContext.Current.Request.Headers.Remove("Authorization");
                        HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                        return CreateBySectionId(inscriptionDTO, sis_section_id, section_id);
                    }
                    string errorMessage = Extensions.SearchValueInJObject((JObject)e.Data, "message");

                    if (errorMessage != null)
                        return new InscriptionReturn() { error_message = errorMessage };
                    else
                        return new InscriptionReturn() { error_message = e.ToString() };
                }
            }
            logger.Info("InscriptionService/CreateBySectionId - Task 'Create inscription' FINISHED");
            return null;
        }

        public InscriptionReturn InactivateInscription(long? iDAcademicoSeccion, int? iDCanvasEnrolamiento)
        {
            logger.Info("InscriptionService/InactivateInscription - Task 'Inactivate inscription' STARTED");

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/courses/sis_course_id:" + iDAcademicoSeccion + "/enrollments/" + iDCanvasEnrolamiento;

            try
            {
                WebRequest request = WebRequest.Create(WebConfigurationManager.AppSettings["BASE_URL"] + url);
                request.Method = "DELETE";

                request.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                string postData = new JavaScriptSerializer().Serialize("{ \"action\" : \"delete\",\"enrollment:id\":" + iDCanvasEnrolamiento + ", \"sis_course_id\":\"" + iDAcademicoSeccion + "\"}");
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
                object rtn = JsonConvert.DeserializeObject<InscriptionReturn>(reader.ReadToEnd());
                if (rtn != null)
                {
                    logger.Info("InscriptionService/InactivateInscription - Task 'Inactivate inscription' FINISHED");
                    return (InscriptionReturn)rtn;
                }

                rtn = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                if (rtn != null)
                {
                    ErrorMessage errorMessageDto = JsonConvert.DeserializeObject<ErrorMessage>(reader.ReadToEnd());
                    if (errorMessageDto != null)
                    {
                        logger.Info("InscriptionService/InactivateInscription - Task 'Inactivate inscription' FINISHED");
                        return new InscriptionReturn() { error_message = errorMessageDto.errors.First().message };
                    }
                    logger.Info("InscriptionService/InactivateInscription - Task 'Inactivate inscription' FINISHED");
                    return null;
                }
                logger.Info("InscriptionService/InactivateInscription - Task 'Inactivate inscription' FINISHED");
                return null;
            }
            catch (WebException e)
            {
                logger.Error("InscriptionService/InactivateInscription - Task 'Inactivate inscription' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                {
                    HttpContext.Current.Request.Headers.Remove("Authorization");
                    HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                    return InactivateInscription(iDAcademicoSeccion, iDCanvasEnrolamiento);
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
                                return new InscriptionReturn() { error_message = x.First["message"].ToString() };
                        }
                    }
                    return new InscriptionReturn() { error_message = e.ToString() };
                }
            }
            logger.Info("InscriptionService/InactivateInscription - Task 'Inactivate inscription' FINISHED");
            return null;
        }

        /// <summary>
        /// Verifica si el alumno tiene los datos actualizados en función a la cantidad de meses enviada
        /// </summary>
        /// <param name="username">Nombre de usuario del alumno</param>
        /// <param name="cantMeses">Cantidad de meses para verificar, 6 por defecto</param>
        /// <returns>True si la información está actualizada. False en caso contrario</returns>
        [Route("GetBySectionId")]
        [HttpGet]
        public object GetBySectionId(string sis_section_id, Nullable<int> section_id = 0)
        {
            logger.Info("InscriptionService/InactivateInscription - Task 'Get inscription by section Id' STARTED");

            if (section_id != 0 || sis_section_id != null)
            {
                string url = string.Empty;

                if (section_id != 0 && (sis_section_id == "null" || sis_section_id == null))
                    url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/sections/" + section_id + "/enrollments";
                if (sis_section_id != null && (section_id == 0 || section_id == null))
                    url = WebConfigurationManager.AppSettings["BASE_URL"] + "api/lms/v1/sections/sis_section_id:" + sis_section_id + "/enrollments";

                try
                {
                    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                    webReq.Method = "GET";
                    webReq.Headers.Add(HttpRequestHeader.Authorization, SessionController.GetToken());

                    webResponse = (HttpWebResponse)webReq.GetResponse();

                    Stream answer = webResponse.GetResponseStream();
                    StreamReader _recivedAnswer = new StreamReader(answer);

                    logger.Info("InscriptionService/InactivateInscription - Task 'Get inscription by section Id' FINISHED");
                    return JsonConvert.DeserializeObject<List<InscriptionDTO>>(_recivedAnswer.ReadToEnd());
                }
                catch (Exception e)
                {
                    logger.Error("InscriptionService/InactivateInscription - Task 'Get inscription by section Id' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    if (e.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
                    {
                        HttpContext.Current.Request.Headers.Remove("Authorization");
                        HttpContext.Current.Request.Headers.Add("Authorization", SessionController.GetToken());
                        return GetBySectionId(sis_section_id, section_id);
                    }
                    return e.ToString();
                }
            }
            logger.Info("InscriptionService/InactivateInscription - Task 'Get inscription by section Id' FINISHED");
            return null;
        }

        /// <summary>
        /// Sincroniza los usuarios de Académico con los de Canvas
        /// </summary>
        [Route("Sync")]
        [HttpPost]
        public void SyncToCanvas()
        {
            logger.Info("InscriptionService/SyncToCanvas - Task 'Sync inscription' STARTED");

            InscriptionService.SyncToCanvas();

            logger.Info("InscriptionService/SyncToCanvas - Task 'Sync inscription' FINISHED");
        }
    }
}