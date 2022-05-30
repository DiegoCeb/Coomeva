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
        public CantidadesExtractos CantidadesExtractosNacional = new CantidadesExtractos();

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
    public class CantidadesExtractos
    {
        public CantidadProducto Extractos;
        public CantidadProducto HojasEstadoCuentaSimplex;
        public CantidadProducto HojasEstadoCuentaDuplex;
        public CantidadProducto HojasViviendaSimplex;
        public CantidadProducto HojasViviendaDuplex;
        public CantidadProducto HojasDespositosSimplex;
        public CantidadProducto HojasDespositosDuplex;
        public CantidadProducto ExtractosVisa;
        public CantidadProducto ExtractosMaster;
        public CantidadProducto CartasSOAT;
        public CantidadProducto CartasAsocHabeasData;
        public CantidadProducto CartasCobrosHabeasData;
        public CantidadProducto ActivacionProtecciones;
        public CantidadProducto ExtractosPlanPagosLibranza;
        public CantidadProducto ExtractosCreditoRotativo;
        public CantidadProducto ExtractosMicroCredito;
        public CantidadProducto Fiducoomeva;
        public CantidadProducto CartasTAC;
    }

    public struct CantidadProducto
    {
        public Int32 MesActual;
        public Int32 MesAnterior;
        public Int32 Diferencia;
    }
}
