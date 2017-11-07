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
    public class SectionService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void SyncToCanvas()
        {
            logger.Info("SectionService/SyncToCanvas - Task 'Sync section' STARTED");

            try
            {
                SyncronizationDAL.SyncToCanvas();

                List<sp_get_uniCanvas_ws_secciones_Result> sectionToSyncList = SectionDAL.SectionsToSync();

                foreach (sp_get_uniCanvas_ws_secciones_Result sectionToSync in sectionToSyncList)
                {
                    try
                    {
                        SectionController sectionController = new SectionController();
                        Section section = new Section();
                        
                        section.course_section = new SectionDTO(sectionToSync);

                        SectionDTO newSection = (SectionDTO)sectionController.Create(section, section.course_section.sis_course_id);

                        if (newSection != null)
                        {
                            SectionDAL.UpdateCanvasData((int)sectionToSync.IDAcademico, newSection);
                        }
                        logger.Info("SectionService/SyncToCanvas - Task 'Sync section' FINISHED");
                    }
                    catch (Exception e)
                    {
                        logger.Error("SectionService/SyncToCanvas - Task 'Sync section' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                        SectionDTO newSection = new SectionDTO() { error_message = e.Message };
                        SectionDAL.UpdateCanvasData((int)sectionToSync.IDAcademico, newSection);
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}