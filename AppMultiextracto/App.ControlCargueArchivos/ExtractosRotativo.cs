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
            #region FormatearArchivo
            List<string> resultado = new List<string>();
            List<int> posFormateos = new List<int>();

            List<string> campos;
            foreach (var linea in datosOriginales)
            {
                campos = linea.Split('|').ToList();

                switch (campos[0])
                {
                    case "1":
                        posFormateos = new List<int>()
                        {
                            9, 10
                        };
                        resultado.Add($"1ROT| |{Helpers.ValidarPipePipe(ArmarLineaFormateada(posFormateos, new List<string>(linea.Split('|'))))}");
                        break;
                    case "2":
                        posFormateos = new List<int>()
                        {
                            4, 5, 6, 7
                        };
                        resultado.Add($"1ROB|{Helpers.ValidarPipePipe(ArmarLineaFormateada(posFormateos, new List<string>(linea.Split('|')), "2"))}");
                        break;
                    case "3":
                        posFormateos = new List<int>()
                        {
                            1, 2, 3, 4, 5, 6, 7
                        };
                        resultado.Add($"1ROC|{Helpers.ValidarPipePipe(ArmarLineaFormateada(posFormateos, new List<string>(linea.Split('|'))))}");
                        break;
                    case "4":
                        posFormateos = new List<int>()
                        {
                            1, 2, 3, 4, 5, 6, 7
                        };
                        resultado.Add($"1ROD|{Helpers.ValidarPipePipe(ArmarLineaFormateada(posFormateos, new List<string>(linea.Split('|'))))}");
                        break;
                }
            }
            return resultado; 
            #endregion
        }


        /// <summary>
        /// Arma la Linea Formateada de la cadena
        /// </summary>
        /// <param name="pFormateos">Campos a formatear</param>
        /// <param name="pDatosLinea">Daots de la linea</param>
        /// <param name="pTipo"></param>
        /// <returns>Linea formateada</returns>
        private string ArmarLineaFormateada(List<int> pFormateos, List<string> pDatosLinea, string pTipo = "0")
        {
            #region ArmarLineaFormateada
            string resultado = string.Empty;
            string comodin = string.Empty;

            for (int i = 0; i < pDatosLinea.Count; i++)
            {
                var dato = pDatosLinea[i];

                if (pFormateos.Contains(i))
                {
                    var transformado = Convert.ToDouble(dato.Replace(",", ".")).ToString("N2");

                    switch (pTipo)
                    {
                        case "2":
                            if (i == 5)
                            {
                                comodin = string.Empty;
                            }
                            else
                            {
                                comodin = "$ ";
                            }
                            break;

                        default:
                            comodin = "$ ";
                            break;
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
