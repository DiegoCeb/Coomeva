using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que carga los datos puros del Producto ExtractosVivienda
    /// </summary>
    public class ExtractosVivienda : App.Variables.Variables, ICargue
    {
        private const string _producto = "ExtractosVivienda";

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pArchivo">Ruta del Archivo</param>
        public ExtractosVivienda(string pArchivo)
        {
            #region ExtractosVivienda
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

            #endregion ExtractosVivienda
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public ExtractosVivienda()
        {}

        /// <summary>
        /// Metodo que ejecuta el inicio del Cargue
        /// </summary>
        /// <param name="pArchivo">Ruta del Archivo</param>
        public void Ejecutar(string pArchivo)
        {
            CargueArchivoDiccionario(pArchivo);
        }

        /// <summary>
        /// Metodo que carga los datos Puros del producto TarjetasCredito
        /// </summary>
        /// <param name="pArchivo">Ruta del Archivo</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            string llaveCruce = string.Empty;
            List<string> TemEncabezado = new List<string>();
            bool llaveEncontrada = false;
            while ((linea = lector.ReadLine()) != null)
            {
                if (linea.Length > 120)
                {
                    if (linea.Substring(60, 35).Trim() == "B A N C O O M E V A")
                    {
                        llaveEncontrada = false;
                        TemEncabezado = new List<string>();
                        TemEncabezado.Add(linea);
                    }
                    else if (linea.Substring(0, 19).Trim() == "Cedula del cliente:")
                    {
                        llaveEncontrada = true;
                        llaveCruce = linea.Substring(22, 25).Trim();
                        TemEncabezado.Add(linea);

                        if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                        {
                            DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = 'P',
                                    Extracto = TemEncabezado,
                                    TipoClase = typeof(ExtractosVivienda)
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
                                    Separador = 'p',
                                    Extracto = TemEncabezado,
                                    TipoClase = typeof(ExtractosVivienda)
                                });
                            }
                            else
                            {
                                DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                            }

                        }

                    }
                    else
                    {
                        if (!llaveEncontrada)
                        {
                            TemEncabezado.Add(linea);
                        }
                        else
                        {
                            DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                        }
                    }
                }
                else
                {
                    if (!llaveEncontrada)
                    {
                        TemEncabezado.Add(linea);
                    }
                    else
                    {
                        DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                    }
                }

            }

            lector.Close();
            #endregion CargueArchivoDiccionario
        }

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            return new List<string>();
        }
    }
}
