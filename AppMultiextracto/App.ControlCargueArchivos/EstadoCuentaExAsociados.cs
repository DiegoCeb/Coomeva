using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using var = App.Variables.Variables;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de EstadoCuentaExAsociados
    /// </summary>
    public class EstadoCuentaExAsociados: ICargue
    {
        private const string _producto = "EstadoCuentaExAsociados";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public EstadoCuentaExAsociados(string pArchivo)
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
        /// Metodo que carga los datos Puros del producto TarjetasCredito
        /// </summary>
        /// <param name="pArchivo">Ruta del Archivo</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            List<string> temp = new List<string>();

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea.Contains("del Mes"))
                {
                    //cambio prueba
                    string llaveCruce = linea.Substring(79, 15).Trim();

                    if (var.DiccionarioExtractos.ContainsKey(llaveCruce))
                    {
                        if (var.DiccionarioExtractos[llaveCruce].ContainsKey(_producto))
                        {
                            var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(temp.FirstOrDefault());
                            var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                        }
                        else
                        {
                            var.DiccionarioExtractos[llaveCruce].Add(_producto, new Variables.DatosExtractos
                            {
                                Separador = 'P',
                                Extracto = new List<string>() { linea }
                            });
                        }
                    }
                    else
                    {
                        var.DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { _producto, new Variables.DatosExtractos
                            {
                                Separador = 'P',
                                Extracto = new List<string>(temp)
                            } }
                        });
                    }
                }
                else
                {
                    temp.Add(linea);
                }
            }

            lector.Close();
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
    }
}
