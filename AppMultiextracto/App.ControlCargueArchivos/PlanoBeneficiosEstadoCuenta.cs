﻿using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using var = App.Variables.Variables;
using System.Text;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de PlanoBeneficiosEstadoCuenta
    /// </summary>
    public class PlanoBeneficiosEstadoCuenta: ICargue
    {
        private const string _producto = "PlanoBeneficiosEstadoCuenta";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public PlanoBeneficiosEstadoCuenta(string pArchivo)
        {
            #region PlanoBeneficiosEstadoCuenta
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

            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);
            string linea = string.Empty;

            while (!string.IsNullOrEmpty(linea = lector.ReadLine()))
            {
                if (linea.Split('\t')[0].Trim().ToUpper() != "PERIODO")
                {
                    llaveCruce = linea.Split('\t')[3].Trim();

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
