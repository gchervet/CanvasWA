using CanvasWebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Common
{
    [Serializable]
    public class Inscription
    {
        public InscriptionDTO enrollment { get; set; }
    }
    
    public class InscriptionReturn
    {
        public string id { get; set; }
        public string sis_section_id { get; set; }
        public string sis_course_id { get; set; }
        public string sis_user_id { get; set; }
        public string user_id { get; set; }
        public string error_message { get; set; }
    }

    [Serializable]
    public class InscriptionDTO
    {
        public InscriptionDTO(sp_get_uniCanvas_ws_enrolamientos_Result inscriptionResult)
        {
            sis_user_id = inscriptionResult.IDAcademicoUsuario.ToString();
            send_notification = true;
            sis_section_id = inscriptionResult.IDAcademicoSeccion.ToString();
            section_id = inscriptionResult.IDCanvasSeccion.ToString();
            user_id = inscriptionResult.IDCanvasUsuario.ToString();
            type = "StudentEnrollment";
        }

        public string user_id { get; set; }
        public string type { get; set; }
        public string sis_section_id { get; set; }
        public bool send_notification { get; set; }
        public int group_id { get; set; }
        public string id { get; set; }
        public string sis_course_id { get; set; }
        public string sis_user_id { get; set; }
        public string section_id { get; set; }
        public string course_id { get; set; }
        public string last_activity_at { get; set; }
        public string current_score { get; set; }
        public string final_score { get; set; }
        public string current_grade { get; set; }
        public string final_grade { get; set; }
        public string enrollment_state { get; set; }
        public Nullable<DateTime> start_at { get; set; }
        public Nullable<DateTime> end_at { get; set; }
    }
}