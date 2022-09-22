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
        /// Usuario de Sesion que Ejecuta el Proceso
        /// </summary>
        public string UsuarioSesion { get; set; }

        /// <summary>
        /// Nombre Corte
        /// </summary>
        public string Corte { get; set; }

        /// <summary>
        /// Diccionario de Cantidades por Archivo de Proceso
        /// </summary>
        public Dictionary<string, CantidadesArchivos> DiccionarioCantidadesArchivos = new Dictionary<string, CantidadesArchivos>();

        /// <summary>
        /// Diccionario de Cantidades por Extractos Producto
        /// </summary>
        public CantidadesExtractos CantidadesProducto = new CantidadesExtractos();

        public void CalcularPorcentajesCantidadesArchivos()
        {
            foreach (var archivo in DiccionarioCantidadesArchivos.Values)
            {
                archivo.DiferenciaPesoArchivo = calcularPorcentaje(archivo);
            }
        }

        public void CalcularPorcentajesCantidadesProducto()
        {
            CantidadesProducto.Extractos.Diferencia = calcularPorcentaje(CantidadesProducto.Extractos);
            CantidadesProducto.EstadoCuenta.Diferencia = calcularPorcentaje(CantidadesProducto.EstadoCuenta);
            CantidadesProducto.Despositos.Diferencia = calcularPorcentaje(CantidadesProducto.Despositos);
            CantidadesProducto.TarjetasCredito.Diferencia = calcularPorcentaje(CantidadesProducto.TarjetasCredito);
            CantidadesProducto.ExtractosFundacion.Diferencia = calcularPorcentaje(CantidadesProducto.ExtractosFundacion);
            CantidadesProducto.ExtractosRotativo.Diferencia = calcularPorcentaje(CantidadesProducto.ExtractosRotativo);
            CantidadesProducto.ExtractosVivienda.Diferencia = calcularPorcentaje(CantidadesProducto.ExtractosVivienda);
            CantidadesProducto.Libranza.Diferencia = calcularPorcentaje(CantidadesProducto.Libranza);
            CantidadesProducto.Fiducoomeva.Diferencia = calcularPorcentaje(CantidadesProducto.Fiducoomeva);
            CantidadesProducto.ActivacionProtecciones.Diferencia = calcularPorcentaje(CantidadesProducto.ActivacionProtecciones);
            CantidadesProducto.CartasCobranzaHabeasData.Diferencia = calcularPorcentaje(CantidadesProducto.CartasCobranzaHabeasData);
            CantidadesProducto.HabeasData.Diferencia = calcularPorcentaje(CantidadesProducto.HabeasData);
            CantidadesProducto.CartasTAC.Diferencia = calcularPorcentaje(CantidadesProducto.CartasTAC);
            CantidadesProducto.CartasSOAT.Diferencia = calcularPorcentaje(CantidadesProducto.CartasSOAT);
        }

        private decimal calcularPorcentaje(CantidadProducto pCantidadProducto)
        {
            decimal resultado = 0;

            try
            {
                if (pCantidadProducto.MesAnterior > 0)
                {
                    decimal resta = pCantidadProducto.MesActual - pCantidadProducto.MesAnterior;
                    resultado = (resta / pCantidadProducto.MesAnterior) * 100;
                }
                else
                {
                    resultado = 100;
                }
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }

        private decimal calcularPorcentaje(CantidadesArchivos pCantidadesArchivos)
        {
            decimal resultado = 0;

            try
            {

                decimal resta = pCantidadesArchivos.PesoArchivoMesActual - pCantidadesArchivos.PesoArchivoMesAnterior;
                resultado = (resta / pCantidadesArchivos.PesoArchivoMesAnterior) * 100;
            }
            catch (Exception)
            {
                resultado = 0;
            }
            return resultado;
        }
    }

    /// <summary>
    /// Estructura para almacenar Cantidades por Archivo
    /// </summary>
    public class CantidadesArchivos
    {
        public string NombreArchivo { get; set; }
        public Int64 PesoArchivoMesActual { get; set; }
        public Int64 PesoArchivoMesAnterior { get; set; }
        public decimal DiferenciaPesoArchivo { get; set; }
    }

    /// <summary>
    /// Estructura para almacenar Cantidades por Extractos y Tipo de Producto
    /// </summary>
    public class CantidadesExtractos
    {
        public CantidadProducto Extractos;
        public CantidadProducto EstadoCuenta;
        public CantidadProducto Despositos;
        public CantidadProducto TarjetasCredito;
        public CantidadProducto ExtractosFundacion;
        public CantidadProducto ExtractosRotativo;
        public CantidadProducto ExtractosVivienda;
        public CantidadProducto Libranza;
        public CantidadProducto Fiducoomeva;
        public CantidadProducto ActivacionProtecciones;
        public CantidadProducto CartasCobranzaHabeasData;
        public CantidadProducto HabeasData;
        public CantidadProducto CartasTAC;
        public CantidadProducto CartasSOAT;
    }

    public struct CantidadProducto
    {
        public string Nombre;
        public Int32 MesActual;
        public Int32 MesAnterior;
        public decimal Diferencia;
    }
}
