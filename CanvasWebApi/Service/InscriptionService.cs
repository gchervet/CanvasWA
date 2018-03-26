using CanvasWebApi.Common;
using CanvasWebApi.Controllers;
using CanvasWebApi.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Service
{
    public class InscriptionService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void SyncToCanvas()
        {
            logger.Info("InscriptionService/SyncToCanvas - Task 'Sync inscription' STARTED");

            try
            {
                SyncronizationDAL.SyncToCanvas();

                foreach (CanvasWebApi.Common.ConfigEnum.Enrollment_Operation enrollmentOperation in Enum.GetValues(typeof(CanvasWebApi.Common.ConfigEnum.Enrollment_Operation)))
                {
                    //PRIMERO SE DAN LAS ALTAS Y LUEGO LAS BAJAS
                    List<sp_get_uniCanvas_ws_enrolamientos_Result> inscriptionToSyncList = InscriptionDAL.InscriptionsToSync(enrollmentOperation.ToString());

                    foreach (sp_get_uniCanvas_ws_enrolamientos_Result inscriptionToSync in inscriptionToSyncList)
                    {
                        try
                        {
                            InscriptionController inscriptionController = new InscriptionController();
                            Inscription inscription = new Inscription();

                            inscription.enrollment = new InscriptionDTO(inscriptionToSync);

                            if (enrollmentOperation == CanvasWebApi.Common.ConfigEnum.Enrollment_Operation.A)
                            {
                                InscriptionReturn newInscription = (InscriptionReturn)inscriptionController.CreateBySectionId(inscription, inscriptionToSync.IDAcademicoSeccion.ToString(), inscriptionToSync.IDCanvasSeccion);

                                if (newInscription != null)
                                {
                                    InscriptionDAL.UpdateCanvasData((int)inscriptionToSync.ID, newInscription);
                                }
                            }
                            else if (enrollmentOperation == CanvasWebApi.Common.ConfigEnum.Enrollment_Operation.B)
                            {
                                InscriptionReturn newInscription = (InscriptionReturn)inscriptionController.DeleteInscription(inscriptionToSync.IDAcademicoCurso, inscriptionToSync.IDCanvasEnrolamiento);

                                if (newInscription != null)
                                {
                                    InscriptionDAL.UpdateCanvasData((int)inscriptionToSync.ID, newInscription);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            InscriptionReturn newInscription = new InscriptionReturn() { error_message = e.Message };
                            InscriptionDAL.UpdateCanvasData((int)inscriptionToSync.ID, newInscription);
                        }
                    }
                    logger.Info("InscriptionService/SyncToCanvas - Task 'Sync inscription' FINISHED");
                }
            }
            catch (Exception e)
            {
                logger.Error("SyncronizationDAL/SyncToCanvas - Task 'Sync user' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                return;
            }
        }
    }
}