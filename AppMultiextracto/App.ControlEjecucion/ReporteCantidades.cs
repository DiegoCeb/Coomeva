using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppVariables = App.Variables.Variables;
using App.Controlnsumos;
using System.IO;

namespace App.ControlEjecucion
{
    /// <summary>
    /// Clase para Extraccion de Muestras
    /// </summary>
    public class ReporteCantidades
    {
        private bool _disposed = false;
        private string _rutaSalidaCarpeta = string.Empty;
        private string _rutaSalidaReporteCantidades = string.Empty;
        private string _rutaSalidaCheckList = string.Empty;
        private string _rutaSalidaReporteGuias = string.Empty;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ReporteCantidades()
        {
            #region ReporteCantidades
            ExraerCantidades();
            _rutaSalidaCarpeta = Directory.CreateDirectory($@"{Helpers.RutaProceso}\Reportes").FullName;
            _rutaSalidaReporteCantidades = $@"{_rutaSalidaCarpeta}\ReporteCantidades_{AppVariables.Orden}.csv";
            _rutaSalidaCheckList = $@"{_rutaSalidaCarpeta}\CheckList_{AppVariables.Orden}.csv";
            _rutaSalidaReporteGuias = $@"{_rutaSalidaCarpeta}\ReporteGuias_{AppVariables.Orden}.csv";
            GenerarArchivosCantidades();
            GenerarArchivoCheckList();
            GenerarReporteGuias();
            #endregion
        }

        /// <summary>
        /// Generar el reporte de guias por Codigo de Courier
        /// </summary>
        private void GenerarReporteGuias()
        {
            #region CargarGuias

            List<string> reporteGuias = new List<string>() { "Nombre Archivo;Nombre Courier;Cod Curier;kit Inicial;kit Final" };
            string kitInicial = string.Empty;
            string kitFinal = string.Empty;
            string nombreCourier = string.Empty;
            string codCurier = string.Empty;

            string[] rutaGuias = Directory.GetFiles(Variables.Variables.RutaBaseDelta);

            foreach (string archivo in rutaGuias)
            {
                string nombreArchivo = Path.GetFileNameWithoutExtension(archivo);
                string extension = Path.GetExtension(archivo);

                if (nombreArchivo != null && (extension != null && (extension.ToLower() == ".sal" && nombreArchivo.Contains("guias"))))
                {
                    Variables.Variables.Lector = new StreamReader(archivo, Encoding.Default);
                    Dictionary<string, string> dicGuiasTemp = new Dictionary<string, string>();
                    string Linea = string.Empty;
                    string[] campos = null;

                    while ((Linea = Variables.Variables.Lector.ReadLine()) != null)
                    {
                        campos = Linea.Split('|');

                        if (campos[2] != codCurier)
                        {
                            if (string.IsNullOrEmpty(codCurier))
                            {
                                codCurier = campos[2];
                                nombreCourier = campos[3];
                                kitInicial = campos[1];
                                kitFinal = campos[1];

                            }
                            else
                            {
                                reporteGuias.Add($"{nombreArchivo};{nombreCourier};{codCurier};{kitInicial};{kitFinal}");

                                codCurier = campos[2];
                                nombreCourier = campos[3];
                                kitInicial = campos[1];
                                kitFinal = campos[1];
                            }
                        }
                        else
                        {
                            kitFinal = campos[1];
                        }

                        
                    }

                    reporteGuias.Add($"{nombreArchivo};{nombreCourier};{codCurier};{kitInicial};{kitFinal}");
                    kitInicial = string.Empty;
                    kitFinal = string.Empty;
                    nombreCourier = string.Empty;
                    codCurier = string.Empty;

                    Variables.Variables.Lector.Close();
                }
            }

            Helpers.EscribirEnArchivo($@"{_rutaSalidaReporteGuias}", reporteGuias);

            #endregion
        }

