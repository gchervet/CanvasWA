using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/// <summary> 
/// Contiene Tipos para realizar llamadas a clientes REST
/// </summary>
namespace CommonLib.ClientHTTP
{
    /// <summary>
    /// Representa excepción que se lanza al obtener una respuesta con un código de estado de error
    /// </summary>
    [Serializable]
    public class ResponseStatusUnsuccessfulException : Exception
    {
        /// <summary>
        /// Inicializa la clase
        /// </summary>
        public ResponseStatusUnsuccessfulException()
            : base("La respuesta obtuvo un codigo de estado incorrecto")
        {
        }

        /// <summary>
        /// Inicializa la clase
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public ResponseStatusUnsuccessfulException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa la clase
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="innerException">Excepción anterior</param>
        public ResponseStatusUnsuccessfulException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}