//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CanvasWebApi.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class uniCanvasEnrolamiento
    {
        public long ID { get; set; }
        public Nullable<int> IDAcademicoUsuario { get; set; }
        public Nullable<long> IDAcademicoSeccion { get; set; }
        public Nullable<int> IDCanvas { get; set; }
        public string Operacion { get; set; }
        public Nullable<int> Estado { get; set; }
        public string Error { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
    }
}
