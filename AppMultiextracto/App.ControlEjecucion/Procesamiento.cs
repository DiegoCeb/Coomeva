using App.ControlFtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLL_Utilidades;
using App.Controlnsumos;
using System.IO;
using App.Variables;
using App.ControlWebServiceZonificacion;
using DLL_GenradorDocOne;

namespace App.ControlEjecucion
{
    /// <summary>
    /// Clase de ejecucion del proceso
    /// </summary>
    public class Procesamiento : Variables.Variables, IProcess, IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        Dictionary<string, string> DatosVerificacionArchivos;
        private Dictionary<string, string> datosBaseMaestra = new Dictionary<string, string>();
        private bool _encabezado = false;
        private string rutaBaseMaestra = string.Empty;

        /// <summary>
        /// Metodo para descaragra Archivos del FTP de Coomeva
        /// </summary>
        /// <returns>Bandera para ver estado proceso True = Correct / False = Error</returns>
        public bool DescargaArchivos()
        {
            #region DescargaArchivos
            try
            {
                ControlFTP objFtp = new ControlFTP(Utilidades.LeerAppConfig("FtpDireccionCoomeva"), Utilidades.LeerAppConfig("FtpUsuarioCoomeva"), Utilidades.LeerAppConfig("FtpClaveCoomeva"));
                objFtp.Conectar();
                Console.WriteLine("Conecto FTP");
                objFtp.DescargarArchivosFtp(Utilidades.LeerAppConfig("RutaEntrada"));
                objFtp.Desconectar();
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
            #endregion
        }

        /// <summary>
        /// Metodo para desencriptar Archivos
        /// </summary>
        public void DesencriptarArchivos()
        {
            #region DesencriptarArchivos

            DatosVerificacionArchivos = Insumos.CargarNombresArchivos();
            string insumo = string.Empty;

            foreach (var archivo in Directory.GetFiles(Utilidades.LeerAppConfig("RutaEntrada"), "*.gpg"))
            {
                insumo = IdentificarArchivo(Path.GetFileName(archivo));
                if(!string.IsNullOrEmpty(insumo))
                {
                    GetTamañoArchivo(insumo, archivo);
                }               

                Helpers.DesencriptarArchivos(archivo, Utilidades.LeerAppConfig("LLaveDesencripcion"), Utilidades.LeerAppConfig("RutaGnuPg"), Utilidades.LeerAppConfig("ClaveDesencriptado"));

            }

            Helpers.CortarMoverArchivosExtension(Utilidades.LeerAppConfig("RutaEntrada"), "*.gpg", Helpers.RutaOriginales);

            foreach (var archivo in Directory.GetFiles(Utilidades.LeerAppConfig("RutaEntrada"), "*.pgp"))
            {
                insumo = IdentificarArchivo(Path.GetFileName(archivo));
                if (!string.IsNullOrEmpty(insumo))
                {
                    GetTamañoArchivo(insumo, archivo);
                }

                Helpers.DesencriptarArchivos(archivo, Utilidades.LeerAppConfig("LLaveDesencripcion"), Utilidades.LeerAppConfig("RutaGnuPg"), Utilidades.LeerAppConfig("ClaveDesencriptado"));
            }

            Helpers.CortarMoverArchivosExtension(Utilidades.LeerAppConfig("RutaEntrada"), "*.pgp", Helpers.RutaOriginales);

            #endregion
        }

        /// <summary>
        /// Método para identificar el archivo a procesar
        /// </summary>
        /// <param name="pNombreArchivo">Nombre Archivo</param>
        /// <returns>Insumo correspondiente para llenar el CheckList</returns>
        private string IdentificarArchivo(string pNombreArchivo)
        {
            #region IdentificarArchivo
            foreach (var insumo in DatosVerificacionArchivos.Keys)
            {
                if (pNombreArchivo.Contains(insumo))
                {
                    if (pNombreArchivo.Contains("EXTV"))
                    {
                        if (pNombreArchivo.Substring(0, 4) == "EXTV")
                        {
                            return insumo;
                        }
                        else
                        {
                            return "PAPEXTVIVV";
                        }
                    }
                    else if (pNombreArchivo.Contains("RXX"))
                    {
                        if (pNombreArchivo.Substring(7, 1) == "E")
                        {
                            return "RXXE";
                        }
                        else if (pNombreArchivo.Substring(7, 1) == "I")
                        {
                            return "RXXI";
                        }
                        else
                        {
                            return insumo;
                        }

                    }
                    else
                    {
                        return insumo;
                    }
                }
            }

            return null; 
            #endregion
        }

        /// <summary>
        /// Metodo para verificar archivos de entrada
        /// </summary>
        /// <returns>Bandera para ver estado proceso True = Correct / False = Error</returns>
        public bool VerificacionArchivosEntrada()
        {
            #region VerificacionArchivosEntrada
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
            #endregion
        }

        /// <summary>
        /// Metodo que Obtiene el tamaño del Archivo
        /// </summary>
        /// <param name="pInsumo">Nombre del Insumo</param>
        /// <param name="pArchivo">Ruta del Archivo</param>
        private void GetTamañoArchivo(string pInsumo, string pArchivo)
        {
            #region GetTamañoArchivo
            Int64 tamañoArchivo = Helpers.GetTamañoArchivo(pArchivo);

            if (CheckListProceso.DiccionarioCantidadesArchivos.ContainsKey(pInsumo))
            {
                CantidadesArchivos cantidadesArchivos = CheckListProceso.DiccionarioCantidadesArchivos[pInsumo];
                cantidadesArchivos.NombreArchivo = Path.GetFileName(pArchivo);
                cantidadesArchivos.PesoArchivoMesActual = tamañoArchivo;
            }
            #endregion
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
                RutaBaseDelta = Helpers.RutaBaseMaestraFisico;
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

            if (!_encabezado)
            {
                //Encabezado
                datosBaseMaestra.Add("encabezado", $"cedula;cuenta;nombres;direccion;ciudad;dpto");
                rutaBaseMaestra = $"{Helpers.RutaProceso}\\BaseMaestra{DateTime.Now:yyyyMMddhhmm}.csv";
                _encabezado = true;
            }

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

                if (!datosBaseMaestra.ContainsKey(cedula))
                {
                    datosBaseMaestra.Add(cedula, $"{cedula};{cuenta};{nombres};{direccion};{ciudad};{dpto}");
                }
            }

            Helpers.EscribirEnArchivo(rutaBaseMaestra, datosBaseMaestra.Values.ToList());

            datosBaseMaestra.Clear();

            return rutaBaseMaestra;
            #endregion
        }

