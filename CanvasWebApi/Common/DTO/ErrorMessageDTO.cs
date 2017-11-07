using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Common
{
    public class ErrorMessage
    {
        public ErrorMessageDTO[] errors { get; set; }
        public int error_report_id { get; set; }
    }

    public class ErrorMessageDTO
    {
        public string message { get; set; }
        public string error_code { get; set; }
    }
}