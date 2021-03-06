using App.Controlnsumos;
using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de ExtractosRotativo
    /// </summary>
    public class ExtractosRotativo: App.Variables.Variables, ICargue
    {
        private const string _producto = "ExtractosRotativo";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public ExtractosRotativo(string pArchivo)
        {
            #region ExtractosRotativo
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(ExtractosRotativo),
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
        public ExtractosRotativo()
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
            List<string> temp = new List<string>();
            bool extractoCompleto = false;
            string llaveCruce = string.Empty;

            while ((linea = lector.ReadLine()) != null)
            {
                if(linea.Split('|')[0].Trim() == "1")
                {
                    extractoCompleto = false;

                    if (temp.Count > 1)
                    {
                        extractoCompleto = true;
                    }

                    if (extractoCompleto)
                    {
                        llaveCruce = temp.ElementAt(0).Split('|')[6].Trim();
                        AgregarDiccionario(llaveCruce, temp);
                        temp.Clear();
                    }

                    temp.Add(linea);
                }
                else
                {
                    temp.Add(linea);
                }
            }

            //Ultimo Extracto
            if (temp.Count > 1)
            {
                llaveCruce = temp.ElementAt(0).Split('|')[6].Trim();
                AgregarDiccionario(llaveCruce, temp);
            }

            lector.Close();
            #endregion
        }

        /// <summary>
        /// Metodo que agrega al Dicionario General.
        /// </summary>
        /// <param name="pLlaveCruce">llave de cruce (Cedula)</param>
        /// <param name="pTemp">Lista del extracto</param>
        private void AgregarDiccionario(string pLlaveCruce, List<string> pTemp)
        {
            #region AgregarDiccionario
            if (DiccionarioExtractos.ContainsKey(pLlaveCruce))
            {
                if (DiccionarioExtractos[pLlaveCruce].ContainsKey(_producto))
                {
                    DiccionarioExtractos[pLlaveCruce][_producto].Extracto.InsertRange(DiccionarioExtractos[pLlaveCruce][_producto].Extracto.Count, pTemp);
                }
                else
                {
                    DiccionarioExtractos[pLlaveCruce].Add(_producto, new Variables.DatosExtractos
                    {
                        Separador = '|',
                        Extracto = new List<string>(pTemp),
                        TipoClase = typeof(ExtractosRotativo)
                    });
                }
            }
            else
            {
                DiccionarioExtractos.Add(pLlaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { _producto, new Variables.DatosExtractos
                            {
                                Separador = '|',
                                Extracto = new List<string>(pTemp),
                                TipoClase = typeof(ExtractosRotativo)
                            } }
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

        /// <summary>
        /// Metodo que Formatea la data para el Sal.
        /// </summary>
        /// <param name="datosOriginales">Lista orginal</param>
        /// <returns>Lista Formateada</returns>
        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            List<string> resultado = new List<string>();

            List<string> campos;
            foreach (var linea in datosOriginales)
            {
                campos = linea.Split('|').ToList();

                switch (campos[0])
                {
                    case "1":
                        resultado.Add($"1ROT| |{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "2":
                        resultado.Add($"1ROB|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "3":
                        resultado.Add($"1ROC|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "4":
                        resultado.Add($"1ROD|{Helpers.ValidarPipePipe(linea)}");
                        break;
                }
            }
            return resultado;
        }
    }
}