        /// <summary>
        /// Metodo que extraer las cantidades por producto
        /// </summary>
        private void ExraerCantidades()
        {
            #region ExraerCantidades
            int cantidadExtractos = 0;
            string regional = "Virtual";
            foreach (var extractoCedula in AppVariables.DiccionarioExtractosFormateados)
            {
                foreach (var tipoExtracto in extractoCedula.Value)
                {
                    if (tipoExtracto.Key == regional)
                    {
                        ReporteCantidades.ExraerCantidades(new KeyValuePair<string, List<string>>("Extractos", new List<string>() { $"1MUL| | | " }), regional);

                        foreach (var paqueteExtracto in tipoExtracto.Value)
                        {
                            cantidadExtractos = ContarExtractos(paqueteExtracto.Key, paqueteExtracto.Value);

                            if (AppVariables.ReporteCantidades.ContainsKey(regional))
                            {
                                if (AppVariables.ReporteCantidades[regional].ContainsKey(paqueteExtracto.Key))
                                {
                                    AppVariables.ReporteCantidades[regional][paqueteExtracto.Key] += cantidadExtractos;
                                }
                                else
                                {
                                    AppVariables.ReporteCantidades[regional].Add(paqueteExtracto.Key, cantidadExtractos);
                                }
                            }
                            else
                            {
                                Dictionary<string, int> dicTipo = new Dictionary<string, int>();
                                dicTipo.Add(paqueteExtracto.Key, cantidadExtractos);

                                AppVariables.ReporteCantidades.Add(regional, dicTipo);
                            }

                        }
                    }
                }
            } 
            #endregion
        }

        /// <summary>
        /// Metodo que cuenta los porductos por extracto
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int ContarExtractos(string key, List<string> value)
        {
            #region ContarExtractos
            int contador = 0;
            string[] campos;

            foreach (string val in value)
            {
                campos = val.Split('|');

                switch (key)
                {
                    case "Extractos":
                        if (campos[0] == "1MUL")
                        {
                            contador++;
                        }
                        break;

                    case "EstadoCuenta":
                        if (campos[0] == "1AAA")
                        {
                            contador++;
                        }
                        break;

                    case "ExtractoAhorros":
                        if (campos[0] == "1AEA")
                        {
                            contador++;
                        }
                        break;

                    case "TarjetasCredito":
                        if (campos[0] == "1TAR")
                        {
                            contador++;
                        }
                        break;

                    case "ExtractosFundacion":
                        if (campos[0] == "1MIC")
                        {
                            contador++;
                        }
                        break;

                    case "ExtractosRotativo":
                        if (campos[0] == "1ROT")
                        {
                            contador++;
                        }
                        break;

                    case "ExtractosVivienda":
                        if (campos[0] == "1ANV")
                        {
                            contador++;
                        }
                        break;

                    case "Libranza":
                        if (campos[0] == "1ANV")
                        {
                            contador++;
                        }
                        break;

                    case "Fiducoomeva":
                        if (campos[0] == "1FID")
                        {
                            contador++;
                        }
                        break;

                    case "ActivacionProtecciones":
                        if (campos[0] == "1ACP")
                        {
                            contador++;
                        }
                        break;

                    case "CartasCobranzaHabeasData":
                        if (campos[0] == "1CCH")
                        {
                            contador++;
                        }
                        break;

                    case "HabeasData":
                        if (campos[0] == "1HAB")
                        {
                            contador++;
                        }
                        break;

                    case "CartasTAC":
                        if (campos[0] == "1UUU")
                        {
                            contador++;
                        }
                        break;

                    default:
                        if (campos[0] == "")
                        {
                            contador++;
                        }
                        break;
                }
            }

            return contador; 
            #endregion
        }

        /// <summary>
        /// Metodo que lamacena en el diccionario las cantidades por producto
        /// </summary>
        /// <param name="pExtracto">Diccionario de extracto</param>
        /// <param name="pRegional">Regional</param>
        public static void ExraerCantidades(KeyValuePair<string, List<string>> pExtracto, string pRegional)
        {
            #region ExraerCantidades

            string producto = pExtracto.Key;

            int cantidadExtractos = ContarExtractos(producto, pExtracto.Value);

            if (AppVariables.ReporteCantidades.ContainsKey(pRegional))
            {
                if (AppVariables.ReporteCantidades[pRegional].ContainsKey(producto))
                {
                    AppVariables.ReporteCantidades[pRegional][producto] += cantidadExtractos;
                }
                else
                {
                    AppVariables.ReporteCantidades[pRegional].Add(producto, cantidadExtractos);
                }
            }
            else
            {
                Dictionary<string, int> dicTipo = new Dictionary<string, int>();
                dicTipo.Add(producto, cantidadExtractos);

                AppVariables.ReporteCantidades.Add(pRegional, dicTipo);
            } 
            #endregion
        }

