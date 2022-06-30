using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using App.Controlnsumos;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de BaseEstadosCuentaTerceros
    /// </summary>
    public class BaseEstadosCuentaTerceros : App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public BaseEstadosCuentaTerceros(string pArchivo)
        {
            #region BaseEstadosCuentaTerceros 
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(BaseEstadosCuentaTerceros),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
            }
            #endregion BaseEstadosCuentaTerceros
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public BaseEstadosCuentaTerceros()
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
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = linea.Split('|')[0].Trim();

                if (InsumoBaseTerceros.ContainsKey(llaveCruce))
                {
                    InsumoBaseTerceros[llaveCruce].InsumoLinea.Add(linea);
                }
                else
                {
                    InsumoBaseTerceros.Add(llaveCruce, new Variables.DatosInsumos
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

        /// <summary>
        /// Metodo que Formatea la data para el Sal.
        /// </summary>
        /// <param name="datosOriginales">Lista orginal</param>
        /// <returns>Lista Formateada</returns>
        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            return new List<string>();
        }
    }
}
