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
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));

                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
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

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            #region FormatearArchivo
            List<string> resultado = new List<string>();

            List<PosCortes> listaCortes = new List<PosCortes>();
            string identificador;
            string canalEnMapeo = string.Empty;
            foreach (var lineaDatos in datosOriginales)
            {
                identificador = lineaDatos.Substring(0, 1);
                switch (identificador)
                {
                    case "T":
                        canalEnMapeo = string.Empty;
                        canalEnMapeo += $"1TAR|KITXXX|";
                        listaCortes.Add(new PosCortes(0, 15));
                        listaCortes.Add(new PosCortes(15, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes,lineaDatos);
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
                        listaCortes.Add(new PosCortes(44, 16));
                        listaCortes.Add(new PosCortes(60, 7));
                        listaCortes.Add(new PosCortes(67, 9));
                        listaCortes.Add(new PosCortes(76, 17));
                        listaCortes.Add(new PosCortes(93, 17));
                        listaCortes.Add(new PosCortes(110, 16));
                        listaCortes.Add(new PosCortes(126, 5));
                        listaCortes.Add(new PosCortes(131, 5));
                        listaCortes.Add(new PosCortes(136, 0));
                        canalEnMapeo = Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add($"1TA3|{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        canalEnMapeo = string.Empty;
                        break;
                    case "6":
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
                        break;
                    case "7":
                        canalEnMapeo = string.Empty;

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 13));
                        listaCortes.Add(new PosCortes(14, 13));
                        listaCortes.Add(new PosCortes(27, 16));
                        listaCortes.Add(new PosCortes(43, 16));
                        listaCortes.Add(new PosCortes(59, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add($"1TA5|{Helpers.ValidarPipePipe(canalEnMapeo)}");
                        break;
                    case "8":
                        canalEnMapeo = string.Empty;

                        listaCortes.Clear();
                        listaCortes.Add(new PosCortes(1, 0));
                        canalEnMapeo += Helpers.ExtraccionCamposSpool(listaCortes, lineaDatos);
                        resultado.Add($"1TA6|{Helpers.ValidarPipePipe(canalEnMapeo)}");
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
    }
}
