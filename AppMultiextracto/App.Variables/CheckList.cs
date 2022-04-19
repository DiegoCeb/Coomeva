using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Variables
{
    /// <summary>
    /// Clase que almacena los datos del ChekList de Archivos
    /// </summary>
    public class CheckList
    {
        /// <summary>
        /// Fecha y Hora de Inicio de Proceso
        /// </summary>
        public DateTime FechaHoraIncio { get; set; }

        /// <summary>
        /// Fecha y Hora de Fin de Proceso
        /// </summary>
        public DateTime FechaHoraFin { get; set; }

        /// <summary>
        /// Usuario se Sesion que Ejecuta el Proceso
        /// </summary>
        public string UsuarioSesion { get; set; } //-- enviroment puede ser ojo????

        /// <summary>
        /// Diccionario de Cantidades por Archivo de Proceso
        /// </summary>
        public Dictionary<string, CantidadesArchivos> DiccionarioCantidadesArchivos = new Dictionary<string, CantidadesArchivos>();

        /// <summary>
        /// Cantidades Extractos por Prodctos Nacional
        /// </summary>
        public CantidadesExtractos CantidadesExtractosNacional { get; set; }

        /// <summary>
        /// Diccionario de Cantidades por Extractos Producto por Ciudades
        /// </summary>
        public Dictionary<string, CantidadesExtractos> DiccionarioCantidadesPorCiudades = new Dictionary<string, CantidadesExtractos>();

    }

    /// <summary>
    /// Estructura para almacenar Cantidades por Archivo
    /// </summary>
    public class CantidadesArchivos
    {
        public string NombreArchivo { get; set; }
        public Int64 PesoArchivoMesActual { get; set; }
        public Int64 PesoArchivoMesAnterior { get; set; }
        public Int64 DiferenciaPesoArchivo { get; set; }
    }

    /// <summary>
    /// Estructura para almacenar Cantidades por Extractos y Tipo de Producto
    /// </summary>
    public struct CantidadesExtractos
    {
        public Int32 Extractos;
        public Int32 HojasEstadoCuentaSimplex;
        public Int32 HojasEstadoCuentaDuplex;
        public Int32 HojasViviendaSimplex;
        public Int32 HojasViviendaDuplex;
        public Int32 HojasDespositosSimplex;
        public Int32 HojasDespositosDuplex;
        public Int32 ExtractosVisa;
        public Int32 ExtractosMaster;
        public Int32 CartasSOAT;
        public Int32 CartasAsocHabeasData;
        public Int32 CartasCobrosHabeasData;
        public Int32 ActivacionProtecciones;
        public Int32 ExtractosPlanPagosLibranza;
        public Int32 ExtractosCreditoRotativo;
        public Int32 ExtractosMicroCredito;
        public Int32 Fiducoomeva;
    }
}
