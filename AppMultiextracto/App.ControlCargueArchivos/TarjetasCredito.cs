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
    /// Clase que carga los datos puros del Producto TarjetasCredito
    /// </summary>
    public class TarjetasCredito: App.Variables.Variables, ICargue
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
                Helpers.EscribirLogVentana(ex.Message, true);
            }
            #endregion TarjetasCredito
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public TarjetasCredito()
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

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea.Length > 6)
                {
                    if (linea.Substring(0, 7) == "TARJETA")
                    {
                        llaveCruce = linea.Substring(20).Trim();

                        if (!DiccionarioExtractos.ContainsKey(llaveCruce))
                        {
                            DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = 'P',
                                    Extracto = new List<string>(){ linea},
                                    TipoClase = typeof(TarjetasCredito)
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
                                    Extracto = new List<string>() { linea },
                                    TipoClase = typeof(TarjetasCredito)
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
                        DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                    }
                }
                else
                {
                    DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
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

            List<PosCortes> listaCortes = new List<PosCortes>();
            string identificador;
            string canalEnMapeo = string.Empty;
            int ContadorDetalles = 0;
            foreach (var lineaDatos in datosOriginales)
            {
                identificador = lineaDatos.Substring(0, 1);
                switch (identificador)
                {
                    case "T":
                        canalEnMapeo = string.Empty;
                        listaCortes.Clear();
                        canalEnMapeo += $"1TAR| |";
                        listaCortes.Add(new PosCortes(0, 15));
                        listaCortes.Add(new PosCortes(15, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "1":
                        canalEnMapeo += $"|";
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "2":
                        resultado.Add(Helpers.ValidarPipePipe(canalEnMapeo));

                        canalEnMapeo = string.Empty;
                        canalEnMapeo += $"1TA2|";
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 25));
                        listaCortes.Add(new PosCortes(26, 20));
                        listaCortes.Add(new PosCortes(46, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "3":
                        canalEnMapeo += $"|";
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 40));
                        listaCortes.Add(new PosCortes(41, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "4":
                        canalEnMapeo += $"|";
                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "5":
                        if (!string.IsNullOrEmpty(canalEnMapeo))
                        { resultado.Add(Helpers.ValidarPipePipe(canalEnMapeo)); }

                        canalEnMapeo = string.Empty;

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 11));
                        listaCortes.Add(new PosCortes(12, 6));
                        listaCortes.Add(new PosCortes(18, 26));
                        listaCortes.Add(new PosCortes(44, 17));
                        listaCortes.Add(new PosCortes(61, 7));
                        listaCortes.Add(new PosCortes(68, 9));
                        listaCortes.Add(new PosCortes(77, 17));
                        listaCortes.Add(new PosCortes(94, 17));
                        

                        if (lineaDatos.Length < 135)
                        {
                            canalEnMapeo = Helpers.CompletarEspaciosLinea(lineaDatos, 120);

                            listaCortes.Add(new PosCortes(null, null));
                            listaCortes.Add(new PosCortes(111, 5));
                            listaCortes.Add(new PosCortes(116, 0));
                            
                        }
                        else
                        {
                            listaCortes.Add(new PosCortes(111, 16));
                            listaCortes.Add(new PosCortes(127, 5));
                            listaCortes.Add(new PosCortes(132, 0));
                            canalEnMapeo = lineaDatos;
                            

                        }

                        canalEnMapeo = Helpers.ExtraccionCamposSpool(listaCortes, canalEnMapeo);
                        canalEnMapeo += $"|{ObtenerCuotasPendientes(canalEnMapeo)}";
                        resultado.Add($"1TA3|{Helpers.ValidarPipePipe(canalEnMapeo)}");

                        ContadorDetalles++;
                        canalEnMapeo = string.Empty;
                        break;
                    case "6":
                        if (!string.IsNullOrEmpty(canalEnMapeo))
                        { resultado.Add(Helpers.ValidarPipePipe(canalEnMapeo)); }

                        canalEnMapeo = string.Empty;

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 17));
                        listaCortes.Add(new PosCortes(18, 17));
                        listaCortes.Add(new PosCortes(35, 17));
                        listaCortes.Add(new PosCortes(52, 17));
                        listaCortes.Add(new PosCortes(69, 17));
                        listaCortes.Add(new PosCortes(86, 17));
                        listaCortes.Add(new PosCortes(103, 17));
                        listaCortes.Add(new PosCortes(120, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add($"1TA4|{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        canalEnMapeo = string.Empty;
                        break;
                    case "7":
                        if (!string.IsNullOrEmpty(canalEnMapeo))
                        { resultado.Add(Helpers.ValidarPipePipe(canalEnMapeo)); }
                        canalEnMapeo = string.Empty;

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 13));
                        listaCortes.Add(new PosCortes(14, 13));
                        listaCortes.Add(new PosCortes(27, 16));
                        listaCortes.Add(new PosCortes(43, 16));
                        listaCortes.Add(new PosCortes(59, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add($"1TA5|{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        canalEnMapeo = string.Empty;
                        break;
                    case "8":
                        canalEnMapeo = string.Empty;

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add($"1TA6|{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        canalEnMapeo = string.Empty;
                        break;
                    case "9":
                        canalEnMapeo = string.Empty;
                        canalEnMapeo += $"1TA7|";

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "A":
                        canalEnMapeo += $"|";

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        break;
                    case "B":
                        canalEnMapeo += $"|";

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 4));
                        listaCortes.Add(new PosCortes(8, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add(Helpers.ValidarPipePipe(canalEnMapeo));
                        break;
                }

            }

            return resultado;
            #endregion
        }

        /// <summary>
        /// Metodo que calcula las Cuotas Pendintes
        /// </summary>
        /// <param name="linea">Campos del Detalle</param>
        /// <returns>campo de Cuotas Pendientes</returns>
        private string ObtenerCuotasPendientes(string linea)
        {
            #region ObtenerCuotasPendientes
            string[] campos = linea.Split('|');

            if ((!string.IsNullOrEmpty(campos[9].Trim())) && (!string.IsNullOrEmpty(campos[10].Trim())))
            {
                try
                {
                    int plazo = int.Parse(campos[9].Trim());
                    int cuotasPagadas = int.Parse(campos[10].Trim());
                    int cuotasPendintes = 0;

                    if (plazo >= cuotasPagadas)
                    {
                        cuotasPendintes = (plazo - cuotasPagadas);
                    }

                    return $"{cuotasPendintes.ToString().PadLeft(2, '0')}";

                }
                catch
                {
                    return "  ";
                }
            }
            else
            {
                return "  ";
            } 
            #endregion
        }
    }
}
