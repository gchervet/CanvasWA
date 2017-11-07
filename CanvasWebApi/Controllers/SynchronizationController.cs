using CanvasWebApi.Common;
using CanvasWebApi.Data;
using CanvasWebApi.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    [RoutePrefix("api/Sync")]
    public class SynchronizationController : ApiController
    {
        /// <summary>
        /// Sincroniza la información con la base de CANVAS
        /// </summary>
        [Route("SyncToCanvas")]
        [HttpGet]
        public void SyncToCanvas()
        {
            CourseService.SyncToCanvas();
            SectionService.SyncToCanvas();
            UserService.SyncToCanvas();
            InscriptionService.SyncToCanvas();
        }
    }
}