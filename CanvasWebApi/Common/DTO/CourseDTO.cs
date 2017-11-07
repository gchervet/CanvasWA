using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CanvasWebApi.Data;

namespace CanvasWebApi.Common
{
    public class Course
    {
        public CourseDTO course { get; set; }
    }

    public class CourseReturn
    {
        public string id { get; set; }
        public string sis_course_id { get; set; }
        public string error_message { get; set; }
    }

    public class CourseDTO
    {
        public CourseDTO(sp_get_uniCanvas_ws_cursos_Result courseToSync)
        {
            account_id = 1;
            sis_course_id = courseToSync.IDAcademico;
            name = courseToSync.Nombre;
            term_id = "term";
            code = courseToSync.IDAcademico;
        }

        public int account_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Nullable<DateTime> start_at { get; set; }
        public Nullable<DateTime> end_at { get; set; }
        public string term_id { get; set; }
        public string sis_course_id { get; set; }
        public string id { get; set; }
        public string status { get; set; }
    }
}