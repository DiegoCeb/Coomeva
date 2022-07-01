using DLL_ServicioDelta;
using DLL_ServicioDelta.wsDelta;
using DLL_Utilidades;
using System;
using System.IO;
using System.Linq;

namespace App.ControlWebServiceZonificacion
{
    /// <summary>
    /// Clase ControlZonificacion
    /// </summary>
    public class ControlZonificacion
    {
        /// <summary>
        /// Metodo para crear orden de servicio Delta
        /// </summary>
        /// <param name="cliente">Cliente</param>
        /// <param name="proyecto">Proyecto</param>
        /// <returns>Numero Orden Servicio</returns>
        public static string CrearOrdenServicio(string cliente, string proyecto)
        {
            #region CreaOrdenServicio

            string[] parametros = new string[4];

            parametros[0] = Utilidades.LeerAppConfig("UserWebService");
            parametros[1] = Utilidades.Base64Encode(Utilidades.LeerAppConfig("Password"));
            parametros[2] = cliente;
            parametros[3] = proyecto;

            string ordenServicio = ServicioDelta.CrearOrdenServicio(parametros).ordenServicio;
            return ordenServicio;

            #endregion
        }

        /// <summary>
        /// realiza la zonificacion del proceso
        /// </summary>
        /// <param name="ordenServicio">Numero Orden</param>
        /// <param name="rutaArchivo">Ruta Archivos</param>
        /// <param name="mapeo">Identificacdor Mapeo</param>
        /// <param name="tipoCargue">Tipo cargue</param>
        /// <param name="cliente">Codigo Cliente</param>
        /// <param name="courier">Codigo Courrier</param>
        /// <param name="proyecto">Codigo Proyecto</param>
        /// <param name="certificadoEmail">Bandera Certificdo Email</param>
        /// <param name="tipoarchivo">Tipo Archivo</param>
        /// <param name="reordenamiento">Bandera reordenamiento</param>
        /// <param name="publicacion">Bandera publicación</param>
        /// <param name="nombreProceso">Nombre del proceso</param>
        /// <param name="delimitador">Caracter delimitador</param>
        /// <param name="inicioExtractoSpool">Identifiacor de inicio inicio extracto</param>
        /// <returns>El estado del la zonificacion</returns>
        public static string RealizarZonificacion(string ordenServicio, string rutaArchivo, string mapeo, string tipoCargue, string cliente, string courier, string proyecto, string certificadoEmail, string tipoarchivo, string reordenamiento, string publicacion, string nombreProceso, string delimitador, string inicioExtractoSpool)
        {
            #region RealizarZonificacion 

            string[] parametros = new string[20];

            parametros[0] = Utilidades.LeerAppConfig("UserWebService");                             //Usuario delta
            parametros[1] = Utilidades.Base64Encode(Utilidades.LeerAppConfig("Password"));          //Paswword delta
            parametros[2] = ordenServicio;                                                          //Orden Servicio
            parametros[3] = nombreProceso;                                                          // Nombre Proceso
            parametros[4] = courier;                                                                // Courrier        
            parametros[5] = cliente;                                                                // Codigo Cliente              
            parametros[6] = proyecto;                                                               // Codigo Proceso
            parametros[7] = DateTime.Now.ToString("yyyy-MM-dd");                                    // Fecha Corte
            parametros[8] = certificadoEmail;                                                       // Certificado Mail
            parametros[9] = tipoCargue;                                                             // Tipo Cargue
            parametros[10] = tipoarchivo;                                                           // Tipo Archivo
            parametros[11] = mapeo;                                                                 // Codigo Conf. Mapeo
            parametros[12] = reordenamiento;                                                        // Reordenar
            parametros[13] = "0";                                                                   // Estado
            parametros[14] = publicacion;                                                           // Clasificar Mail
            parametros[15] = delimitador;                                                           // Delimitador
            parametros[16] = inicioExtractoSpool;                                                   // Inicio Extracto
            parametros[17] = rutaArchivo;                                                           // Ruta FTP
            parametros[18] = Path.GetFileName(rutaArchivo);                                         // Nombre Archivo
            parametros[19] = "";                                                                    // Archivo Base64

            procDinaStruct[] resultadoDelta = ServicioDelta.ZonificarBase(parametros);
            return resultadoDelta.Last().estadoCargue;
            #endregion
        }

        /// <summary>
        /// Metodo que se encarga de validar si la orden de servicio termino de procesar.
        /// </summary>
        /// <param name="orden">Orden a verificar</param>
        /// <returns>Regresa el estado en el que se encuentra la orden.</returns>
        public static string ValidarOrden(string orden)
        {
            #region ValidarOrden
            string[] parametros = new string[3];

            parametros[0] = Utilidades.LeerAppConfig("UserWebService");
            parametros[1] = Utilidades.Base64Encode(Utilidades.LeerAppConfig("Password"));
            parametros[2] = orden;

            System.Threading.Thread.Sleep(3000);

            string estado = ServicioDelta.ValidarEstadoOrden(parametros).First().estadoCargue;

            return estado;
            #endregion
        }
    }
}
