using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Muestras
    /// </summary>
    public class Muestras: App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Muestras(string pArchivo)
        {
            #region Muestras
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Helpers.EscribirLogVentana(ex.Message, true);
            }
            #endregion
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public Muestras()
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

            //bool encabezado = true;

            while ((linea = lector.ReadLine()) != null)
            {
                string llaveCruce = linea.Trim();

                if (InsumoMuestras.ContainsKey(llaveCruce))
                {
                    InsumoMuestras[llaveCruce].InsumoLinea.Add(linea);
                }
                else
                {
                    InsumoMuestras.Add(llaveCruce, new Variables.DatosInsumos
                    {
                        Separador = '0',
                        InsumoLinea = new List<string> { linea }
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

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            return new List<string>();
        }
    }
}
