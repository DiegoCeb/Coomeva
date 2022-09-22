using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Controlnsumos;
using DLL_Utilidades;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de CartasSOAT
    /// </summary>
    public class CartasSOAT : App.Variables.Variables, ICargue, IDisposable
    {
        private const string _producto = "CartasSOAT";

        private bool _disposed = false;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public CartasSOAT(string pArchivo)
        {
            #region CartasSOAT
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(CartasTAC),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
            }
            #endregion CartasSOAT
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public CartasSOAT()
        { }

        /// <summary>
        /// Metodo Encargado de cargar al diccionario Principal los datos PUROS, solo con limpieza.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            string linea = string.Empty;
            string llaveCruce = string.Empty;
            List<string> archivo = Helpers.ConvertirExcel(pArchivo);
            bool encabezado = true;

            foreach (string lineaIteracion in archivo)
            {
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                linea = lineaIteracion.Replace('"', ' ').Trim();

                llaveCruce = linea.Split('|').ElementAt(3).Trim();

                if (string.IsNullOrEmpty(llaveCruce))
                {
                    continue;
                }

                if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                {
                    DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = '|',
                                    Extracto = new List<string>(){ linea },
                                    TipoClase = typeof(CartasSOAT)
                                }
                                }
                            });
                }
                else
                {
                    if (!DiccionarioExtractos[llaveCruce].ContainsKey(_producto))
                    {
                        DiccionarioExtractos[llaveCruce].Add(_producto, new Variables.DatosExtractos
                        {
                            Separador = '|',
                            Extracto = new List<string>() { linea },
                            TipoClase = typeof(CartasSOAT)
                        });
                    }
                    else
                    {
                        DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                    }
                }
            }

            archivo.Clear();

            #endregion CargueArchivoDiccionario
        }

        /// <summary>
        /// Metodo que desencadena el cargue.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void Ejecutar(string pArchivo)
        {
            CargueArchivoDiccionario(pArchivo);
        }

        /// <summary>
        /// Metodo que Formatea la data para el Sal.
        /// </summary>
        /// <param name="datosOriginales">Lista orginal</param>
        /// <returns>Lista Formateada</returns>
        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            #region FormatearArchivo
            List<string> resultado = new List<string>();
            string linea;

            foreach (var lineaDatos in datosOriginales)
            {
                resultado.Add($"1SOA| |{Helpers.ValidarPipePipe(lineaDatos)}");
            }

            return resultado;
            #endregion
        }

        /// <summary>
        /// Imlementacion del Dispose para destrir lista del excel.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose()
        {
            #region Dispose

            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);

            #endregion Dispose
        }

        /// <summary>
        /// Imlementacion del Dispose para destrir lista del excel.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            #region Dispose

            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _disposed = true;
            #endregion Dispose
        }

    }
}
