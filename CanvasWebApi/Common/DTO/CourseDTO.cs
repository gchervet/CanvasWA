using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CanvasWebApi.Data;
using Newtonsoft.Json;

namespace CanvasWebApi.Common
{
    public class Course
    {
        [JsonConstructor]
        public Course() { }

        public CourseDTO course { get; set; }
    }

    public class CourseReturn
    {
        [JsonConstructor]
        public CourseReturn() { }

        public string id { get; set; }
        public string sis_course_id { get; set; }
        public string error_message { get; set; }
    }

    public class CourseDTO
    {
        [JsonConstructor]
        public CourseDTO() { }

        public CourseDTO(sp_get_uniCanvas_ws_cursos_Result courseToSync)
        {
            account_id = courseToSync.IDCanvasCuenta;
            sis_course_id = courseToSync.IDAcademico;
            name = courseToSync.Nombre;
            term_id = "term";
            code = String.Format("{0} ({1})", courseToSync.Nombre, courseToSync.IDAcademico);
            sis_master_id = courseToSync.IDAcademicoMaster;
            import_content = courseToSync.ImportarContenido;
            term_id = courseToSync.Termino;
            end_at = courseToSync.FechaFin;
            start_at = courseToSync.FechaInicio;
        }

        public Nullable<int> account_id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public Nullable<DateTime> start_at { get; set; }
        public Nullable<DateTime> end_at { get; set; }
        public string term_id { get; set; }
        public string sis_course_id { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public Nullable<bool> import_content { get; set; }
        public string sis_master_id { get; set; }
    }

    public class CourseToConcludeDTO
    {
        [JsonConstructor]
        public CourseToConcludeDTO() { }

        public CourseToConcludeDTO(sp_get_uniCanvas_ws_cursos_Result courseToConcludeModel)
        {
            IDAcademicoCurso = courseToConcludeModel.IDAcademico;
        }

        public string IDAcademicoCurso { get; set; }
    }
}