        /// <summary>
        /// Metodo que Actuliza los insumo enviados en el proceso
        /// </summary>
        /// <param name="nombreInusmo">Nombre Inusmo</param>
        /// <param name="archivoEntrada">Ruta de Insumo nuevo</param>
        public void ActulizaInsumos(string nombreInusmo, string archivoEntrada)
        {
            #region ActulizaInsumos
            //Buscar Archivo en Insumos
            string rutaInsumoActual = BuscarInsumoActual(nombreInusmo);

            //Movemos ArchivoActual a Historico
            if (!string.IsNullOrEmpty(rutaInsumoActual))
            {
                string nuevaRutaInsumo = string.Format(@"{0}\Historico", Utilidades.LeerAppConfig("RutaInsumos")); //NL
                string nombreInsumo = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(rutaInsumoActual), DateTime.Now.ToString("yyyyMMdd_mmss"), Path.GetExtension(rutaInsumoActual)); //NL
                Helpers.MoverArchivoaCarpeta(rutaInsumoActual, nuevaRutaInsumo, nombreInsumo);
            }

            //Movemos Nuevo Archivo a Insumos
            Helpers.MoverArchivoaCarpeta(archivoEntrada, Utilidades.LeerAppConfig("RutaInsumos"), Path.GetFileName(archivoEntrada)); //NL

