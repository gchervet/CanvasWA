using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasConsoleManager
{
    class Program
    {
        static void Main(string[] args)
        {
            CanvasOperation canvasOperation = new CanvasOperation();
            try
            {
                Console.WriteLine("CanvasWA iniciado");
                Console.WriteLine();
                Console.WriteLine("*********************************************");
                Console.WriteLine();

                ////1. Alta cursos
                //Console.WriteLine("1.   Alta cursos");
                //bool resultCourses = canvasOperation.SyncToCanvas("course");
                //Console.WriteLine("1.   Alta cursos finalizada {0} errores", resultCourses ? "sin" : "con");
                //
                //Console.WriteLine("*********************************************");
                //Console.WriteLine();
                //
                ////2. Alta secciones
                //Console.WriteLine("2.   Alta secciones");
                //bool resultSections = canvasOperation.SyncToCanvas("section");
                //Console.WriteLine("2.   Alta secciones finalizada {0} errores", resultSections ? "sin" : "con");
                //
                //Console.WriteLine("*********************************************");
                //Console.WriteLine();

                //3. Alta usuarios
                Console.WriteLine("1.   Alta usuarios");
                bool resultUsers = canvasOperation.SyncToCanvas("user");
                Console.WriteLine("1.   Alta usuarios finalizada {0} errores", resultUsers ? "sin" : "con");

                Console.WriteLine("*********************************************");
                Console.WriteLine();

                //4. Alta enrolamientos
                Console.WriteLine("2.   Alta enrolamientos");
                bool resultEnrollments = canvasOperation.SyncToCanvas("inscription");
                Console.WriteLine("2.   Alta enrolamientos finalizada {0} errores", resultEnrollments ? "sin" : "con");

                Console.WriteLine("*********************************************");
                Console.WriteLine();

                Console.WriteLine("CanvasWA finalizado");
            }
            catch (Exception e)
            {
                Console.WriteLine("*********************************************");
                Console.WriteLine();
                Console.WriteLine("CanvasWA error:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}
