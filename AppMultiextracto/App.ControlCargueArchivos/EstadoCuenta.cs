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
    /// Clase que se encarga de cargar el archivo de EstadoCuenta
    /// </summary>
    public class EstadoCuenta : App.Variables.Variables, ICargue
    {
        private const string _producto = "EstadoCuenta";
        private string CedulaProceso = string.Empty;
        private double sumaCapitalVencido = 0.0;
        private double sumaCapitalMes = 0.0;
        private string saldoAportes = "0";
        private double saldoSolidaridad = 0.0;
        private double sumaInteresMora = 0.0;
        private string conceptoCruce = string.Empty;
        private string grupo = string.Empty;
        private string conceptoFinal = string.Empty;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public EstadoCuenta(string pArchivo)
        {
            #region EstadoCuenta
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Helpers.EscribirLogVentana(ex.Message, true);
            }
            #endregion
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public EstadoCuenta()
        { }

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
            bool extractoCompleto = false;
            string llaveCruce = string.Empty;

            while ((linea = lector.ReadLine()) != null)
            {
                if (linea == "-Detalle utilizaciones de cupos-" || linea.Contains("F Ben.Tasa Solidaria")|| linea == "@")
                {
                    continue;
                }

                if (linea.Contains("Código Asociado"))
                {
                    extractoCompleto = false;

                    if (temp.Count > 1)
                    {
                        extractoCompleto = true;
                    }

                    if (extractoCompleto)
                    {
                        llaveCruce = temp.ElementAt(1).Substring(79, 15).Trim();

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
                llaveCruce = temp.ElementAt(1).Substring(79, 15).Trim();

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
                        Separador = 'P',
                        Extracto = new List<string>(pTemp),
                        TipoClase = typeof(EstadoCuenta)
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
                                TipoClase = typeof(EstadoCuenta)
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

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            #region FormatearArchivo
            List<string> resultado = new List<string>();
            string result = string.Empty;

            resultado.Add(ArmarCanal("1AAA", datosOriginales));

            #region resultado.Add(ArmarCanal("1BBB", datosOriginales));
            var listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "M " select x;

            foreach (var detalle in listadoDatos.ToList())
            {
                result = ArmarCanal("1BBB", datosOriginales, detalle);

                if (!string.IsNullOrEmpty(result))
                {
                    resultado.Add(result);
                }
            }
            #endregion

            if (resultado.Exists(x => x.Contains("1BBB")))
            {
                resultado.Add(ArmarCanal("1BBA", datosOriginales));
            }

            #region resultado.Add(ArmarCanal("1CCC", datosOriginales));
            listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "M " select x;

            foreach (var detalle in listadoDatos.ToList())
            {
                result = ArmarCanal("1CCC", datosOriginales, detalle);

                if (!string.IsNullOrEmpty(result))
                {
                    resultado.Add(result);
                }
            }
            #endregion

            if (resultado.Exists(x => x.Contains("1CCC")))
            {
                #region resultado.Add(ArmarCanal("1CCB", datosOriginales));
                listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "T " && x.Contains('|') select x;

                foreach (var detalle in listadoDatos.ToList())
                {
                    result = ArmarCanal("1CCB", datosOriginales, detalle);

                    if (!string.IsNullOrEmpty(result))
                    {
                        resultado.Add(result);
                    }
                }
                #endregion
            }

            #region resultado.Add(ArmarCanal("1DDD", datosOriginales));
            listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "F " && !x.Contains("Ben.Tasa Solidaria") select x;

            foreach (var detalle in listadoDatos.ToList())
            {
                result = ArmarCanal("1DDD", datosOriginales, detalle);

                if (!string.IsNullOrEmpty(result))
                {
                    resultado.Add(result);
                }
            }
            #endregion

            if (resultado.Exists(x => x.Contains("1DDD")))
            {
                #region resultado.Add(ArmarCanal("1DDB", datosOriginales));
                listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "F " && x.Contains('|') select x;

                foreach (var detalle in listadoDatos.ToList())
                {
                    result = ArmarCanal("1DDB", datosOriginales, detalle);

                    if (!string.IsNullOrEmpty(result))
                    {
                        resultado.Add(result);
                    }
                }
                #endregion
            }

            #region resultado.Add(ArmarCanal("1EEE", datosOriginales));
            listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "R " select x;

            foreach (var detalle in listadoDatos.ToList())
            {
                result = ArmarCanal("1EEE", datosOriginales, detalle);

                if (!string.IsNullOrEmpty(result))
                {
                    resultado.Add(result);
                }
            }
            #endregion

            #region resultado.Add(ArmarCanal("1FFF", datosOriginales));
            listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "T " && x.Contains('|') select x;

            foreach (var detalle in listadoDatos.ToList())
            {
                result = ArmarCanal("1FFF", datosOriginales, detalle);

                if (!string.IsNullOrEmpty(result))
                {
                    resultado.Add(result);
                }
            }
            #endregion

            #region resultado.Add(ArmarCanal("1GGG", datosOriginales));
            if (InsumoPlanoBeneficios.ContainsKey(CedulaProceso))
            {
                foreach (var detalles in InsumoPlanoBeneficios[CedulaProceso].InsumoLinea)
                {
                    ArmarCanal("1GGG", datosOriginales, detalles);
                }

                foreach (var datosBeneficios in EstructuraBeneficios.Values)
                {
                    resultado.Add(datosBeneficios.Formato);
                }

                //Solo se obtiene el primero por que todos tienen los mismos totales
                resultado.Add(ArmarCanal("1GGA", datosOriginales, InsumoPlanoBeneficios[CedulaProceso].InsumoLinea.FirstOrDefault()));
            }
            #endregion

            //resultado.Add(ArmarCanal("1III", datosOriginales));

            #region resultado.Add(ArmarCanal("1JJJ", datosOriginales));
            //result = ArmarCanal("1JJJ", datosOriginales);

            //if (!string.IsNullOrEmpty(result))
            //{
            //    resultado.Add(result);
            //}
            #endregion

            #region resultado.Add(ArmarCanal("1KKK", datosOriginales));
            //listadoDatos = from x in datosOriginales
            //               where x.Contains("Sr.Usuario") ||
            //               x.Contains("Fecha limite de pago") ||
            //               x.Contains("Tenga la tranquilidad") ||
            //               x.Contains("Si realiza su pago") ||
            //               x.Contains("** ")
            //               select x;

            //foreach (var detalle in listadoDatos.ToList())
            //{
            //    result = ArmarCanal("1KKK", datosOriginales, detalle);

            //    if (!string.IsNullOrEmpty(result))
            //    {
            //        resultado.Add(result);
            //    }
            //}
            #endregion

            resultado.Add(ArmarCanal("1PR2", datosOriginales));

            #region resultado.Add(ArmarCanal("1PRU", datosOriginales));
            listadoDatos = from x in datosOriginales where x.Length > 1 && x.Substring(0, 2) == "U " select x;

            foreach (var detalle in listadoDatos.ToList())
            {
                result = ArmarCanal("1PRU", datosOriginales, detalle);

                if (!string.IsNullOrEmpty(result))
                {
                    resultado.Add(result);
                }
            }
            #endregion

            return resultado;
            #endregion
        }

        private string ArmarCanal(string pCanal, List<string> pDatos, string pLineaDetalle = "")
        {
            #region ArmarCanal
            string resultado = string.Empty;

            List<PosCortes> listaCortes = new List<PosCortes>();
            string condicionBusqueda = string.Empty;
            int indiceDatoBuscado = 0;
            string cuentaAhorro = string.Empty;
            string fechaOtrasEntidades = string.Empty;
            string fechaServibanca = string.Empty;
            string fechaBancoomeva = string.Empty;
            string referencia = string.Empty;
            string totalPagar = string.Empty;

            switch (pCanal)
            {
                case "1AAA":
                    #region 1AAA
                    CedulaProceso = pDatos.ElementAt(1).Substring(80, 14).Trim();

                    if (CedulaProceso == "35199646")
                    {

                    }

                    listaCortes.Add(new PosCortes(11, 40));
                    listaCortes.Add(new PosCortes(0, 11));

                    #region Busqueda
                    condicionBusqueda = (from x in pDatos where x.Length > 3 && x.Substring(0, 3) == "** " select x).FirstOrDefault();

                    if (!string.IsNullOrEmpty(condicionBusqueda))
                    {
                        indiceDatoBuscado = pDatos.FindIndex(x => x == condicionBusqueda);

                        cuentaAhorro = pDatos.ElementAt(indiceDatoBuscado + 4);
                    }
                    else
                    {
                        cuentaAhorro = " ";
                    }
                    #endregion

                    resultado = $"1AAA| |{ Helpers.ExtraccionCamposSpool(listaCortes, pDatos.ElementAt(1))}|{cuentaAhorro}";
                    listaCortes.Clear();

                    listaCortes.Add(new PosCortes(80, 14)); //Cedula
                    listaCortes.Add(new PosCortes(60, 5));
                    listaCortes.Add(new PosCortes(94, 0));
                    listaCortes.Add(new PosCortes(65, 14));

                    resultado += $"|{ Helpers.ExtraccionCamposSpool(listaCortes, pDatos.ElementAt(1))}";
                    listaCortes.Clear();

                    listaCortes.Add(new PosCortes(46, 10));

                    #region Busqueda
                    condicionBusqueda = (from x in pDatos where x.Contains("Fecha limite de pago en bancos hasta el") select x).FirstOrDefault();

                    if (!string.IsNullOrEmpty(condicionBusqueda))
                    {
                        if (condicionBusqueda.Length > 40)
                        {
                            fechaOtrasEntidades = condicionBusqueda.Substring(40);
                        }
                        else
                        {
                            fechaOtrasEntidades = " ";
                        }
                    }

                    condicionBusqueda = (from x in pDatos where x.Contains("Fecha limite de pago en cajeros automaticos Servibanca") select x).FirstOrDefault();

                    if (!string.IsNullOrEmpty(condicionBusqueda))
                    {
                        fechaServibanca = condicionBusqueda.Substring(55);
                    }
                    else
                    {
                        fechaServibanca = " ";
                    }

                    referencia = pDatos.ElementAt(pDatos.Count - 1).Split('(').ElementAt(2).Substring(5);
                    totalPagar = $"{Convert.ToInt64(pDatos.ElementAt(pDatos.Count - 1).Split('(').ElementAt(3).Substring(5)):N0}".Replace('.', ',');

                    #endregion

                    resultado += $"|{ Helpers.ExtraccionCamposSpool(listaCortes, pDatos.ElementAt(0))}|{fechaOtrasEntidades}|{fechaServibanca}" +
                        $"|{referencia}|{pDatos.ElementAt(pDatos.Count - 1)}|{totalPagar}";
                    listaCortes.Clear();

                    if (InsumoEtiquetasFisico.ContainsKey(CedulaProceso))
                    {
                        #region Fisico
                        listaCortes.Add(new PosCortes(31, 60));

                        resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, InsumoEtiquetasFisico[CedulaProceso].InsumoLinea.FirstOrDefault())}|{BuscarCiudadDpto(InsumoEtiquetasFisico)}";
                        listaCortes.Clear();

                        listaCortes.Add(new PosCortes(135, 2));

                        resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, InsumoEtiquetasFisico[CedulaProceso].InsumoLinea.FirstOrDefault())}";
                        listaCortes.Clear();
                        #endregion
                    }
                    else if (InsumoEtiquetasMail.ContainsKey(CedulaProceso))
                    {
                        #region Mail
                        listaCortes.Add(new PosCortes(31, 60));

                        resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, InsumoEtiquetasMail[CedulaProceso].InsumoLinea.FirstOrDefault())}|{BuscarCiudadDpto(InsumoEtiquetasMail)}";
                        listaCortes.Clear();

                        listaCortes.Add(new PosCortes(135, 2));

                        resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, InsumoEtiquetasMail[CedulaProceso].InsumoLinea.FirstOrDefault())}";
                        listaCortes.Clear();
                        #endregion
                    }

                    #region Busqueda
                    condicionBusqueda = (from x in pDatos where x.Contains("Fecha limite de pago en Coomeva") select x).FirstOrDefault();

                    if (!string.IsNullOrEmpty(condicionBusqueda))
                    {
                        if (condicionBusqueda.Length > 41)
                        {
                            fechaBancoomeva = condicionBusqueda.Substring(41);
                        }
                        else
                        {
                            fechaBancoomeva = " ";
                        }
                    }

                    #endregion

                    if (pDatos.ElementAt(0).Length > 56)
                    {
                        listaCortes.Add(new PosCortes(57, 0));
                        listaCortes.Add(new PosCortes(67, 0));

                        resultado += $"|{fechaBancoomeva}|{Helpers.ExtraccionCamposSpool(listaCortes, pDatos.ElementAt(0))}| | ";
                        listaCortes.Clear();
                    }
                    else
                    {
                        resultado += $"|{fechaBancoomeva} | | | | ";
                    }

                    #endregion
                    break;

                case "1BBB":
                    #region 1BBB
                    if (!pLineaDetalle.Contains('|'))
                    {
                        #region Detalles
                        BuscarConceptosDiccionario(pLineaDetalle, 184, 0);

                        if (grupo == "Estatutarios")
                        {
                            sumaCapitalVencido += pLineaDetalle.Substring(49, 14).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(49, 14).Trim()) : 0.0;
                            sumaCapitalMes += pLineaDetalle.Substring(63, 28).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(63, 28).Trim()) : 0.0;
                            sumaInteresMora += pLineaDetalle.Substring(118, 13).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(118, 13).Trim()) : 0.0;

                            if (conceptoFinal == "Aportes")
                            {
                                saldoAportes = pLineaDetalle.Substring(32, 15).Trim();
                            }
                            else if (conceptoFinal.Contains("Solidaridad"))
                            {
                                saldoSolidaridad += pLineaDetalle.Substring(32, 15).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(32, 15).Trim()) : 0.0;
                            }

                            listaCortes.Add(new PosCortes(49, 14));  //Capital Vencido
                            listaCortes.Add(new PosCortes(63, 28));  //Capital Mes
                            listaCortes.Add(new PosCortes(119, 8));  //Interes Mora
                            listaCortes.Add(new PosCortes(127, 14)); //Valor a pagar

                            resultado = $"1BBB|{conceptoFinal}|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}";
                        }
                        #endregion
                    }
                    #endregion
                    break;

                case "1BBA":
                    #region 1BBA
                    resultado = $"1BBA|{sumaCapitalVencido:N0}|{sumaCapitalMes:N0}|{sumaInteresMora}|{saldoAportes}|{saldoSolidaridad:N0}|0".Replace('.', ',');
                    sumaCapitalVencido = 0.0;
                    sumaCapitalMes = 0.0;
                    #endregion
                    break;

                case "1CCC":
                    #region 1CCC
                    if (!pLineaDetalle.Contains('|'))
                    {
                        #region Detalles
                        BuscarConceptosDiccionario(pLineaDetalle, 184, 0);

                        if (grupo == "Creditos Cooperativos" || grupo == "Planes Adicionales Solidaridad" || grupo == "Seguros" || grupo == "Medicina Prepagada")
                        {
                            sumaCapitalVencido += pLineaDetalle.Substring(49, 14).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(49, 14).Trim()) : 0.0;
                            sumaCapitalVencido += pLineaDetalle.Substring(63, 14).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(63, 14).Trim()) : 0.0;
                            sumaCapitalMes += pLineaDetalle.Substring(70, 14).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(70, 14).Trim()) : 0.0;
                            sumaCapitalMes += pLineaDetalle.Substring(91, 14).Trim() != "" ? Convert.ToDouble(pLineaDetalle.Substring(91, 14).Trim()) : 0.0;

                            listaCortes.Add(new PosCortes(49, 14));  //Capital Vencido
                            listaCortes.Add(new PosCortes(63, 14));  //Financiacion Vencida
                            listaCortes.Add(new PosCortes(70, 21));  //Capital Mes
                            listaCortes.Add(new PosCortes(91, 14));  //Financiacion Mes
                            listaCortes.Add(new PosCortes(121, 6));  //Interes Mora
                            listaCortes.Add(new PosCortes(127, 14)); //Valor a Pagar
                            listaCortes.Add(new PosCortes(156, 3));  //Plazo Pactado
                            listaCortes.Add(new PosCortes(159, 3));  //Plazo Pendiente
                            listaCortes.Add(new PosCortes(162, 12)); //Tasa
                            listaCortes.Add(new PosCortes(32, 15));  //Saldo Capital Anterior
                            listaCortes.Add(new PosCortes(141, 15)); //Saldo Capital Posterior

                            resultado = $"1CCC|{grupo}|{conceptoFinal}|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}";
                        }
                        #endregion
                    }
                    #endregion
                    break;

                case "1CCB":
                    #region 1CCB

                    string interesMora = "0";
                    string valorPagar = "0";

                    if (!string.IsNullOrEmpty(pLineaDetalle.Split('|').ElementAt(4).Trim()))
                    {
                        interesMora = pLineaDetalle.Split('|').ElementAt(4).Trim();
                    }

                    if (!string.IsNullOrEmpty(pLineaDetalle.Split('|').ElementAt(5).Trim()))
                    {
                        valorPagar = pLineaDetalle.Split('|').ElementAt(5).Trim();
                    }

                    resultado = $"1CCB|{sumaCapitalVencido:N0}|{sumaCapitalMes:N0}|{interesMora}|{valorPagar}".Replace('.', ',');
                    sumaCapitalVencido = 0.0;
                    sumaCapitalMes = 0.0;
                    #endregion
                    break;

                case "1DDD":
                    #region 1DDD
                    if (!pLineaDetalle.Contains('|'))
                    {
                        BuscarConceptosDiccionario(pLineaDetalle, 184, 0);

                        listaCortes.Add(new PosCortes(22, 10));   //Forma de Pago
                        listaCortes.Add(new PosCortes(32, 15));   //Saldo capital Anterior
                        listaCortes.Add(new PosCortes(49, 14));   //Capital Vencido
                        listaCortes.Add(new PosCortes(63, 14));   //Financiacion Vencida
                        listaCortes.Add(new PosCortes(77, 14));   //Capital Mes
                        listaCortes.Add(new PosCortes(91, 14));   //Financiacion Mes
                        listaCortes.Add(new PosCortes(116, 11));  //Interes Mora
                        listaCortes.Add(new PosCortes(105, 11));  //Total cuentas por cobrar
                        listaCortes.Add(new PosCortes(127, 14));  //Total Pagar
                        listaCortes.Add(new PosCortes(141, 15));  //Saldo Capital Posterior
                        listaCortes.Add(new PosCortes(156, 3));   //Plazo Pactado
                        listaCortes.Add(new PosCortes(159, 3));   //Plazo Pendiente
                        listaCortes.Add(new PosCortes(162, 12));  //Tasa

                        resultado = $"1DDD|{conceptoFinal}|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}";
                    }
                    #endregion
                    break;

                case "1DDB":
                    #region 1DDB
                    resultado = $"1DDB| |{pLineaDetalle.Split('|').ElementAt(1).Trim()}|{pLineaDetalle.Split('|').ElementAt(2).Trim()}|{pLineaDetalle.Split('|').ElementAt(3).Trim()}|{pLineaDetalle.Split('|').ElementAt(4).Trim()}|{pLineaDetalle.Split('|').ElementAt(5).Trim()}|{pLineaDetalle.Split('|').ElementAt(6).Trim()}|{pLineaDetalle.Split('|').ElementAt(0).Substring(1).Trim()}".Replace("||", "| |").Replace("||", "| |");
                    #endregion
                    break;

                case "1EEE":
                    #region 1EEE
                    if (!pLineaDetalle.Contains('|'))
                    {
                        BuscarConceptosDiccionario(pLineaDetalle, 2, 15);

                        listaCortes.Add(new PosCortes(17, 15));

                        resultado = $"1EEE|{conceptoFinal}|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}| | | ";
                        listaCortes.Clear();

                        listaCortes.Add(new PosCortes(77, 15));

                        resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}| | | ";
                        listaCortes.Clear();

                        listaCortes.Add(new PosCortes(137, 15));
                        listaCortes.Add(new PosCortes(152, 15));

                        resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}";
                        listaCortes.Clear();
                    }
                    #endregion
                    break;

                case "1FFF":
                    #region 1FFF
                    resultado = $"1FFF|{pLineaDetalle.Split('|').ElementAt(0).Substring(1).Trim()}|{pLineaDetalle.Split('|').ElementAt(1).Trim()}|{pLineaDetalle.Split('|').ElementAt(2).Trim()}|{pLineaDetalle.Split('|').ElementAt(3).Trim()}|{pLineaDetalle.Split('|').ElementAt(4).Trim()}|{pLineaDetalle.Split('|').ElementAt(5).Trim()}|{pLineaDetalle.Split('|').ElementAt(6).Trim()}".Replace("||", "| |").Replace("||", "| |");
                    #endregion
                    break;

                case "1GGG":
                    #region 1GGG
                    string producto = pLineaDetalle.Split('\t').ElementAt(5).Trim();

                    if (EstructuraBeneficios.ContainsKey(producto))
                    {
                        EstructuraBeneficios[producto].Formato = $"1GGG|{pLineaDetalle.Split('\t').ElementAt(4).Trim()}|{producto}|{pLineaDetalle.Split('\t').ElementAt(6).Trim()}|{pLineaDetalle.Split('\t').ElementAt(7).Trim()}|{pLineaDetalle.Split('\t').ElementAt(8).Trim()}| ".Replace("||", "| |").Replace("||", "| |");
                    }
                    #endregion
                    break;

                case "1GGA":
                    #region 1GGA
                    resultado = $"1GGA|{pLineaDetalle.Split('\t').ElementAt(9).Trim()}|{pLineaDetalle.Split('\t').ElementAt(10).Trim()}|{pLineaDetalle.Split('\t').ElementAt(1).Trim()}";
                    #endregion
                    break;

                case "1III":
                    #region 1III
                    resultado = $"1III|{pDatos.ElementAt(pDatos.Count - 1)}";
                    #endregion
                    break;

                case "1JJJ":
                    #region 1JJJ
                    string OficinaPago = (from x in pDatos where x.Contains("OF.COOMEVA") select x).FirstOrDefault();

                    if (!string.IsNullOrEmpty(OficinaPago))
                    {
                        resultado = $"1JJJ|{OficinaPago}";
                    }
                    #endregion
                    break;

                case "1KKK":
                    #region 1KKK
                    resultado = $"1KKK|{pLineaDetalle}";
                    #endregion
                    break;

                case "1PR2":
                    #region 1PR2
                    string fechaHasta = (from x in pDatos
                                         where x.Length > 1 && x.Substring(0, 2) == "T " && x.Contains('|')
                                         select x).FirstOrDefault().Split('|').ElementAt(6).Trim().Replace("/", "-");

                    string fechadesde = $"{Convert.ToDateTime(fechaHasta).AddMonths(-1):yyyy-MM-dd}";

                    resultado = $"1PR2|{fechadesde}|{fechaHasta}";
                    #endregion
                    break;

                case "1PRU":
                    #region 1PRU

                    listaCortes.Add(new PosCortes(2, 11));
                    listaCortes.Add(new PosCortes(41, 11));
                    listaCortes.Add(new PosCortes(52, 16));
                    listaCortes.Add(new PosCortes(68, 4));
                    listaCortes.Add(new PosCortes(72, 4));
                    listaCortes.Add(new PosCortes(76, 10));
                    listaCortes.Add(new PosCortes(86, 18));

                    resultado = $"1PRU|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}| ";
                    listaCortes.Clear();

                    if (pLineaDetalle.Length > 125)
                    {
                        listaCortes.Add(new PosCortes(125, 15));
                    }
                    

                    if (pLineaDetalle.Length > 140)
                    {
                        listaCortes.Add(new PosCortes(140, 18));
                    }
                   
                    if (pLineaDetalle.Length > 158)
                    {
                        listaCortes.Add(new PosCortes(158, 18));
                    }
                   

                    resultado += $"|{Helpers.ExtraccionCamposSpool(listaCortes, pLineaDetalle)}";
                    #endregion
                    break;
            }

            return Helpers.ValidarPipePipe(resultado);
            #endregion
        }

        private string BuscarCiudadDpto(Dictionary<string, Variables.DatosInsumos> pDatos)
        {
            #region BuscarCiudadDpto
            string segmentoCiudadDpto = pDatos[CedulaProceso].InsumoLinea.FirstOrDefault().Substring(92, 30).Trim();
            string ciudad = segmentoCiudadDpto.LastIndexOf('(') != -1 ? segmentoCiudadDpto.Substring(0, segmentoCiudadDpto.LastIndexOf('(')).Trim() : segmentoCiudadDpto;
            string dpto = segmentoCiudadDpto.LastIndexOf('(') != -1 ? segmentoCiudadDpto.Substring(segmentoCiudadDpto.LastIndexOf('(')).Trim().Replace("(", "").Replace(")", "") : segmentoCiudadDpto;
            if (ciudad.ToUpper() == "BOGOTA D.C.")
            {
                ciudad = "BOGOTA D.C.";
                dpto = "BOGOTA D.C.";
            }

            return $"{ciudad}|{dpto}";
            #endregion
        }

        private void BuscarConceptosDiccionario(string pLineaDetalle, int pCorteIncial, int pCantidad)
        {
            #region BuscarConceptosDiccionario
            if (pCantidad == 0)
            {
                conceptoCruce = pLineaDetalle.Substring(pCorteIncial).Trim();
            }
            else
            {
                conceptoCruce = pLineaDetalle.Substring(pCorteIncial, pCantidad).Trim();
            }

            grupo = string.Empty;
            conceptoFinal = string.Empty;

            if (InsumoDiccionarioDatos.ContainsKey(conceptoCruce))
            {
                var separador = InsumoDiccionarioDatos[conceptoCruce].Separador;

                grupo = InsumoDiccionarioDatos[conceptoCruce].InsumoLinea.FirstOrDefault().Split(separador).ElementAt(2);
                conceptoFinal = InsumoDiccionarioDatos[conceptoCruce].InsumoLinea.FirstOrDefault().Split(separador).ElementAt(4);
            }
            else
            {
                //Diccionario no tiene concepto se dja el original.
                conceptoFinal = conceptoCruce;
            }
            #endregion
        }

    }
}
