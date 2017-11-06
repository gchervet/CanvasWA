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
    /// Representa excepción que se lanza al no poder realizar la consulta
    /// </summary>
    [Serializable]
    public class SecureHttpRequestException : Exception
    {
        /// <summary>
        /// Inicializa la clase
        /// </summary>
        public SecureHttpRequestException()
            : base("Ocurrió un error al realizar la petición http.")
        {
        }

        /// <summary>
        /// Inicializa la clase
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public SecureHttpRequestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa la clase
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="innerException">Excepción anterior</param>
        public SecureHttpRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}