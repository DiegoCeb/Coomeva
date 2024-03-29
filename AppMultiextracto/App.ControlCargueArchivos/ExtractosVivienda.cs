﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using App.Controlnsumos;

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
                DatosError StructError = new DatosError
                {
                    Clase = nameof(ExtractosVivienda),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);                
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

        /// <summary>
        /// Metodo que Formatea la data para el Sal.
        /// </summary>
        /// <param name="datosOriginales">Lista orginal</param>
        /// <returns>Lista Formateada</returns>
        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            #region FormatearArchivo
            List<string> resultado = new List<string>();
            List<string> resultadoEVB = new List<string>();

            List<PosCortes> listaCortes = new List<PosCortes>();
            string lineaDatos;
            string canalEnMapeo = string.Empty;
            int indiceExtracto = 0;

            for (int indice = 0; indice < datosOriginales.Count(); indice++)
            {
                lineaDatos = datosOriginales[indice];

                if (lineaDatos.Length >= 84 && lineaDatos.Substring(65,19).Trim() == "B A N C O O M E V A")
                {
                    indiceExtracto = 0;
                }
                
                switch (indiceExtracto)
                {
                    case 0:
                        canalEnMapeo = string.Empty;
                        resultadoEVB.Clear();
                        listaCortes.Clear();
                        canalEnMapeo += $"1ANV|";
                        listaCortes.Add(new PosCortes(65, 19));
                        listaCortes.Add(new PosCortes(112, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;

                    case 1:
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(54, 48));
                        listaCortes.Add(new PosCortes(109, 0));
                        canalEnMapeo += $"|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                        canalEnMapeo += $"| ";
                        resultado.Add($"{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        canalEnMapeo = string.Empty;
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(0, 87));
                        canalEnMapeo = $"1EVA|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                        resultado.Add($"{Helpers.ValidarPipePipe(canalEnMapeo)}");

                        canalEnMapeo = string.Empty;
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(88, 0));
                        canalEnMapeo = $"1EVB|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                        resultadoEVB.Add($"{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        break;
                    case 7:
                        canalEnMapeo = string.Empty;
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(0, 87));
                        canalEnMapeo = $"1EVA|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                        resultado.Add($"{Helpers.ValidarPipePipe(canalEnMapeo)}");

                        canalEnMapeo = string.Empty;
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(88, 0));
                        canalEnMapeo = $"1EVB|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                        resultadoEVB.Add($"{Helpers.ValidarPipePipe(canalEnMapeo)}");

                        resultado.AddRange(resultadoEVB);
                        resultadoEVB.Clear();
                        break;

                    default:
                                                        
                        resultado.Add($"1VDE|{Helpers.ValidarPipePipe(lineaDatos)}");
                        break;
                }

                indiceExtracto++;
            }

            return resultado;
            #endregion

        }
    }
}
