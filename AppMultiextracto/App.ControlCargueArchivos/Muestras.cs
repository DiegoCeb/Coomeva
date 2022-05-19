using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Muestras
    /// </summary>
    public class Muestras: App.Variables.Variables, ICargue
    {
        private const string _producto = "Muestras";

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
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                Environment.Exit(1);
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
            string llaveCruce = string.Empty;

            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);
            string linea = string.Empty;

            while (!string.IsNullOrEmpty(linea = lector.ReadLine()))
            {
                llaveCruce = linea.Trim();

                if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                {
                    DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                    {
                                        Separador = '|',
                                        Extracto = new List<string>(){ linea},
                                        TipoClase = typeof(Muestras),
                                        Insumo = true
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
                            TipoClase = typeof(Muestras),
                            Insumo = true
                        });
                    }
                    else
                    {
                        DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                    }
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
