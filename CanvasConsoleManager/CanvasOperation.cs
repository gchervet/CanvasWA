using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace CanvasConsoleManager
{
    public class CanvasOperation
    {
        public bool SyncToCanvas(string entity)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(WebConfigurationManager.AppSettings["BASE_URL"] + entity + "/sync");
                request.Method = "POST";
                request.Headers.Add(HttpRequestHeader.Authorization, WebConfigurationManager.AppSettings["AUTH_HEADER"]);

                request.ContentLength = 0;
                request.ContentType = "application/json";

                request.GetResponse();

                return true;
            }
            catch (WebException ex)
            {
                return false;
            }
        }
    }
}
