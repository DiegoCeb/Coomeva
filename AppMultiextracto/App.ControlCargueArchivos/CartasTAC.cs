using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    public class CartasTAC : App.Variables.Variables, ICargue, IDisposable
    {
        private const string _producto = "CartasTAC";

        private bool _disposed = false;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public CartasTAC(string pArchivo)
        {
            #region CartasTAC
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Helpers.EscribirLogVentana(ex.Message, true);
            }
            #endregion CartasTAC
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public CartasTAC()
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

                if (linea.Split('|')[0].Trim().ToUpper() != "ESTADO")
                {
                    llaveCruce = linea.Split('|')[2].Trim();

                    if (pArchivo.ToUpper().Contains("BASE_ACTIVOS_TAC"))
                    {
                        linea = $"{linea}|Activos";
                    }
                    else
                    {
                        linea = $"{linea}|Inactivos";
                    }


                    if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                    {
                        DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = '|',
                                    Extracto = new List<string>(){ linea },
                                    TipoClase = typeof(CartasTAC)
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
                                TipoClase = typeof(CartasTAC)
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
            List<string> campos;
            string linea;
            foreach (var lineaDatos in datosOriginales)
            {
                linea = Helpers.RemplazarCaracteres(';', '|', lineaDatos);
                linea = Helpers.TrimCamposLinea('|', linea);
                campos = linea.Split('|').ToList();

                if (campos[(campos.Count() - 1)] == "Activos")
                {
                    campos.Insert(8, " ");
                }
                linea = Helpers.ListaCamposToLinea(campos, '|');
                resultado.Add($"1UUU| |{Helpers.ValidarPipePipe(linea)}");
            }

            return resultado;
            #endregion
        }
    }
}
