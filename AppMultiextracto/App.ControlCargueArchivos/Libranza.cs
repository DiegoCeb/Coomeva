using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using var = App.Variables.Variables;
using DLL_Utilidades;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Libranzas
    /// </summary>
    public class Libranza: ICargue
    {
        private const string _producto = "Libranza";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Libranza(string pArchivo)
        {
            #region Libranza
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

                        if (!var.DiccionarioExtractos.ContainsKey(llaveCruce))
                        {
                            var.DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                    {
                                        Separador = 'P',
                                        Extracto = TemEncabezado

                                    }
                                }
                            });
                        }
                        else
                        {
                            if (!var.DiccionarioExtractos[llaveCruce].ContainsKey(_producto))
                            {
                                var.DiccionarioExtractos[llaveCruce].Add(_producto, new Variables.DatosExtractos
                                {
                                    Separador = 'P',
                                    Extracto = TemEncabezado
                                });
                            }
                            else
                            {
                                var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
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
                            var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
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
                        var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                    }
                }
            }

            lector.Close();
            #endregion CargueArchivoDiccionario
        }
    }
}
