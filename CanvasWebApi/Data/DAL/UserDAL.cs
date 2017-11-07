using CanvasWebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace CanvasWebApi.Data
{
    public class UserDAL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static List<sp_get_uniCanvas_ws_usuarios_Result> SyncToCanvas()
        {
            logger.Info("UserDAL/SyncToCanvas - Task 'Sync user' STARTED");

            using (var context = new CANVAS_Model_Entities())
            {
                try
                {
                    List<sp_get_uniCanvas_ws_usuarios_Result> rtn = context.sp_get_uniCanvas_ws_usuarios().ToList();
                    logger.Info("UserDAL/SyncToCanvas - Task 'Sync user' FINISHED");
                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Error("UserDAL/SyncToCanvas - Task 'Sync user' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    return new List<sp_get_uniCanvas_ws_usuarios_Result>();
                }
            }
        }

        public static void UpdateCanvasData(int? idEntidad, UserReturn newUser)
        {
            logger.Info("UserDAL/UpdateCanvasData - Task 'Update user data from Canvas' STARTED");
            if (newUser != null)
            {
                using (var context = new CANVAS_Model_Entities())
                {
                    uniCanvasUsuario newCanvasUser = context.uniCanvasUsuarios.Where(x => x.IDAcademico == idEntidad).FirstOrDefault();
                    if (newUser.error_message == null)
                    {
                        newCanvasUser.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                        newCanvasUser.Fecha = DateTime.Now;
                        newCanvasUser.IDCanvas = Int32.Parse(newUser.id);
                    }
                    else
                        newCanvasUser.Estado = CanvasWebApi.Common.ConfigEnum.CanvasState.Error.GetHashCode();

                    newCanvasUser.Error = newUser.error_message;
                    context.SaveChanges();
                }
                logger.Info("UserDAL/UpdateCanvasData - Task 'Update user data from Canvas' FINISHED");
            }
            logger.Info("UserDAL/UpdateCanvasData - Task 'Update user data from Canvas' FINISHED");
        }

        internal static List<uniCanvasUsuario> UsersToSync()
        {
            logger.Info("UserDAL/UsersToSync - Task 'Get the list of users to sync' STARTED");

            using (var context = new CANVAS_Model_Entities())
            {
                try
                {
                    int sentEnum = CanvasWebApi.Common.ConfigEnum.CanvasState.Sincronizado.GetHashCode();
                    List<uniCanvasUsuario> rtn = context.uniCanvasUsuarios.Where(x => x.Estado != sentEnum).ToList();
                    logger.Info("UserDAL/SyncToCanvas - Task 'Get the list of users to sync' FINISHED");
                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Error("UserDAL/UsersToSync - Task 'Get the list of users to sync' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
                    return new List<uniCanvasUsuario>();
                }
            }
        }
    }
}