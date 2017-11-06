using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CanvasWebApi.Data;
using Newtonsoft.Json;

namespace CanvasWebApi.Common
{
    public class User
    {
        public UserDTO user { get; set; }
    }

    public class UserReturn
    {
        public string id { get; set; }
        public string login { get; set; }
        public string error_message { get; set; }
    }

    public class UserDTO
    {
        [JsonConstructor]
        public UserDTO() { }

        public UserDTO(sp_get_uniCanvas_ws_usuarios_Result userSync)
        {
            sis_user_id = userSync.IDAcademico.ToString();
            email = userSync.EMail;
			short_name = userSync.Nombre;
            sortable_name = userSync.Apellido;
            full_name = userSync.Nombre + " " + userSync.Apellido;
            //unique_id = userSync.Username;
            login = userSync.Username;
        }

        public string id { get; set; }
        public string email { get; set; }
        public string last_login { get; set; }
        public string full_name { get; set; }
        public string short_name { get; set; }
        public string sortable_name { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string sis_user_id { get; set; }
        public string name { get; set; }
        public string unique_id { get; set; }
    }
}