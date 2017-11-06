using CanvasWebApi.Common;
using CanvasWebApi.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Data
{
    public class SectionDAL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static List<sp_uni_canvas_sincronizacion_Result> SyncToCanvas()
        {
            logger.Info("SectionDAL/SyncToCanvas - Task 'Sync section' STARTED");
            using (var context = new CANVAS_Model_Entities())
            {
                try
                {
                    List<sp_uni_canvas_sincronizacion_Result> rtn = context.sp_uni_canvas_sincronizacion().ToList();
                    logger.Info("SectionDAL/SyncToCanvas - Task 'Sync section' FINISHED");
                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Error("SectionDAL/SyncToCanvas - Task 'Sync section' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    return new List<sp_uni_canvas_sincronizacion_Result>();
                }
            }
        }

        internal static List<sp_get_uniCanvas_ws_secciones_Result> SectionsToSync()
        {
            logger.Info("SectionDAL/SectionsToSync - Task 'Get a list with the sections to sync' STARTED");
            using (var context = new CANVAS_Model_Entities())
            {
                try
                {
                    int sentEnum = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                    List<sp_get_uniCanvas_ws_secciones_Result> rtn = context.sp_get_uniCanvas_ws_secciones().ToList();
                    logger.Info("SectionDAL/SectionsToSync - Task 'Get a list with the sections to sync' FINISHED");
                    return rtn;

                }
                catch (Exception e)
                {
                    logger.Error("SectionDAL/SectionsToSync - Task 'Get a list with the sections to sync' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    return new List<sp_get_uniCanvas_ws_secciones_Result>();
                }
            }
        }

        public static void UpdateCanvasData(int? idEntidad, SectionDTO newSection)
        {
            logger.Info("SectionDAL/SectionsToSync - Task 'Get a list with the sections to sync' STARTED");
            if (newSection != null)
            {
                using (var context = new CANVAS_Model_Entities())
                {
                    uniCanvasCursosSeccione newCanvasCourseSection = context.uniCanvasCursosSecciones.Where(x => x.IDAcademico == idEntidad).FirstOrDefault();
                    if (newSection.error_message == null)
                    {
                        newCanvasCourseSection.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                        newCanvasCourseSection.Fecha = DateTime.Now;
                        newCanvasCourseSection.IDCanvas = Int32.Parse(newSection.id);
                    }
                    else
                        newCanvasCourseSection.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Error.GetHashCode();

                    newCanvasCourseSection.Error = newSection.error_message;
                    context.SaveChanges();
                }
            }
        }
    }
}