        /// <summary>
        /// Genera Archivo de reporte de cantidades por regional
        /// </summary>
        private void GenerarArchivosCantidades()
        {
            #region GenerarArchivosCantidades
            List<string> registrosCantidades = new List<string>() { "Producto;Extractos;Estado de Cuenta;Depositos;Tarjetas de Credito;Extractos Fundación;Rotativo;Vivienda;Libranza;Fiducoomeva;ActivacionProtecciones;CartasCobranzaHabeasData;HabeasData;CartasTAC" };

            foreach (string regional in AppVariables.ReporteCantidades.Keys)
            {
                registrosCantidades.Add(regional);
                AppVariables.CheckListProceso.CantidadesProducto.Extractos.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "Extractos");
                AppVariables.CheckListProceso.CantidadesProducto.EstadoCuenta.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "EstadoCuenta");
                AppVariables.CheckListProceso.CantidadesProducto.Despositos.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "ExtractoAhorros");
                AppVariables.CheckListProceso.CantidadesProducto.TarjetasCredito.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "TarjetasCredito");
                AppVariables.CheckListProceso.CantidadesProducto.ExtractosFundacion.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "ExtractosFundacion");
                AppVariables.CheckListProceso.CantidadesProducto.ExtractosRotativo.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "ExtractosRotativo");
                AppVariables.CheckListProceso.CantidadesProducto.ExtractosVivienda.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "ExtractosVivienda");
                AppVariables.CheckListProceso.CantidadesProducto.Libranza.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "Libranza");
                AppVariables.CheckListProceso.CantidadesProducto.Fiducoomeva.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "Fiducoomeva");
                AppVariables.CheckListProceso.CantidadesProducto.ActivacionProtecciones.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "ActivacionProtecciones");
                AppVariables.CheckListProceso.CantidadesProducto.CartasCobranzaHabeasData.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "CartasCobranzaHabeasData");
                AppVariables.CheckListProceso.CantidadesProducto.HabeasData.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "HabeasData");
                AppVariables.CheckListProceso.CantidadesProducto.CartasTAC.MesActual += AgregarProductoReporteCantidades(registrosCantidades, regional, "CartasTAC");

            }

            Helpers.EscribirEnArchivo($@"{_rutaSalidaReporteCantidades}", registrosCantidades); 
            #endregion
        }

        /// <summary>
        /// Orden y genera el archivo de Check List
        /// </summary>
        private void GenerarArchivoCheckList()
        {
            #region GenerarArchivoCheckList
            List<string> checkList = new List<string>();

            AppVariables.CheckListProceso.CalcularPorcentajesCantidadesArchivos();
            AppVariables.CheckListProceso.CalcularPorcentajesCantidadesProducto();

            checkList.Add("Título:;CHECK LIST ESTADOS CUENTA");
            checkList.Add(string.Empty);
            checkList.Add($"Fecha Inicio Proceso:;{ AppVariables.CheckListProceso.FechaHoraIncio}");
            checkList.Add($"Fecha Final Proceso:;{ AppVariables.CheckListProceso.FechaHoraFin}");
            checkList.Add($"Usuarios Sesion:;{ AppVariables.CheckListProceso.UsuarioSesion}");
            checkList.Add(string.Empty);
            checkList.Add($"Corte:;{ AppVariables.CheckListProceso.Corte}");
            checkList.Add(string.Empty);
            checkList.Add($"Nombre archivo Trasmitido;Peso Archivos Mes Anterior;Peso Archivos Mes Actual;Diferencia");
            foreach (var cantidaArchivo in AppVariables.CheckListProceso.DiccionarioCantidadesArchivos.Values)
            {
                checkList.Add($"{cantidaArchivo.NombreArchivo};{cantidaArchivo.PesoArchivoMesAnterior};{cantidaArchivo.PesoArchivoMesActual};{cantidaArchivo.DiferenciaPesoArchivo}");
            }
            checkList.Add(string.Empty);
            checkList.Add($"3. ENVIO MUESTRAS;a.) Se envia muestras fisicas de las cedulas que crucen con el archivo de muestras que se encuentra en la raiz de coomeva. b.) Se envian muestras virtuales de la carpeta que se genera con el procesmiento llamada : Muestras_Cliente_NumeroOrden Hay se encuentra una carpeta llamada muestras coomeva virtual donde esta los PDFs que se enviaran al cliente. c.) Dejar las muestras por carpetas separadas, no juntas y especificar las rutas de cada carpeta al cliente.");
            checkList.Add(string.Empty);
            checkList.Add($"Producto;Cantidad Mes Anterior;CantidadMes Actual;Diferencia");
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.Extractos));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.EstadoCuenta));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.Despositos));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.TarjetasCredito));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.ExtractosFundacion));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.ExtractosRotativo));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.ExtractosVivienda));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.Libranza));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.Fiducoomeva));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.ActivacionProtecciones));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.CartasCobranzaHabeasData));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.HabeasData));
            checkList.Add(ObtenerCantidadesXProducto(AppVariables.CheckListProceso.CantidadesProducto.CartasTAC));

            checkList.Add($"4. VALIDACION DE LAS CANTIDADES POR PROCESO;A. Se deben validar las cantidades para identificar inconsistencias  1. Cantidad de Tarjetas Visa deben Tener, Estados de Cuenta");
            checkList.Add($"5. VALIDACION DE LOS DATOS EN IMPRESIÓN;A. Fecha de Corte corresponde al ciclo de facturacion. ");
            checkList.Add($"6. ENTREGA DE MUESTRAS;a.) Realizar entrega de muestras digitales (PDF) a blancaf_astudillo@coomeva.com.co para su validación. b.) Enviar correo a blancaf_astudillo@coomeva.com.co , con copia a las personas involucradas en el proceso. ");
            checkList.Add($"7. APROBACION PROCESO DE IMPRESIÓN;a.) Para iniciar la  impresión  y el  ensobrado se debe recibir correo de aprobación de parte de blancaf_astudillo@coomeva.com.co para poder continuar con el proceso,  ya validados los puntos anteriores.");
            checkList.Add($"8. OBSERVACIONES;Cualquier inconsistemcia y/o inquietud preguntar a Blanca Astudillo o Andrey Amado");

            Helpers.EscribirEnArchivo($@"{_rutaSalidaCheckList}", checkList); 
            #endregion
        }

        /// <summary>
        /// Metodo que retrna una linea con los datos de check list para cantidades por producto
        /// </summary>
        /// <param name="pProducto">Producto</param>
        /// <returns></returns>
        private string ObtenerCantidadesXProducto(Variables.CantidadProducto pProducto)
        {
            return $"{pProducto.Nombre};{pProducto.MesAnterior};{pProducto.MesActual};{pProducto.Diferencia}";
        }

        /// <summary>
        /// Metodo que registra las Cantidades en el reporte de cantidades
        /// </summary>
        /// <param name="registrosCantidades">Lista de registris</param>
        /// <param name="pRegional">Nombre Regional</param>
        /// <param name="pProducto">Producto</param>
        /// <returns></returns>
        private int AgregarProductoReporteCantidades(List<string> registrosCantidades, string pRegional, string pProducto)
        {
            #region AgregarProductoReporteCantidades
            int resultado = 0;

            int pos = registrosCantidades.Count() - 1;
            if (AppVariables.ReporteCantidades.ContainsKey(pRegional))
            {
                var productos = AppVariables.ReporteCantidades[pRegional];

                if (productos.ContainsKey(pProducto))
                {
                    resultado = productos[pProducto];
                    registrosCantidades[pos] += $";{productos[pProducto].ToString()}";

                }
                else
                {
                    resultado = 0;
                    registrosCantidades[pos] += $";0";
                }
            }
            else
            {
                resultado = 0;
                registrosCantidades[pos] += $",0";
            }

            return resultado; 
            #endregion
        }
        
        /// <summary>
        /// Metodo para liberar Memoria
        /// </summary>        
        public void Dispose()
        {
            #region Dispose
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
            #endregion
        }

        // Protected implementation of Dispose pattern.
        /// <summary>
        /// Metodo para liberar Memoria
        /// </summary>
        /// <param name="disposing">Bandera para limpiar variables</param>
        protected virtual void Dispose(bool disposing)
        {
            #region Dispose
            if (_disposed)
                return;

            if (disposing)
            {
                
            }

            // Free any unmanaged objects here.
            _disposed = true;
            #endregion
        }
    }
}