            #endregion
        }

        /// <summary>
        /// Metodo que busca si el insumo existe en la carpeta de inusmos
        /// </summary>
        /// <param name="nombreInusmo">NOmbre del Insumo</param>
        /// <returns>Ruta del Inusmo encontrado</returns>
        private string BuscarInsumoActual(string nombreInusmo)
        {
            #region BuscarInsumoActual
            foreach (var archivoInsumos in Directory.GetFiles(Utilidades.LeerAppConfig("RutaInsumos")))
            {
                var nombreInsumos = Path.GetFileNameWithoutExtension(archivoInsumos);

                if (nombreInsumos.ToUpper().Contains(nombreInusmo))
                {
                    return archivoInsumos;
                }
            }

            return string.Empty;
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
            #region IniciarZonificacion
            Console.WriteLine("Entro IniciarZonificacion");
            try
            {
                // Ftp Delta
                ClaseFtp claseFTP = new ClaseFtp(Utilidades.LeerAppConfig("FtpDireccionDelta"),
                                                 Convert.ToInt16(Utilidades.LeerAppConfig("FtpPuertoDelta")),
                                                 Utilidades.LeerAppConfig("FtpUsuarioDelta"),
                                                 Utilidades.LeerAppConfig("FtpClaveDelta"));

                claseFTP.ConectarFtp();

                switch (tipoProceso.ToLower())
                {
                    #region Virtual Comentada
                    //case "virtual":
                    //    {
                    //        #region ZonificacionMail

                    //        string nombreCarpeta = Utilidades.LeerAppConfig("RutaFtp") + "/" + nombreProceso + " - " + DateTime.Now.ToShortDateString().Replace("/", "") + "_" + DateTime.Now.Second;

                    //        if (claseFTP.CrearcarpetaFtp(nombreCarpeta))
                    //        {
                    //            //carpeta creada correctamente
                    //            if (claseFTP.CargarArchivoFtp(RutaBaseDelta, nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta)))
                    //            {
                    //                //se crea la orden de servicio
                    //                Orden = ControlZonificacion.CrearOrdenServicio(Utilidades.LeerAppConfig("CodigoCliente"), Utilidades.LeerAppConfig("CodigoProcesoMail"));

                    //                //se realiza zonificacion
                    //                string estado = ControlZonificacion.RealizarZonificacion(Orden,
                    //                                                                         nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta),
                    //                                                                         Utilidades.LeerAppConfig("ConfiguracionMapeoVirtual"),
                    //                                                                         Utilidades.LeerAppConfig("TipoCargueVirtual"),
                    //                                                                         Utilidades.LeerAppConfig("CodigoCliente"),
                    //                                                                         Utilidades.LeerAppConfig("CodigoCourier"),
                    //                                                                         Utilidades.LeerAppConfig("CodigoProcesoVirtual"),
                    //                                                                         Utilidades.LeerAppConfig("EmailCertificadoVirtual"),
                    //                                                                         Utilidades.LeerAppConfig("TipoArchivo"),
                    //                                                                         Utilidades.LeerAppConfig("ReordenamientoVirtual"),
                    //                                                                         Utilidades.LeerAppConfig("Publicacion"),
                    //                                                                         nombreProceso,
                    //                                                                         Utilidades.LeerAppConfig("Delimitador"),
                    //                                                                         Utilidades.LeerAppConfig("InicioExtractoSpool")
                    //                                                                         );

                    //                //verifica si ya termino el proceso
                    //                while (estado != "finalizado")
                    //                {
                    //                    estado = ControlZonificacion.ValidarOrden(Orden).ToLower();
                    //                }

                    //                string archivosMail = Utilidades.LeerAppConfig("RutaFtpMail") + "/" + Orden;

                    //                claseFTP.CrearcarpetaFtp(archivosMail);

                    //                foreach (var item in Directory.GetFiles(Path.GetDirectoryName(RutaBaseDelta) ?? throw new InvalidOperationException()))
                    //                {
                    //                    if (Path.GetExtension(item).ToLower() == ".pdf")
                    //                    {
                    //                        claseFTP.CargarArchivoFtp(item, archivosMail + "/" + Path.GetFileName(item));
                    //                    }
                    //                }

                    //                File.Create(Path.GetDirectoryName(Path.GetDirectoryName(RutaBaseDelta)) + "\\" + Orden + ".txt");

                    //                Utilidades.EscribirLog("Termina Zonificacion por DELTA", Utilidades.LeerAppConfig("RutaLog"));
                    //            }
                    //            else
                    //            {
                    //                Utilidades.EscribirLog("Error al momento de cargar la base DELTA", Utilidades.LeerAppConfig("RutaLog"));
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Utilidades.EscribirLog("Error al momento de crear la carpeta para la base DELTA", Utilidades.LeerAppConfig("RutaLog"));
                    //        }
                    //        #endregion

                    //        break;
                    //    } 
                    #endregion

                    case "fisico":
                        {
                            #region ZonificacionFisica
                            string nombreCarpeta = (Utilidades.LeerAppConfig("RutaFtp") + "/" + nombreProceso + " - " + DateTime.Now.ToShortDateString().Replace("/", "") + "_" + DateTime.Now.Second).Replace(" ", "");

                            if (claseFTP.CrearcarpetaFtp(nombreCarpeta))
                            {
                                //carpeta creada correctamente
                                if (claseFTP.CargarArchivoFtp(RutaBaseDelta, nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta)))
                                {
                                    //se crea la orden de servicio
                                    Console.WriteLine($"Numero Orden:{Orden}");
                                    Console.WriteLine(nombreCarpeta + "/" + Path.GetFileName(RutaBaseDelta));
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
                                        Utilidades.EscribirLog($"While{estado}", Utilidades.LeerAppConfig("RutaLog"));
                                        Console.WriteLine($"While{estado}");
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
                                Utilidades.EscribirLog($"Error al momento de crear la carpeta para la base DELTA {nombreCarpeta}", Utilidades.LeerAppConfig("RutaLog"));
                            }

                            #endregion
                            break;
                        }
                }

                claseFTP.DesconectarFtp();
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
            #endregion
        }

        /// <summary>
        /// Metodo que Carga los Datos del proceso anterior correspondiete al corte actual
        /// </summary>
        /// <param name="pNumeroOrdenProceso">Número de Orden</param>
        public void CargueDiccionarioCheckList(string pNumeroOrdenProceso)
        {
            #region CargueDiccionarioCheckList
            CheckListProceso.Corte = ValidarNumeroOrden(pNumeroOrdenProceso);
            List<string> camposUltimoCorte = CargarHistoricoCantidades(Utilidades.LeerAppConfig("RutaLogCantidades"), CheckListProceso.Corte);
            Insumos.CargarNombresArchivosChekList(camposUltimoCorte);
            Insumos.CargarCantidadesExtractos(camposUltimoCorte);
            #endregion
        }

        /// <summary>
        /// Metodo para validar numero de orden
        /// </summary>
        /// <param name="pNumeroOrdenProceso">Número de Orden</param>
        /// <returns>Retorna el corte</returns>
        public string ValidarNumeroOrden(string pNumeroOrdenProceso)
        {
            #region ValidarNumeroOrden

            try
            {
                string corte = string.Empty;
                if (pNumeroOrdenProceso.Length == 10)
                {
                    corte = ObtenerNombreCorte(pNumeroOrdenProceso);
                    //if (corte == "05" || corte == "10" || corte == "15" || corte == "20" || corte == "25" || corte == "30") //NL
                    //{
                    corte = $"C{corte}";
                    //}
                    //else
                    //{
                    //    throw new Exception(RXGeneral.ErrorNumCorte);                    
                    //}
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
        /// <returns>Lista del ultimo corte correspondiente.</returns>
        private List<string> CargarHistoricoCantidades(string pRutaHistorico, string pCorte)
        {
            #region CargarHistoricoCantidades
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
            #endregion
        }

        /// <summary>
        /// Metodo que controla la insercion de datos de Cantidades en el Log
        /// </summary>
        /// <param name="pRutaHistorico">Ruta Archivo</param>
        /// <param name="pEscribirTitulos">Bandera de Titulos</param>
        /// <param name="pLinea">Linea a registrar</param>
        private void InsertarDatosHistoCantidades(string pRutaHistorico, bool pEscribirTitulos, string pLinea)
        {
            #region InsertarDatosHistoCantidades
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
            #endregion
        }

        /// <summary>
        /// Metodo que escribe directamente en el archivo
        /// </summary>
        /// <param name="pStreamWriter">Objeto de escritura</param>
        /// <param name="pEscribirTitulos">Bandera de Titulos</param>
        /// <param name="pLinea">LInea a escribir</param>
        private void EscribirHistoricoCantidades(StreamWriter pStreamWriter, bool pEscribirTitulos, string pLinea)
        {
            #region EscribirHistoricoCantidades
            if (pEscribirTitulos)
            {
                pStreamWriter.WriteLine(RXGeneral.TitulosHistoricoCantidades);
            }

            if (!string.IsNullOrEmpty(pLinea))
            {
                pStreamWriter.WriteLine(pLinea);
            }
            #endregion
        }

        /// <summary>
        /// Metodo que obtiene el Corte a aprtir del Número de Orden
        /// </summary>
        /// <param name="pNumeroOrdenProceso">Número de Orden</param>
        /// <returns>Nombre del corte</returns>
        private string ObtenerNombreCorte(string pNumeroOrdenProceso)
        {
            #region ObtenerNombreCorte
            if (pNumeroOrdenProceso.Length > 4)
            {
                return $"{pNumeroOrdenProceso.Substring(pNumeroOrdenProceso.Length - 2)}";
            }
            else
            { return string.Empty; }
            #endregion
        }

        /// <summary>
        /// Metdodo que obtiene los datos y los ordena para registralos
        /// </summary>
        public void RegistrarDatosHistoCantidades()
        {
            #region RegistrarDatosHistoCantidades
            string nuevaLineaCantidades =
                $"{CheckListProceso.Corte}" +
                $"|{DateTime.Now.ToString("dd/MM/yyyy")}" +
            #region Tamaño Archivos
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["ACTIVACION-PROTECCIONES"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["HABEASDATA"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["ExtractoFundacion"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["F99TODOSXX"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["R99TODOSXX"].PesoArchivoMesActual}" +
                $"|{CheckListProceso.DiccionarioCantidadesArchivos["RXXI"].PesoArchivoMesActual}" +
                 $"|{CheckListProceso.DiccionarioCantidadesArchivos["RXXE"].PesoArchivoMesActual}" +
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
                $"|{CheckListProceso.CantidadesProducto.Extractos.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.EstadoCuenta.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.Despositos.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.TarjetasCredito.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.ExtractosFundacion.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.ExtractosRotativo.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.ExtractosVivienda.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.Libranza.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.Fiducoomeva.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.ActivacionProtecciones.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.CartasCobranzaHabeasData.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.HabeasData.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.CartasSOAT.MesActual}" +
                $"|{CheckListProceso.CantidadesProducto.CartasTAC.MesActual}";

            #endregion

            InsertarDatosHistoCantidades(Utilidades.LeerAppConfig("RutaLogCantidades"), false, nuevaLineaCantidades);
            #endregion
        }

        /// <summary>
        /// Metodo para el cargue del proceso digital en Delta
        /// </summary>
        /// <param name="nombreProceso">Nombre Proceso</param>
        /// <param name="codigoCliente">Codigo Cliente</param>
        /// <param name="codigoProceso">Codigo Proceso</param>
        /// <param name="codigoCourier">Codigo Courrier</param>
        /// <param name="parametros">Parametros</param>
        /// <param name="pdfCliente">pdfClient</param>
        /// <param name="pAdjuntosEnLinea">Base del proceso</param>
        /// <param name="clienteDoc1">Cliente Doc1</param>
        /// <param name="productoDoc1">Producto Doc1</param>
        /// <param name="tipoSalidaDoc1">Tipo salida Doc1</param>
        /// <param name="pRutaArchivoVault">Ruta Archivos vault</param>
        public void CargueProcesoDigital(string nombreProceso, string codigoCliente, string codigoProceso, string codigoCourier, string parametros, bool pdfCliente, Dictionary<string, string> pAdjuntosEnLinea, string clienteDoc1, string productoDoc1, string tipoSalidaDoc1, string pRutaArchivoVault)
        {
            #region CargueProcesoDigital
            Console.WriteLine("mire el archivo .sal antes de que se genere");

            Console.WriteLine($"{clienteDoc1}|{productoDoc1}|{tipoSalidaDoc1}|{pRutaArchivoVault}");
            Console.ReadKey();
            GenerarSalidasDoc1(clienteDoc1, productoDoc1, tipoSalidaDoc1, pRutaArchivoVault);

            string nombreJrn = string.Empty;
            string archivoJrn = string.Empty;
            string nombrePs = string.Empty;

            nombreJrn = (from file in Directory.GetFiles(Path.GetDirectoryName(pRutaArchivoVault), "*.jrn")
                         where Path.GetExtension(file).ToLower() == ".jrn"
                         select Path.GetFileNameWithoutExtension(file)).FirstOrDefault();

            archivoJrn = (from file in Directory.GetFiles(Path.GetDirectoryName(pRutaArchivoVault), "*.jrn")
                          where Path.GetExtension(file).ToLower() == ".jrn"
                          select file).FirstOrDefault();

            nombrePs = (from file in Directory.GetFiles(Path.GetDirectoryName(pRutaArchivoVault), "*.ps")
                        where Path.GetExtension(file).ToLower() == ".ps"
                        select Path.GetFileNameWithoutExtension(file)).FirstOrDefault();

            Console.WriteLine($"cargue a vault");
            Console.ReadKey();

            Helpers.MoverArchivosExtension(Path.GetDirectoryName(pRutaArchivoVault), "*.jrn", Utilidades.LeerAppConfig("RutaVaultDownload"));
            Helpers.MoverArchivosExtension(Path.GetDirectoryName(pRutaArchivoVault), "*.ps", Utilidades.LeerAppConfig("RutaVaultDownload"));
            Helpers.MoverArchivosCondicionados(Utilidades.LeerAppConfig("RutaVaultDownload"), "*.jrn", Utilidades.LeerAppConfig("RutaVaultFinal"), nombreJrn, nombrePs);
            Helpers.MoverArchivosCondicionados(Utilidades.LeerAppConfig("RutaVaultDownload"), "*.ps", Utilidades.LeerAppConfig("RutaVaultFinal"), nombrePs, nombrePs);

            Console.WriteLine($"termino cargue a vault");
            Console.WriteLine($"Inicia cargue JRN a delta");

            IniciarSalidasZonificadas(nombreProceso, archivoJrn, codigoCliente, codigoProceso, codigoCourier, parametros, pdfCliente, pAdjuntosEnLinea);
            #endregion
        }

        /// <summary>
        /// Metodo para iniciar Salidas Zonificadas
        /// </summary>
        /// <param name="nombreProceso">Nombre Proceso</param>
        /// <param name="archivoCargue">Ruta Archivo cargue</param>
        /// <param name="codigoCliente">Codigo Cliente</param>
        /// <param name="codigoProceso">Codigo Proceso</param>
        /// <param name="codigoCourier">Codigo Courrier</param>
        /// <param name="parametros">Parametros</param>
        /// <param name="pdfCliente">pdfCliente</param>
        /// <param name="pAdjuntosEnLinea">Base del proceso</param>
        public void IniciarSalidasZonificadas(string nombreProceso, string archivoCargue, string codigoCliente, string codigoProceso, string codigoCourier, string parametros, bool pdfCliente, Dictionary<string, string> pAdjuntosEnLinea)
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

                ClaseFtpDelta.ConectarFtp();

                //carpeta creada correctamente
                if (ClaseFtpDelta.CrearcarpetaFtpDelta(nombreCarpeta))
                {
                    //archivo cargado correctamente
                    if (ClaseFtpDelta.CargarArchivoFtpDelta(archivoCargue, nombreCarpeta + "/" + Path.GetFileName(archivoCargue)))
                    {
                        //se realiza zonificacion
                        string estado = Helpers.RealizarSalidasZonificadas(Orden, nombreProceso, codigoCourier, codigoCliente, codigoProceso, parametros, "2", nombreCarpeta + "/" + Path.GetFileName(archivoCargue));

                        //verifica si ya termino el proceso
                        while (estado != "finalizado")
                        {
                            estado = Helpers.ValidarOrden(Orden).ToLower();
                            Console.WriteLine(estado);
                        }

                        Helpers.EscribirVentanaLog("Se genera correctamente el proceso...");

                        if (pdfCliente)
                        {
                            #region Cargar Pdfs de cliente a adjuntos en linea
                            string archivosMail = Utilidades.LeerAppConfig("RutaFtpMail") + "/" + Orden + "_adicional";

                            ClaseFtpDelta.CrearcarpetaFtpDelta(archivosMail);

                            foreach (var item in pAdjuntosEnLinea.Values)
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

                ClaseFtpDelta.DesconectarFtp();
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

        /// <summary>
        /// Metodo para genrar salidad de Doc1
        /// </summary>
        /// <param name="clienteDoc1">Cliente Doc1</param>
        /// <param name="productoDoc1">Producto Doc1</param>
        /// <param name="tipoSalidaDoc1">Tipo Salida Doc1</param>
        /// <param name="pArchivoSal">Ruta Archivo Sal</param>
        public void GenerarSalidasDoc1(string clienteDoc1, string productoDoc1, string tipoSalidaDoc1, string pArchivoSal)
        {
            #region Generar Salida DOC1 (PDF - PS)

            try
            {
                string estado = GeneradorArchivos.Procesar_Sal(LLenarParametros(), pArchivoSal, clienteDoc1, productoDoc1, tipoSalidaDoc1, Path.GetDirectoryName(pArchivoSal));

                if (estado.Contains("1"))
                {
                    throw new Exception($"Error en la generacion de salida Merge {estado}");                    
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

        /// <summary>
        /// Metodo para llenar parametros desde el Config
        /// </summary>
        /// <returns></returns>
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

        // Protected implementation of Dispose pattern.
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
                // Free any other managed objects here.
                DatosVerificacionArchivos.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
            #endregion
        }
    }
}
