using App.ControlFtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using App.Controlnsumos;
using System.IO;
using App.Variables;
using App.ControlWebServiceZonificacion;
using DLL_GenradorDocOne;


namespace App.ControlEjecucion
{
    public class Procesamiento : Variables.Variables, IProcess, IDisposable 
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        Dictionary<string, string> DatosVerificacionArchivos;
       
        /// <summary>
        /// Metodo para descaragra Archivos del FTP de Coomeva
        /// </summary>
        /// <returns></returns>
        public bool DescargaArchivos()
        {
            try
            {
                using (ClaseFtp objFtp = new ClaseFtp(Utilidades.LeerAppConfig("FtpDireccionCoomeva"),
                    Convert.ToInt16(Utilidades.LeerAppConfig("FtpPuertoCoomeva")), Utilidades.LeerAppConfig("FtpUsuarioCoomeva"),
                    Utilidades.LeerAppConfig("FtpClaveCoomeva")))
                {
                    objFtp.ConectarFtp();

                    objFtp.DescargarArchivosFtp(".", Utilidades.LeerAppConfig("RutaEntrada"), ".gpg", ".pgp");

                    objFtp.DesconectarFtp();
                }

                return true;
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Procesamiento),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);                
                return false;
            }

        }

        /// <summary>
        /// Metodo para verificar archivos de entrada
        /// </summary>
        /// <returns></returns>
        public bool VerificacionArchivosEntrada()
        {
            try
            {
                string resultado = "1";

                DatosVerificacionArchivos = Insumos.CargarNombresArchivos();

                foreach (var archivo in Directory.GetFiles(Utilidades.LeerAppConfig("RutaEntrada")))
                {
                    foreach (var insumo in DatosVerificacionArchivos.Keys)
                    {
                        if (archivo.Contains(insumo))
                        {
                            resultado = "0";
                            GetTamañoArchivo(insumo, archivo);
                            break;
                        }
                    }

                    if (resultado == "1")
                    {
                       throw new Exception($"El siguiente archivo {Path.GetFileName(archivo)} no se reconoce dentro de los nombres configurados para el proceso.");
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Procesamiento),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
                return false;
            }
            
        }

        /// <summary>
        /// Metodo que Obtiene el tamaño del Archivo
        /// </summary>
        /// <param name="pInsumo">Nombre del Insumo</param>
        /// <param name="pArchivo">Ruta del Archivo</param>
        private void GetTamañoArchivo(string pInsumo, string pArchivo)
        {
            Int64 tamañoArchivo = Helpers.GetTamañoArchivo(pArchivo);

            if (CheckListProceso.DiccionarioCantidadesArchivos.ContainsKey(pInsumo))
            {
                CantidadesArchivos cantidadesArchivos = CheckListProceso.DiccionarioCantidadesArchivos[pInsumo];
                cantidadesArchivos.PesoArchivoMesActual = tamañoArchivo;
                cantidadesArchivos.DiferenciaPesoArchivo = cantidadesArchivos.PesoArchivoMesActual - cantidadesArchivos.PesoArchivoMesAnterior;
            }
        }


        /// <summary>
        /// Carga los archivos de manera global y temporal.
        /// </summary>
        /// <typeparam name="TEntidad">Entidad que se va a cargar</typeparam>
        /// <param name="pArchivo">ruta del archivo</param>
        /// <param name="pEntidadArchivo">Entidad del proceso</param>
        /// <returns>True o False segun proceso.</returns>
        public bool CargueArchivosGlobal<TEntidad>(string pArchivo, TEntidad pEntidadArchivo)
        {
            #region CargueArchivosGlobal
            var newObject = (Type)(object)pEntidadArchivo;

            if (newObject.Name == "Etiquetas" && Path.GetFileNameWithoutExtension(pArchivo).ToUpper().Contains("I"))
            {
                Helpers.RutaBaseMaestraFisico = GenerarBaseMaestra(pArchivo);
            }

            object invoke = newObject.InvokeMember(newObject.Name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance, null,
                newObject, new object[] { pArchivo });

            return true;
            #endregion
        }

        /// <summary>
        /// Metodo que genera la base maestra para zonificacion
        /// </summary>
        /// <param name="pArchivo">ruta del archivo de datos donde sale la informacion principal</param>
        /// <returns>cadena con la ruta del servidor donde se creo el archivo</returns>
        public string GenerarBaseMaestra(string pArchivo)
        {
            #region GenerarBaseMaestra
            string rutaResult = $"{Helpers.RutaProceso}\\BaseMaestra.csv";
            List<string> datosBaseMaestra = new List<string>();

            //Encabezado
            datosBaseMaestra.Add($"cedula;cuenta;nombres;direccion;ciudad;dpto");

            foreach (var linea in Utilidades.LeerArchivoPlanoANSI(pArchivo))
            {
                string cedula = $"{linea.Substring(143, 6)}{linea.Substring(151, 5)}".TrimStart('0');
                string cuenta = $"{linea.Substring(143, 6)}{linea.Substring(151, 5)}".TrimStart('0');
                string nombres = linea.Substring(0, 31).Trim();
                string direccion = linea.Substring(31, 59).Trim();
                string segmentoCiudadDpto = linea.Substring(92, 30).Trim();
                string ciudad = segmentoCiudadDpto.LastIndexOf('(') != -1 ? segmentoCiudadDpto.Substring(0, segmentoCiudadDpto.LastIndexOf('(')).Trim() : segmentoCiudadDpto;
                string dpto = segmentoCiudadDpto.LastIndexOf('(') != -1 ? segmentoCiudadDpto.Substring(segmentoCiudadDpto.LastIndexOf('(')).Trim().Replace("(", "").Replace(")", "") : segmentoCiudadDpto;

                if (ciudad.ToUpper() == "BOGOTA D.C.")
                {
                    ciudad = "BOGOTA D.C.";
                    dpto = "BOGOTA D.C.";
                }

                datosBaseMaestra.Add($"{cedula};{cuenta};{nombres};{direccion};{ciudad};{dpto}");
            }

            File.WriteAllLines(rutaResult, datosBaseMaestra);

            datosBaseMaestra.Clear();

            return rutaResult;
            #endregion
        }

        /// <summary>
        /// Metodo para Iniciar La Zonificacion
        /// </summary>
        /// <param name="tipoProceso">Enviar literal de "Fisico" o  "Virtual"</param>
        /// <param name="nombreProceso">nombre personalizado para el proceso.</param>
        /// <returns>True o False dependiendo el resultado.</returns>
        public bool IniciarZonificacion(string tipoProceso, string nombreProceso)
        {
            try
            {
                // Ftp Delta
                ClaseFtp claseFTP = new ClaseFtp(Utilidades.LeerAppConfig("FtpDireccionDelta"),
                                                 Convert.ToInt16(Utilidades.LeerAppConfig("FtpPuertoDelta")),
                                                 Utilidades.LeerAppConfig("FtpUsuarioDelta"),
                                                 Utilidades.LeerAppConfig("FtpClaveDelta"));

                switch (tipoProceso.ToLower())
                {
                    case "virtual":
                        {
                            #region ZonificacionMail

                            string nombreCarpeta = Utilidades.LeerAppConfig("RutaFtp") + "/" + nombreProceso + " - " + DateTime.Now.ToShortDateString().Replace("/", "") + "_" + DateTime.Now.Second;
                            
                            if (claseFTP.CrearcarpetaFtp(nombreCarpeta))
                            {
                                //carpeta creada correctamente
                                if (claseFTP.CargarArchivoFtp(RutaBaseDelta, nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta)))
                                {
                                    //se crea la orden de servicio
                                    Orden = ControlZonificacion.CrearOrdenServicio(Utilidades.LeerAppConfig("CodigoCliente"), Utilidades.LeerAppConfig("CodigoProcesoMail"));

                                    //se realiza zonificacion
                                    string estado = ControlZonificacion.RealizarZonificacion(Orden, 
                                                                                             nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta),
                                                                                             Utilidades.LeerAppConfig("ConfiguracionMapeoVirtual"),
                                                                                             Utilidades.LeerAppConfig("TipoCargueVirtual"), 
                                                                                             Utilidades.LeerAppConfig("CodigoCliente"),
                                                                                             Utilidades.LeerAppConfig("CodigoCourier"),
                                                                                             Utilidades.LeerAppConfig("CodigoProcesoVirtual"),
                                                                                             Utilidades.LeerAppConfig("EmailCertificadoVirtual"),
                                                                                             Utilidades.LeerAppConfig("TipoArchivo"),
                                                                                             Utilidades.LeerAppConfig("ReordenamientoVirtual"),
                                                                                             Utilidades.LeerAppConfig("Publicacion"),
                                                                                             nombreProceso,
                                                                                             Utilidades.LeerAppConfig("Delimitador"),
                                                                                             Utilidades.LeerAppConfig("InicioExtractoSpool")
                                                                                             );

                                    //verifica si ya termino el proceso
                                    while (estado != "finalizado")
                                    {
                                        estado = ControlZonificacion.ValidarOrden(Orden).ToLower();
                                    }

                                    string archivosMail = Utilidades.LeerAppConfig("RutaFtpMail") + "/" + Orden;

                                    claseFTP.CrearcarpetaFtp(archivosMail);

                                    foreach (var item in Directory.GetFiles(Path.GetDirectoryName(RutaBaseDelta) ?? throw new InvalidOperationException()))
                                    {
                                        if (Path.GetExtension(item).ToLower() == ".pdf")
                                        {
                                            claseFTP.CargarArchivoFtp(item, archivosMail + "/" + Path.GetFileName(item));
                                        }
                                    }

                                    File.Create(Path.GetDirectoryName(Path.GetDirectoryName(RutaBaseDelta)) + "\\" + Orden + ".txt");

                                    Utilidades.EscribirLog("Termina Zonificacion por DELTA", Utilidades.LeerAppConfig("RutaLog"));
                                }
                                else
                                {
                                    Utilidades.EscribirLog("Error al momento de cargar la base DELTA", Utilidades.LeerAppConfig("RutaLog"));
                                }
                            }
                            else
                            {
                                Utilidades.EscribirLog("Error al momento de crear la carpeta para la base DELTA", Utilidades.LeerAppConfig("RutaLog"));
                            }
                            #endregion

                            break;
                        }
                    case "fisico":
                        {
                            #region ZonificacionFisica
                            string nombreCarpeta = Utilidades.LeerAppConfig("RutaFtp") + "/Proceso " + tipoProceso + " - " + DateTime.Now.ToShortDateString().Replace("/", "") + "_" + DateTime.Now.Second;

                            if (claseFTP.CrearcarpetaFtp(nombreCarpeta))
                            {
                                //carpeta creada correctamente
                                if (claseFTP.CargarArchivoFtp(RutaBaseDelta, nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta)))
                                {
                                    //se crea la orden de servicio
                                    Orden = ControlZonificacion.CrearOrdenServicio(Utilidades.LeerAppConfig("CodigoCliente"), Utilidades.LeerAppConfig("CodigoProceso"));
                                    
                                    //se realiza zonificacion
                                    string estado = ControlZonificacion.RealizarZonificacion(Orden, 
                                                                                             nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta),
                                                                                             Utilidades.LeerAppConfig("ConfiguracionMapeoFisica"),
                                                                                             Utilidades.LeerAppConfig("TipoCargueFisico"),
                                                                                             Utilidades.LeerAppConfig("CodigoCliente"),
                                                                                             Utilidades.LeerAppConfig("CodigoCourier"),
                                                                                             Utilidades.LeerAppConfig("CodigoProcesoFisico"),
                                                                                             Utilidades.LeerAppConfig("EmailCertificadoFisico"),
                                                                                             Utilidades.LeerAppConfig("TipoArchivo"),
                                                                                             Utilidades.LeerAppConfig("ReordenamientoFisico"),                                                                                             
                                                                                             Utilidades.LeerAppConfig("Publicacion"),
                                                                                             nombreProceso,
                                                                                             Utilidades.LeerAppConfig("Delimitador"),
                                                                                             Utilidades.LeerAppConfig("InicioExtractoSpool")
                                                                                             );

                                    //verifica si ya termino el procesos
                                    while (estado != "finalizado")
                                    {
                                        estado = ControlZonificacion.ValidarOrden(Orden).ToLower();
                                    }

                                    //descarga la orden                                    
                                    RutaBaseDelta = Helpers.CrearCarpeta(Path.GetDirectoryName(RutaBaseDelta) + "\\" + Orden);
                                    claseFTP.DescargarArchivosFtpOrden(Utilidades.LeerAppConfig("RutaFtpSalidas") + Orden, RutaBaseDelta);
                                    Utilidades.EscribirLog("Termina Zonificacion por DELTA", Utilidades.LeerAppConfig("RutaLog"));
                                }
                                else
                                {
                                    Utilidades.EscribirLog("Error al momento de cargar la base DELTA", Utilidades.LeerAppConfig("RutaLog"));
                                }
                            }
                            else
                            {                                
                                Utilidades.EscribirLog("Error al momento de crear la carpeta para la base DELTA", Utilidades.LeerAppConfig("RutaLog"));
                            }

                            #endregion

                            break;
                        }
                }
                return true;
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Procesamiento),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
                return false;
            }            
        }

        /// <summary>
        /// Metodo que Carga los Datos del proceso anterior correspondiete al corte actual
        /// </summary>
        /// <param name="pNumeroOrdenProceso">Número de Orden</param>
        public void CargueDiccionarioCheckList(string pNumeroOrdenProceso)
        {
            NombreCorte = ValidarNumeroOrden(pNumeroOrdenProceso);
            List<string> camposUltimoCorte = CargarHistoricoCantidades(Utilidades.LeerAppConfig("RutaLogCantidades"), NombreCorte);
            Insumos.CargarNombresArchivosChekList(camposUltimoCorte);
            Insumos.CargarCantidadesExtractos(camposUltimoCorte);
        }

        public string ValidarNumeroOrden(string pNumeroOrdenProceso)
        {
            #region ValidarNumeroOrden
            try
            {
                string corte = string.Empty;

                if (pNumeroOrdenProceso.Length == 10)
                {
                    corte = ObtenerNombreCorte(pNumeroOrdenProceso);

                    if (corte == "05" || corte == "10" || corte == "15" || corte == "20" || corte == "25" || corte == "30") //NL
                    {
                        corte = $"C{corte}";
                    }
                    else
                    {
                        throw new Exception(RXGeneral.ErrorNumCorte);
                    }

                }
                else
                {
                    throw new Exception(RXGeneral.ErrorTamañoNumOrden);
                }

                return corte;
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Procesamiento),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
                return String.Empty;
            }

            
            #endregion
        }

        /// <summary>
        /// Busca el ultimo regitro del corte correspondiente y Segmeta la linea de cantiadades para obtner los Valores
        /// </summary>
        /// <param name="pRutaHistorico">Ruta RAchivo</param>
        /// <param name="pCorte">Corte</param>
        /// <returns></returns>
        private List<string> CargarHistoricoCantidades(string pRutaHistorico, string pCorte)
        {
            List<string> camposUltimoCorte = new List<string>();

            if (File.Exists(pRutaHistorico))
            {
                StreamReader lector = new StreamReader(pRutaHistorico, Encoding.Default);

                string linea = string.Empty;
                while ((linea = lector.ReadLine()) != null)
                {
                    string[] campos = linea.Split('|');

                    if (campos[0] == pCorte)
                    {
                        camposUltimoCorte = campos.ToList();
                    }
                }

                lector.Close();
            }
            else
            {
                InsertarDatosHistoCantidades(pRutaHistorico, true, null);
            }

            
            return camposUltimoCorte;
        }

        /// <summary>
        /// Metodo que controla la insercion de datos de Cantidades en el Log
        /// </summary>
        /// <param name="pRutaHistorico">Ruta Archivo</param>
        /// <param name="pEscribirTitulos">Bandera de Titulos</param>
        /// <param name="pLinea">Linea a registrar</param>
        private void InsertarDatosHistoCantidades(string pRutaHistorico, bool pEscribirTitulos, string pLinea)
        {
            if (File.Exists(pRutaHistorico))
            {
                using (StreamWriter streamWriter = new StreamWriter(pRutaHistorico, true, Encoding.Default))
                {

                    EscribirHistoricoCantidades(streamWriter, pEscribirTitulos, pLinea);
                }
            }
            else
            {
                FileStream escritor = File.Create(pRutaHistorico);

                using (StreamWriter streamWriter = new StreamWriter(escritor, Encoding.Default))
                {
                    EscribirHistoricoCantidades(streamWriter, true, pLinea);
                }

                escritor.Close();
            }
        }

        /// <summary>
        /// Metodo que escribe directamente en el archivo
        /// </summary>
        /// <param name="pStreamWriter">Objeto de escritura</param>
        /// <param name="pEscribirTitulos">Bandera de Titulos</param>
        /// <param name="pLinea">LInea a escribir</param>
        private void EscribirHistoricoCantidades(StreamWriter pStreamWriter, bool pEscribirTitulos, string pLinea)
        {
            if (pEscribirTitulos)
            {
                pStreamWriter.WriteLine(RXGeneral.TitulosHistoricoCantidades);
            }

            if (!string.IsNullOrEmpty(pLinea))
            {
                pStreamWriter.WriteLine(pLinea);
            }
        }

        /// <summary>
        /// Metodo que obtiene el COrte a aprtir del Número de Orden
        /// </summary>
        /// <param name="pNumeroOrdenProceso">Número de Orden</param>
        /// <returns></returns>
        private string ObtenerNombreCorte(string pNumeroOrdenProceso)
        {
            if (pNumeroOrdenProceso.Length > 4)
            {
                return $"{pNumeroOrdenProceso.Substring(pNumeroOrdenProceso.Length - 2)}";
            }
            else
            { return string.Empty; }


        }

        /// <summary>
        /// Metdodo que obtiene los datos y los ordena para registralos
        /// </summary>
        public void RegistrarDatosHistoCantidades()
        {
            string nuevaLineaCantidades =
                $"{NombreCorte}" +
                $"|{DateTime.Now.ToString("dd/MM/yyyy")}" +
            #region Tamaño Archivos
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["ACTIVACION-PROTECCIONES"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["HABEASDATA"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["ExtractoFundacion"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["F99TODOSXX"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["R99TODOSXX"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["RXX"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["SMS"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["SOAT"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["E0"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["Asociados_Inactivos"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["Extracto_rotativo"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["EXTV"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["Fiducoomeva"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["PAPEXTVIVV"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["Pinos"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["BaseEstadoCuentaAsociados"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["BaseEstadoCuentaTerceros"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["muestras"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["PlanoBeneficiosEstadoCuenta"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["PAPEXTSUBV"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["Nuevos_Asociados_Fisicos"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["BASE_ACTIVOS_TAC"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["BASE_INACTIVOS_TAC"].PesoArchivoMesActual}" +
            #endregion
            #region Cantidades Extractos
                $"|{CheckListProceso.CantidadesExtractosNacional.Extractos.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.HojasEstadoCuentaSimplex.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.HojasEstadoCuentaDuplex.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.HojasViviendaSimplex.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.HojasViviendaDuplex.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.HojasDespositosSimplex.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.HojasDespositosDuplex.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.ExtractosVisa.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.ExtractosMaster.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.CartasSOAT.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.CartasAsocHabeasData.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.CartasCobrosHabeasData.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.ActivacionProtecciones.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.ExtractosPlanPagosLibranza.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.ExtractosCreditoRotativo.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.ExtractosMicroCredito.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.Fiducoomeva.MesActual}" +
                $"|{CheckListProceso.CantidadesExtractosNacional.CartasTAC.MesActual}";

            #endregion

            InsertarDatosHistoCantidades(Utilidades.LeerAppConfig("RutaLogCantidades"), false, nuevaLineaCantidades);
        }

        public void CargueProcesoDigital(string procesos, string codigoCliente, string codigoProceso, string codigoCourier, string parametros, bool pdfCliente, string basedelProceso, string clienteDoc1, string productoDoc1, string tipoSalidaDoc1)
        {
            GenerarSalidasDoc1(clienteDoc1, productoDoc1, tipoSalidaDoc1);

            string nombreJrn = string.Empty;
            string archivoJrn = string.Empty;
            string nombrePs = string.Empty;

            nombreJrn = (from file in Directory.GetFiles(Path.GetDirectoryName(RutaBaseDelta), "*.jrn")
                         where Path.GetExtension(file).ToLower() == ".jrn"
                         select Path.GetFileNameWithoutExtension(file)).FirstOrDefault();

            archivoJrn = (from file in Directory.GetFiles(Path.GetDirectoryName(RutaBaseDelta), "*.jrn")
                          where Path.GetExtension(file).ToLower() == ".jrn"
                          select file).FirstOrDefault();

            nombrePs = (from file in Directory.GetFiles(Path.GetDirectoryName(RutaBaseDelta), "*.ps")
                        where Path.GetExtension(file).ToLower() == ".ps"
                        select Path.GetFileNameWithoutExtension(file)).FirstOrDefault();

            Helpers.MoverArchivosExtension(Path.GetDirectoryName(RutaBaseDelta), "*.jrn", Utilidades.LeerAppConfig("RutaVaultDownload"));
            Helpers.MoverArchivosExtension(Path.GetDirectoryName(RutaBaseDelta), "*.ps", Utilidades.LeerAppConfig("RutaVaultDownload"));
            Helpers.MoverArchivosCondicionados(Utilidades.LeerAppConfig("RutaVaultDownload"), "*.jrn", Utilidades.LeerAppConfig("RutaVaultFinal"), nombreJrn);
            Helpers.MoverArchivosCondicionados(Utilidades.LeerAppConfig("RutaVaultDownload"), "*.ps", Utilidades.LeerAppConfig("RutaVaultFinal"), nombrePs);

            IniciarSalidasZonificadas(procesos, archivoJrn, codigoCliente, codigoProceso, codigoCourier, parametros, pdfCliente, basedelProceso);
        }

        public void IniciarSalidasZonificadas(string nombreProceso, string archivoCargue, string codigoCliente, string codigoProceso, string codigoCourier, string parametros, bool pdfCliente, string basedelProceso)
        {
            #region Iniciar Salidas Zonificadas
            try
            {
                string nombreCarpeta = Utilidades.LeerAppConfig("RutaFtp") + "/" + nombreProceso + " - " + DateTime.Now.ToShortDateString().Replace("/", "") + "_" + DateTime.Now.Second;

                // Ftp Delta
                ClaseFtp ClaseFtpDelta = new ClaseFtp(Utilidades.LeerAppConfig("FtpDireccionDelta"),
                                                 Convert.ToInt16(Utilidades.LeerAppConfig("FtpPuertoDelta")),
                                                 Utilidades.LeerAppConfig("FtpUsuarioDelta"),
                                                 Utilidades.LeerAppConfig("FtpClaveDelta"));

                if (ClaseFtpDelta.CrearcarpetaFtpDelta(nombreCarpeta))
                {
                    //carpeta creada correctamente
                    if (ClaseFtpDelta.CargarArchivoFtpDelta(archivoCargue, nombreCarpeta + "/" + Path.GetFileName(archivoCargue)))
                    {
                        //se crea la orden de servicio
                        Orden = Helpers.CrearOrdenServicio(codigoCliente, codigoProceso);

                        //se realiza zonificacion
                        string estado = Helpers.RealizarSalidasZonificadas(Orden, nombreProceso, codigoCourier, codigoCliente, codigoProceso, parametros, "2", nombreCarpeta + "/" + Path.GetFileName(archivoCargue));

                        //verifica si ya termino el proceso
                        while (estado != "finalizado")
                        {
                            estado = Helpers.ValidarOrden(Orden).ToLower();
                        }

                        Helpers.EscribirVentanaLog("Se genera correctamente el proceso...");

                        if (Convert.ToBoolean(pdfCliente))
                        {
                            #region Cargar Pdfs de cliente a adjuntos en linea
                            string archivosMail = Utilidades.LeerAppConfig("RutaFtpMail") + "/" + Orden + "_adicional";

                            ClaseFtpDelta.CrearcarpetaFtpDelta(archivosMail);

                            foreach (var item in Directory.GetFiles(Path.GetDirectoryName(basedelProceso) ?? throw new InvalidOperationException()))
                            {
                                if (Path.GetExtension(item).ToLower() == ".pdf")
                                {
                                    ClaseFtpDelta.CargarArchivoFtpDelta(item, archivosMail + "/" + Path.GetFileName(item));
                                }
                            }

                            Helpers.EscribirVentanaLog("Termina la carga de los PDFs...");
                            #endregion
                        }

                        File.Create(Path.GetDirectoryName(Path.GetDirectoryName(archivoCargue)) + "\\" + Orden + ".txt");

                        Helpers.EscribirVentanaLog("Termina Zonificacion por DELTA");
                    }
                    else
                    {
                        Helpers.EscribirVentanaLog("Error al momento de cargar la base DELTA");
                    }
                }
                else
                {
                    Helpers.EscribirVentanaLog("Error al momento de crear la carpeta para la base DELTA");
                }
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Procesamiento),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, false);
                
            }

           
            #endregion
        }

        public void GenerarSalidasDoc1(string clienteDoc1, string productoDoc1, string tipoSalidaDoc1)
        {
            #region Generar Salida DOC1 (PDF - PS)
            try
            {
                string Busqueda = string.Empty;

                Busqueda = RutaBaseDelta;

                foreach (var archivo in Directory.GetFiles(Busqueda, "*.sal"))
                {
                    string nombre = Path.GetFileNameWithoutExtension(archivo);
                    if (nombre.Contains("guias"))
                    {
                        continue;

                    }
                    string estado = GeneradorArchivos.Procesar_Sal(LLenarParametros(), archivo, clienteDoc1, productoDoc1, tipoSalidaDoc1, Busqueda);

                    if (estado.Contains("1"))
                    {
                        throw new Exception("Error en la generacion de salida Merge.");
                    }
                }
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Procesamiento),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
            }

            
            #endregion
        }

        public static string[] LLenarParametros()
        {
            #region LLenar Parametros

            string[] Resultado = new string[15];

            Resultado[0] = @Utilidades.LeerAppConfig("RutaHips");
            Resultado[1] = @Utilidades.LeerAppConfig("RutaDoc1Log");
            Resultado[2] = @Utilidades.LeerAppConfig("Doc1_5.5");
            Resultado[3] = @Utilidades.LeerAppConfig("Doc1_5.6");
            Resultado[4] = @Utilidades.LeerAppConfig("Doc1_6.0");
            Resultado[5] = @Utilidades.LeerAppConfig("RutaTemp");
            Resultado[6] = @Utilidades.LeerAppConfig("FileConfProductos");
            Resultado[7] = @Utilidades.LeerAppConfig("RutaImages");
            Resultado[8] = @Utilidades.LeerAppConfig("RutaImagesPDF");
            Resultado[9] = @Utilidades.LeerAppConfig("RutaImagesAFP");
            Resultado[10] = @Utilidades.LeerAppConfig("RutaImagesPS");
            Resultado[11] = @Utilidades.LeerAppConfig("RutaLookUpTable");
            Resultado[12] = @Utilidades.LeerAppConfig("RutaDoc1Log");
            Resultado[13] = @Utilidades.LeerAppConfig("Doc1_6.6");
            Resultado[14] = @Utilidades.LeerAppConfig("Doc1_6.6.11");

            return Resultado;
            #endregion
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                DatosVerificacionArchivos.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

        
    }
}
