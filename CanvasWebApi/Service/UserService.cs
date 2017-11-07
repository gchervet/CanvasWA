using CanvasWebApi.Common;
using CanvasWebApi.Controllers;
using CanvasWebApi.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Service
{
    public class UserService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void SyncToCanvas()
        {
            logger.Info("UserService/SyncToCanvas - Task 'Sync user' STARTED");

            try
            {
                SyncronizationDAL.SyncToCanvas();

                List<sp_get_uniCanvas_ws_usuarios_Result> userSyncList = UserDAL.SyncToCanvas();
                List<string> createdUserList = new List<string>();

                foreach (sp_get_uniCanvas_ws_usuarios_Result userSync in userSyncList)
                {
                    try
                    {
                        UserController userController = new UserController();
                        User user = new Common.User();
                        user.user = new Common.UserDTO(userSync);

                        if (!createdUserList.Any(x => x == userSync.Username))
                        {
                            UserReturn newUser = (UserReturn)userController.Create(user);

                            if (newUser != null)
                            {
                                UserDAL.UpdateCanvasData(userSync.IDAcademico, newUser);
                                createdUserList.Add(userSync.Username);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        UserReturn newUser = new UserReturn() { error_message = e.Message };
                        UserDAL.UpdateCanvasData(userSync.IDAcademico, newUser);
                    }
                }
                logger.Info("UserService/SyncToCanvas - Task 'Sync user' FINISHED");
            }
            catch (Exception e)
            {
                logger.Error("UserService/SyncToCanvas - Task 'Sync user' FINISHED WITH ERROR: \n " + "  Message: " + e.Message + "\nInner Exception: " + e.InnerException);
            }
        }
    }
}