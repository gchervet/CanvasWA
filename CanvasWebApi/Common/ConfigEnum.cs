using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Common
{
    public class ConfigEnum
    {
        public enum CanvasState
        {
            NoEnviado = 0,
            Sincronizado = 1,
            Error = 2
        }

        public enum Enrollment_Operation
        {
            A = 0, 
            B = 1
        }
    }
}