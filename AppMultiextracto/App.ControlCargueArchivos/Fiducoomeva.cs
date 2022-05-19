﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Fiducoomeva
    /// </summary>
    public class Fiducoomeva : App.Variables.Variables, ICargue
    {
        private const string _producto = "Fiducoomeva";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Fiducoomeva(string pArchivo)
        {
            #region Fiducoomeva
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
        public Fiducoomeva()
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
            string llaveCruce = string.Empty;
            List<string> temp = new List<string>();
            bool extractoCompleto = false;

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea == "\u001a")
                {
                    continue;
                }

                if (linea.Substring(0, 3) == "001")
                {
                    extractoCompleto = false;

                    if (temp.Count > 1)
                    {
                        extractoCompleto = true;
                    }

                    if (extractoCompleto)
                    {
                        llaveCruce = temp.ElementAt(0).Split(';').ElementAt(14).TrimStart('0').Trim();

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
                llaveCruce = temp.ElementAt(0).Split(';').ElementAt(14).TrimStart('0').Trim();

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
                        Separador = ';',
                        Extracto = new List<string>(pTemp),
                        TipoClase = typeof(Fiducoomeva)
                    });
                }
            }
            else
            {
                DiccionarioExtractos.Add(pLlaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { _producto, new Variables.DatosExtractos
                            {
                                Separador = ';',
                                Extracto = new List<string>(pTemp),
                                TipoClase = typeof(Fiducoomeva)
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

        /// <summary>
        /// Metodo que Formatea la data para el Sal.
        /// </summary>
        /// <param name="datosOriginales">Lista orginal</param>
        /// <returns>Lista Formateada</returns>
        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            #region FormatearArchivo
            List<string> resultado = new List<string>();

            List<string> campos;
            string linea;
            foreach (var lineaDatos in datosOriginales)
            {
                linea = Helpers.RemplazarCaracteres(';', '|', lineaDatos);
                linea = Helpers.TrimCamposLinea('|', linea);
                campos = linea.Split('|').ToList();

                switch (campos[0])
                {
                    case "001":
                        resultado.Add($"1FID|KITXXX|{campos[2].Substring(0,3)}|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "002":
                        resultado.Add($"1FIB|KITXXX|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "003":
                        resultado.Add($"1FIC|KITXXX|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "004":
                        resultado.Add($"1FDD|KITXXX|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "005":
                        resultado.Add($"1FIE|KITXXX|{Helpers.ValidarPipePipe(linea)}");
                        break;
                    case "006":
                        resultado.Add($"1FIF|KITXXX|{Helpers.ValidarPipePipe(linea)}");
                        break;
                }
            }
            return resultado; 
            #endregion
        }
    }
}
