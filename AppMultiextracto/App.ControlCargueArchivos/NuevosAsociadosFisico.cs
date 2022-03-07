using App.Controlnsumos;
using DLL_Utilidades;
using System;
using System.Collections.Generic;
using var = App.Variables.Variables;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de NuevosAsociadosFisico
    /// </summary>
    public class NuevosAsociadosFisico : ICargue
    {
        private const string _producto = "NuevosAsociadosFisico";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public NuevosAsociadosFisico (string pArchivo)
        {
            #region AsociadosInactivos
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
        /// Metodo Encargado de cargar al diccionario Principal los datos PUROS, solo con limpieza.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            string llaveCruce = string.Empty;
            List<string> archivo = Helpers.ConvertirExcel(pArchivo);

            foreach (string linea in archivo)
            {
                if (linea.Split('|')[0].Trim().ToUpper() != "CEDULA")
                {
                    llaveCruce = linea.Split('|')[0].Trim();
                    if (!var.DiccionarioExtractos.ContainsKey(llaveCruce))
                    {
                        var.DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                    {
                                        Separador = '|',
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
                                Separador = '|',
                                Extracto = new List<string>() { linea }
                            });
                        }
                        else
                        {
                            var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                        }
                    }
                }
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
    }
}
