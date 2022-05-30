﻿using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Pinos
    /// </summary>
    public class Pinos : App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Pinos(string pArchivo)
        {
            #region Pinos
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
        public Pinos()
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

            bool encabezado = true;

            while ((linea = lector.ReadLine()) != null)
            {
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = linea.Split(';')[0].Trim();

                if (InsumoPinos.ContainsKey(llaveCruce))
                {
                    InsumoPinos[llaveCruce].InsumoLinea.Add(linea);
                }
                else
                {
                    InsumoPinos.Add(llaveCruce, new Variables.DatosInsumos
                    {
                        Separador = ';',
                        InsumoLinea = new List<string> { linea }
                    });
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
