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
    /// Clase que se encarga de cargar el archivo de Etiquetas
    /// </summary>
    public class Etiquetas : App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Etiquetas(string pArchivo)
        {
            #region Etiquetas
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Etiquetas),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);                
            } 
            #endregion
        }

        /// <summary>
        /// Cosntructor General
        /// </summary>
        public Etiquetas()
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
            string producto = string.Empty;

            while ((linea = lector.ReadLine()) != null)
            {
                string llaveCruce = $"{linea.Substring(143, 6)}{linea.Substring(151, 5)}".TrimStart('0');

                if (Path.GetFileNameWithoutExtension(pArchivo).Split('_').ElementAt(0).Last() == 'E')
                {
                    AgregarDiccionario(InsumoEtiquetasMail, llaveCruce, linea);
                }
                else
                {
                    AgregarDiccionario(InsumoEtiquetasFisico, llaveCruce, linea);
                }
            }

            lector.Close(); 
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pDiccionario"></param>
        /// <param name="pLlaveCruce"></param>
        /// <param name="pLinea"></param>
        private void AgregarDiccionario<T>(T pDiccionario, string pLlaveCruce, string pLinea) 
            where T : Dictionary<string, Variables.DatosInsumos>
        {
            #region AgregarDiccionario
            if (pDiccionario.ContainsKey(pLlaveCruce))
            {
                pDiccionario[pLlaveCruce].InsumoLinea.Add(pLinea);
            }
            else
            {
                pDiccionario.Add(pLlaveCruce, new Variables.DatosInsumos
                {
                    Separador = 'P',
                    InsumoLinea = new List<string> { pLinea }
                });
            } 
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
