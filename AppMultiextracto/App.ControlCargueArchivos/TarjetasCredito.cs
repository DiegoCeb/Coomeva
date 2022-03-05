using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using var = App.Variables.Variables;
using DLL_Utilidades;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que carga los datos puros del Producto TarjetasCredito
    /// </summary>
    public class TarjetasCredito: ICargue
    {
        private const string _producto = "TarjetasCredito";

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pArchivo">Ruta del Archivo</param>
        public TarjetasCredito(string pArchivo)
        {
            #region TarjetasCredito
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
            #endregion TarjetasCredito
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

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea.Length > 6)
                {
                    if (linea.Substring(0, 7) == "TARJETA")
                    {
                        llaveCruce = linea.Substring(20).Trim();
                        if (!var.DiccionarioExtractos.ContainsKey(llaveCruce))
                        {
                            var.DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = 'P',
                                    Extracto = new List<string>(){ linea}

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
                                    Separador = 'p',
                                    Extracto = new List<string>() { linea }
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
                        var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                    }
                }
                else
                {
                    var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                }
            }

            lector.Close();

            #endregion CargueArchivoDiccionario

        }

        
    }
}
