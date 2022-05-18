using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                Environment.Exit(1);
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

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            throw new NotImplementedException();
        }
    }
}
