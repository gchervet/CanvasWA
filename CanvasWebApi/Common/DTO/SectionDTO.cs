using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CanvasWebApi.Data;

namespace CanvasWebApi.Common
{
    public class Section
    {
        public SectionDTO course_section { get; set; }
    }

    public class SectionDTO
    {
        public SectionDTO() { }

        public SectionDTO(sp_get_uniCanvas_ws_secciones_Result sectionToSync)
        {
            name = sectionToSync.Nombre;
            sis_course_id = sectionToSync.IDAcademicoCurso.ToString();
            sis_section_id = sectionToSync.IDAcademico.ToString();
            isolate_section = false;
        }

        public string id { get; set; }
        public string sis_section_id { get; set; }
        public string sis_course_id { get; set; }
        public string sis_user_id { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
        public bool isolate_section { get; set; }
        public string error_message { get; set; }
    }
}