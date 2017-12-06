using CanvasWebApi.Common;
using CanvasWebApi.Controllers;
using CanvasWebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanvasWebApi.Service
{
    public class CourseService
    {
        public static void SyncToCanvas()
        {
            try
            {
                SyncronizationDAL.SyncToCanvas();

                List<sp_get_uniCanvas_ws_cursos_Result> courseToSyncList = CourseDAL.CoursesToSync();
                List<string> createdCoursesList = new List<string>();

                foreach (sp_get_uniCanvas_ws_cursos_Result courseToSync in courseToSyncList)
                {
                    try
                    {
                        CourseController courseController = new CourseController();
                        Course course = new Course();
                        course.course = new CourseDTO(courseToSync);

                        CourseReturn newCourse = (CourseReturn)courseController.Create(course);

                        if (newCourse != null)
                        {
                            CourseDAL.UpdateCanvasData(courseToSync.IDAcademico.ToString(), newCourse);
                        }
                    }
                    catch (Exception e)
                    {
                        CourseReturn newCourse = new CourseReturn() { error_message = e.Message };
                        CourseDAL.UpdateCanvasData(courseToSync.IDAcademico.ToString(), newCourse);
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        internal static List<CourseToConcludeDTO> GetCourseToConcludeList()
        {
            try
            {
                SyncronizationDAL.SyncToCanvas();
                List<CourseToConcludeDTO> rtn = new List<CourseToConcludeDTO>();

                List<sp_get_uniCanvas_ws_cursos_Result> courseToConcludeList = CourseDAL.CoursesToConclude();
                foreach (sp_get_uniCanvas_ws_cursos_Result courseToConclude in courseToConcludeList)
                {
                    rtn.Add(new CourseToConcludeDTO(courseToConclude));    
                }
                return rtn;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        internal static void UpdateConcludedCourseCanvasData(string iDAcademicoCurso)
        {
            if(iDAcademicoCurso != null)
            {
                CourseDAL.UpdateCoursesConcludedCanvasData(iDAcademicoCurso);
            }
        }
    }
}