using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de CartasCobranzaHabeasData
    /// </summary>
    public class CartasCobranzaHabeasData : App.Variables.Variables, ICargue, IDisposable
    {
        private const string _producto = "CartasCobranzaHabeasData";

        private bool _disposed = false;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public CartasCobranzaHabeasData(string pArchivo)
        {
            #region CartasCobranzaHabeasData
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));

                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
            }
            #endregion CartasCobranzaHabeasData
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public CartasCobranzaHabeasData()
        {}

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

                if (linea.Split('|')[0].Trim().ToUpper() != "FECHA")
                {
                    llaveCruce = linea.Split('|')[1].Trim();
                    if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                    {
                        DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = '|',
                                    Extracto = new List<string>(){ linea},
                                    TipoClase = typeof(CartasCobranzaHabeasData)
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
                                TipoClase = typeof(CartasCobranzaHabeasData)
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
            string linea;
            foreach (var lineaDatos in datosOriginales)
            {
                linea = Helpers.TrimCamposLinea('|', lineaDatos);
                resultado.Add($"1CCH| |{Helpers.ValidarPipePipe(linea)}");
            }
            //Helpers.EscribirEnArchivo(@"C:\Users\ivanm\OneDrive\Documentos\ProyectosC\Coomeva\EjemploExtractosCartasCobranzaHabeasData.sal", resultado);
            return resultado; 
            #endregion
        }
    }
}
