using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Controlnsumos;
using DLL_Utilidades;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de ActivacionProtecciones
    /// </summary>
    public class ActivacionProtecciones : App.Variables.Variables, ICargue
    {
        private const string _producto = "ActivacionProtecciones";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public ActivacionProtecciones(string pArchivo)
        {
            #region ActivacionProtecciones
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(ActivacionProtecciones),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError , true);
            } 
            #endregion
        }

        /// <summary>
        /// Constructor Generico
        /// </summary>
        public ActivacionProtecciones()
        {}

        /// <summary>
        /// Metodo Encargado de cargar al diccionario Principal los datos PUROS, solo con limpieza.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            bool encabezado = true;

            while ((linea = lector.ReadLine()) != null)
            {
                string[] datosLinea = linea.Split(';');

                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = datosLinea.ElementAt(0).Replace('"', ' ').Trim();

                if (DiccionarioExtractos.ContainsKey(llaveCruce))
                {
                    if (DiccionarioExtractos[llaveCruce].ContainsKey(_producto))
                    {
                        DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea.Replace('"', ' ').Trim());
                    }
                    else
                    {
                        DiccionarioExtractos[llaveCruce].Add(_producto, new Variables.DatosExtractos
                        {
                            Separador = ';',
                            Extracto = new List<string>() { linea.Replace('"', ' ').Trim() },
                            TipoClase = typeof(ActivacionProtecciones)
                        });
                    }
                }
                else
                {
                    DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { _producto, new Variables.DatosExtractos
                            {
                                Separador = ';',
                                Extracto = new List<string>() { linea.Replace('"',' ').Trim() },
                                TipoClase = typeof(ActivacionProtecciones)
                            } }
                        });
                }
            }

            lector.Close();
            #endregion
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
                linea = Helpers.RemplazarCaracteres(';', '|', lineaDatos);
                linea = Helpers.TrimCamposLinea('|', linea);
                resultado.Add($"1ACP| |{Helpers.ValidarPipePipe(linea)}");
            }
            
            return resultado; 
            #endregion
        }
    }
}
