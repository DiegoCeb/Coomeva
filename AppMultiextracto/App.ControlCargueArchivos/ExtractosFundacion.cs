using System;
using System.Collections.Generic;
using App.Controlnsumos;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de ExtractosFundacion
    /// </summary>
    public class ExtractosFundacion : App.Variables.Variables, ICargue
    {
        private const string _producto = "ExtractosFundacion";
        private bool _disposed = false;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public ExtractosFundacion(string pArchivo)
        {
            #region ExtractosFundacion
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(ExtractosFundacion),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);

            }
            #endregion ExtractosFundacion
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public ExtractosFundacion()
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

            foreach (string lineaIteracion in archivo)
            {
                linea = lineaIteracion.Replace('"', ' ').Trim();

                if (linea.Split('|')[0].Trim().ToUpper() != "NAME")
                {
                    llaveCruce = linea.Split('|')[61].Trim();
                    if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                    {
                        DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = '|',
                                    Extracto = new List<string>(){ linea},
                                    TipoClase = typeof(ExtractosFundacion)
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
                                TipoClase = typeof(ExtractosFundacion)
                            });
                        }
                        else
                        {
                            DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                        }

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
        /// Metodo que destruye la lista proveniente del excel
        /// </summary>
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

        /// <summary>
        /// Metodo que Formatea la data para el Sal.
        /// </summary>
        /// <param name="datosOriginales">Lista orginal</param>
        /// <returns>Lista Formateada</returns>
        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            #region FormatearArchivo
            List<string> resultado = new List<string>();
            List<int> posFormateos = new List<int>()
            {
                21, 22, 24, 84, 96, 97, 98, 99, 101, 103, 104, 105, 106, 107
            };

            foreach (var linea in datosOriginales)
            {
                resultado.Add($"1MIC| |{Helpers.ValidarPipePipe(ArmarLineaFormateada(posFormateos, new List<string>(linea.Split('|'))))}");
            }

            return resultado; 
            #endregion
        }

        /// <summary>
        /// Arma la Linea Formateada de la cadena
        /// </summary>
        /// <param name="pFormateos">Campos a formatear</param>
        /// <param name="pDatosLinea">Daots de la linea</param>
        /// <returns>Linea formateada</returns>
        private string ArmarLineaFormateada(List<int> pFormateos, List<string> pDatosLinea)
        {
            #region ArmarLineaFormateada
            string resultado = string.Empty;
            string comodin = string.Empty;

            for (int i = 0; i < pDatosLinea.Count; i++)
            {
                var dato = pDatosLinea[i];

                if (pFormateos.Contains(i))
                {
                    if (string.IsNullOrEmpty(dato.Trim()))
                    {
                        dato = "0";
                    }

                    var transformado = Convert.ToDouble(dato.Trim()).ToString("N2");

                    if (i == 97 || i == 98 || i == 99)
                    {
                        comodin = string.Empty;
                    }
                    else
                    {
                        comodin = "$ ";
                    }

                    dato = $"{comodin}{transformado.Substring(0, transformado.LastIndexOf('.')).Replace(",", ".")},{transformado.Substring(transformado.LastIndexOf('.') + 1)}";
                }

                resultado += $"{dato}|";
            }

            return resultado.Substring(0, resultado.Length - 1);
            #endregion
        }
    }
}
