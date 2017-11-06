using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CanvasWebApi.Data
{
    public class SyncronizationDAL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void SyncToCanvas()
        {
            logger.Info("SyncronizationDAL/SyncToCanvas - Task 'Sync user' STARTED");
            try
            {
                using (var context = new CANVAS_Model_Entities())
                {
                    int ciclo = Int32.Parse(WebConfigurationManager.AppSettings["CurrentYear"]);
                    int cuatri = Int32.Parse(WebConfigurationManager.AppSettings["CurrentQuarter"]);

                    context.sp_ins_uniCanvas_cursos_secciones();
                    logger.Info("SyncronizationDAL/SyncToCanvas - Task 'Sync user' INFO: CURSOS Y SECCIONES SINCRONIZADAS");
                    context.sp_ins_uniCanvas_usuarios_enrolamientos(ciclo, cuatri);
                    logger.Info("SyncronizationDAL/SyncToCanvas - Task 'Sync user' INFO: USUARIOS Y ENROLAMIENTOS SINCRONIZADOS");
                }
                logger.Info("SyncronizationDAL/SyncToCanvas - Task 'Sync user' FINISHED");
            }
            catch(Exception e)
            {
                logger.Error("SyncronizationDAL/SyncToCanvas - Task 'Sync user' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
            }
        }
    }
}