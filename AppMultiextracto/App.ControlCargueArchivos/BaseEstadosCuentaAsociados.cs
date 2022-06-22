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
    /// Clase que se encarga de cargar el archivo de BaseEstadosCuentaAsociados
    /// </summary>
    public class BaseEstadosCuentaAsociados : App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public BaseEstadosCuentaAsociados(string pArchivo)
        {
            #region BaseEstadosCuentaAsociados
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(BaseEstadosCuentaAsociados),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
            }
            #endregion BaseEstadosCuentaAsociados
        }

        /// <summary>
        /// Constructor Generico
        /// </summary>
        public BaseEstadosCuentaAsociados()
        {}

        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario

            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;

            bool encabezado = true;

            while ((linea = lector.ReadLine()) != null)
            {
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = linea.Split('|')[0].Trim();

                if (InsumoBaseAsociados.ContainsKey(llaveCruce))
                {
                    InsumoBaseAsociados[llaveCruce].InsumoLinea.Add(linea);
                }
                else
                {
                    InsumoBaseAsociados.Add(llaveCruce, new Variables.DatosInsumos
                    {
                        Separador = '|',
                        InsumoLinea = new List<string> { linea }
                    });
                }
            }

            lector.Close();

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

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            return new List<string>();
        }
    }
}
