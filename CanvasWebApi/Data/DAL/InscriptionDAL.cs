using CanvasWebApi.Common;
using CanvasWebApi.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Data
{
    public class InscriptionDAL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static List<sp_uni_canvas_sincronizacion_Result> SyncToCanvas()
        {
            logger.Info("InscriptionDAL/SyncToCanvas - Task 'Sync inscription' STARTED");

            using (var context = new CANVAS_Model_Entities())
            {
                try
                {
                    List<sp_uni_canvas_sincronizacion_Result> rtn = context.sp_uni_canvas_sincronizacion().ToList();
                    logger.Info("InscriptionDAL/UpdateCanvasData - Task 'Sync inscription' FINISHED");
                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Error("InscriptionDAL/UpdateCanvasData - Task 'Sync inscription' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    return new List<sp_uni_canvas_sincronizacion_Result>();
                }
            }
        }

        internal static List<sp_get_uniCanvas_ws_enrolamientos_Result> InscriptionsToSync(string operation)
        {
            logger.Info("InscriptionDAL/InscriptionsToSync - Task 'Get the list of inscriptions to sync' STARTED");

            using (var context = new CANVAS_Model_Entities())
            {
                try
                {
                    int sentEnum = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                    List<sp_get_uniCanvas_ws_enrolamientos_Result> rtn = context.sp_get_uniCanvas_ws_enrolamientos(operation).ToList();
                    logger.Info("InscriptionDAL/UpdateCanvasData - Task 'Sync inscription' FINISHED");
                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Error("InscriptionDAL/InscriptionsToSync - Task 'Get the list of inscriptions to sync' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    return new List<sp_get_uniCanvas_ws_enrolamientos_Result>();
                }

            }
        }

        public static void UpdateCanvasData(int? idEntidad, InscriptionReturn newInscription)
        {
            logger.Info("InscriptionDAL/UpdateCanvasData - Task 'Update inscription data in Canvas' STARTED");

            if (newInscription != null)
            {
                using (var context = new CANVAS_Model_Entities())
                {
                    uniCanvasEnrolamiento newCanvasInscription = context.uniCanvasEnrolamientos.Where(x => x.ID == idEntidad).FirstOrDefault();
                    if (newInscription.error_message == null)
                    {
                        newCanvasInscription.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                        newCanvasInscription.Fecha = DateTime.Now;
                        newCanvasInscription.IDCanvas = Int32.Parse(newInscription.id);
                    }
                    else
                        newCanvasInscription.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Error.GetHashCode();

                    newCanvasInscription.Error = newInscription.error_message;
                    context.SaveChanges();
                }
            }
            logger.Info("InscriptionDAL/UpdateCanvasData - Task 'Update inscription data in Canvas' FINISHED");
        }
    }
}