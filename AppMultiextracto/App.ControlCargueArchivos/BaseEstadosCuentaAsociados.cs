﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using var = App.Variables.Variables;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de BaseEstadosCuentaAsociados
    /// </summary>
    public class BaseEstadosCuentaAsociados : ICargue
    {
        private const string _producto = "BaseEstadosCuentaAsociados";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public BaseEstadosCuentaAsociados(string pArchivo)
        {
            #region BaseEstadosCuentaAsociados
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
            #endregion BaseEstadosCuentaAsociados
        }

        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario

            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            string llaveCruce = string.Empty;

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea.Split('|')[0].Trim().ToUpper() != "NITCLI")
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

            lector.Close();

            #endregion CargueArchivoDiccionario
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
