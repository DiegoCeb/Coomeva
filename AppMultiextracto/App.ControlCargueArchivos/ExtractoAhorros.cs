using System;
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
    /// Clase que se encarga de cargar el archivo de ExtractoAhorros
    /// </summary>
    public class ExtractoAhorros : App.Variables.Variables, ICargue
    {
        private const string _producto = "ExtractoAhorros";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public ExtractoAhorros(string pArchivo)
        {
            #region ExtractoAhorros
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(ExtractoAhorros),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
            }
            #endregion
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public ExtractoAhorros()
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
            List<string> temp = new List<string>();
            string llaveCruce = string.Empty;

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea.Trim() == "@")
                {
                    temp.Add(linea);

                    llaveCruce = temp.ElementAt(1).Substring(49, 19).Trim();

                    AgregarDiccionario(llaveCruce, temp);

                    temp.Clear();
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
                        Separador = 'P',
                        Extracto = new List<string>(pTemp),
                        TipoClase = typeof(ExtractoAhorros)
                    });
                }
            }
            else
            {
                DiccionarioExtractos.Add(pLlaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { _producto, new Variables.DatosExtractos
                            {
                                Separador = 'P',
                                Extracto = new List<string>(pTemp),
                                TipoClase = typeof(ExtractoAhorros)
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

            List<PosCortes> listaCortes = new List<PosCortes>();
            string lineaDatos;
            string canalEnMapeo = string.Empty;
            int indiceExtracto = 0;

            for (int indice = 0; indice < datosOriginales.Count(); indice++)
            {
                lineaDatos = datosOriginales[indice];

                if (lineaDatos.Trim() == "@")
                {
                    indiceExtracto = 0;
                }
                else
                {
                    switch (indiceExtracto)
                    {
                        case 0:
                            canalEnMapeo = string.Empty;
                            listaCortes.Clear();
                            canalEnMapeo += $"1AEA|";
                            listaCortes.Add(new PosCortes(0, 40));
                            listaCortes.Add(new PosCortes(85, 0));
                            canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                            break;

                        case 1:
                            listaCortes.Clear();
                            listaCortes.Add(new PosCortes(0, 50));
                            listaCortes.Add(new PosCortes(50, 18));
                            listaCortes.Add(new PosCortes(69, 30));
                            listaCortes.Add(new PosCortes(100, 0));
                            canalEnMapeo += $"|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                            break;
                        case 2:
                            listaCortes.Clear();
                            listaCortes.Add(new PosCortes(0, 10));
                            listaCortes.Add(new PosCortes(11, 10));
                            listaCortes.Add(new PosCortes(21, 20));
                            listaCortes.Add(new PosCortes(41, 20));
                            listaCortes.Add(new PosCortes(61, 0));
                            canalEnMapeo += $"|{Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos)}";
                            canalEnMapeo += $"| | ";
                            resultado.Add($"{Helpers.ValidarPipePipe(canalEnMapeo)}");
                            break;

                        default:
                            canalEnMapeo = string.Empty;
                            int result = 0;
                            bool esDetalle = int.TryParse(lineaDatos.Substring(0, 2), out result);
                            if (esDetalle)
                            {
                                listaCortes.Clear();
                                listaCortes.Add(new PosCortes(0, 2));
                                listaCortes.Add(new PosCortes(3, 30));
                                listaCortes.Add(new PosCortes(34, 26));
                                listaCortes.Add(new PosCortes(60, 19));
                                listaCortes.Add(new PosCortes(79, 19));
                                listaCortes.Add(new PosCortes(98, 0));
                                canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                                resultado.Add($"1DEA|{Helpers.ValidarPipePipe(canalEnMapeo)}");
                            }
                            else
                            { }
                            break;
                    }

                    indiceExtracto++;
                }
            }
            
            return resultado;
            #endregion
        }
    }
}
