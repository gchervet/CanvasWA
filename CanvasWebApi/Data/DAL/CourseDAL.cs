using CanvasWebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Data
{
    public class CourseDAL
    {
        public static List<sp_uni_canvas_sincronizacion_Result> SyncToCanvas()
        {
            using (var context = new CANVAS_Model_Entities())
            {
                return context.sp_uni_canvas_sincronizacion().ToList();
            }
        }

        internal static List<sp_get_uniCanvas_ws_cursos_Result> CoursesToSync()
        {
            using (var context = new CANVAS_Model_Entities())
            {
                int sentEnum = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                return context.sp_get_uniCanvas_ws_cursos().ToList();
            }
        }

        internal static string GetSubjectName(string id)
        {
            using (var context = new CANVAS_Model_Entities())
            {
                uniMateria subject = context.uniMaterias.Where(x => x.codmat == id).FirstOrDefault();

                if(subject != null)
                    return subject.nombre;

                return null;
            }
        }

        public static void UpdateCanvasData(string idEntidad, CourseReturn newCourse)
        {
            if (newCourse != null)
            {
                using (var context = new CANVAS_Model_Entities())
                {
                    uniCanvasCurso newCanvasCourse = context.uniCanvasCursos.Where(x => x.IDAcademico == idEntidad).FirstOrDefault();
                    if (newCourse.error_message == null)
                    {
                        newCanvasCourse.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                        newCanvasCourse.Fecha = DateTime.Now;
                        newCanvasCourse.IDCanvas = Int32.Parse(newCourse.id);
                    }
                    else
                        newCanvasCourse.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Error.GetHashCode();

                    newCanvasCourse.Error = newCourse.error_message;
                    context.SaveChanges();
                }
            }
        }

        //TODO: Obtener los datos desde la SP de cursos a concluir y  modificar el circuito desde el controlador hasta la DAL
        internal static List<sp_get_uniCanvas_ws_cursos_Result> CoursesToConclude()
        {
            throw new NotImplementedException();
        }

        public static void UpdateCoursesConcludedCanvasData(string iDAcademicoCurso)
        {
            using (var context = new CANVAS_Model_Entities())
            {
                uniCanvasCurso uniCanvasCursoModel = context.uniCanvasCursos.Where(x => x.IDAcademico == iDAcademicoCurso).FirstOrDefault();

                if (uniCanvasCursoModel != null)
                {
                    //uniCanvasCursoModel.Concluido = true;
                    context.SaveChanges();
                }
                else
                {
                    //TODO: Loguear la falta del curso en la staging
                }
            }
        }
    }
}