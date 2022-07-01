using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using Excel;
using ICSharpCode.SharpZipLib;
using Starksoft.Aspen.GnuPG;
using DLL_Utilidades;
using DLL_ServicioDelta;
using DLL_ServicioDelta.wsDelta;
using SharpCompress.Archives;
using SharpCompress.Readers;
using SharpCompress.Common;

namespace App.Controlnsumos
{
    /// <summary>
    /// Clase Helpers
    /// </summary>
    public static class Helpers
    {
        public static string RutaProceso { get; set; }
        public static string RutaOriginales { get; set; }
        public static string RutaBaseMaestraFisico { get; set; }

        /// <summary>
        /// Metodo para convertir Excel (.xlsx - .xls) en archivo plano
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<string> ConvertirExcel(string archivo)
        {
            #region ConvertirExcel
            DataSet result = new DataSet();
            try
            {
                #region Leer Excel
                if (archivo.EndsWith(".xlsx"))
                {
                    //Reading from a binary Excel file(format; .xlsx)
                    FileStream stream = File.Open(archivo, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    result = excelReader.AsDataSet();
                    excelReader.Close();
                }

                if (archivo.EndsWith(".xls"))
                {
                    //Reading from a binary Excel file('97-2003 format; .xls)
                    FileStream stream = File.Open(archivo, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    result = excelReader.AsDataSet();
                    excelReader.Close();
                }

                #endregion

                #region Pasar Excel a Plano
                List<string> datosExcel = new List<string>();
                int numHoja = Convert.ToInt32(0);
                string a = "";

                for (int j = 0; j < result.Tables[numHoja].Rows.Count; j++)
                {
                    for (int i = 0; i < result.Tables[numHoja].Columns.Count; i++)
                    {
                        if (result.Tables[numHoja].Rows[j][i].ToString() != "")
                        {
                            a += result.Tables[numHoja].Rows[j][i].ToString().Replace("\n", " ");
                        }
                        else
                        {
                            a += " ";
                        }

                        if (i < (result.Tables[numHoja].Columns.Count + 1))
                        {
                            a += "|";
                        }
                    }

                    datosExcel.Add(a);
                    a = "";

                }
                #endregion

                return datosExcel;
            }
            catch (Exception mens)
            {
                throw new Exception("Error: " + mens.Message);
            }
            #endregion
        }

        /// <summary>
        /// Obtiene el tamaño del archivo
        /// </summary>
        /// <param name="pRutaArchivo">Ruta del Archivo</param>
        /// <returns>Tamaño</returns>
        public static Int64 GetTamañoArchivo(string pRutaArchivo)
        {
            #region GetTamañoArchivo
            Int64 tamañoArchivo = 0;

            FileInfo fileInfo = new FileInfo(pRutaArchivo);

            if (fileInfo.Exists)
            {
                tamañoArchivo = fileInfo.Length;
            }

            return tamañoArchivo;
            #endregion
        }

        /// <summary>
        /// Obtiene el Tamaño del ultimo corte por posicion
        /// </summary>
        /// <param name="tamañoUltimoCorte">Lista de campos UltimoCorte</param>
        /// <param name="pPosicion">Posicion</param>
        /// <returns>Tamaño</returns>
        public static Int64 GetTamañoHistorico(List<string> tamañoUltimoCorte, int pPosicion)
        {
            #region GetTamañoHistorico
            Int64 tamañoArchivo = 0;

            try
            {
                if (tamañoUltimoCorte.Count >= pPosicion)
                {
                    string pValor = tamañoUltimoCorte[pPosicion];

                    if (!string.IsNullOrEmpty(pValor))
                    {
                        tamañoArchivo = Convert.ToInt64(pValor);
                    }
                }

            }
            catch
            {
                tamañoArchivo = 0;
            }

            return tamañoArchivo;
            #endregion
        }

        /// <summary>
        /// Obtiene el Tamaño del ultimo corte por posicion
        /// </summary>
        /// <param name="tamañoUltimoCorte">Lista de campos UltimoCorte</param>
        /// <param name="pPosicion">Posicion</param>
        /// <returns>Tamaño</returns>
        public static Int32 GetTamañoHistoricoInt(List<string> tamañoUltimoCorte, int pPosicion)
        {
            #region GetTamañoHistoricoInt

            Int32 tamañoArchivo = 0;

            try
            {
                if (tamañoUltimoCorte.Count >= pPosicion)
                {
                    string pValor = tamañoUltimoCorte[pPosicion];

                    if (!string.IsNullOrEmpty(pValor))
                    {
                        tamañoArchivo = Convert.ToInt32(pValor);
                    }
                }

            }
            catch
            {
                tamañoArchivo = 0;
            }

            return tamañoArchivo;
            #endregion
        }

        /// <summary>
        /// Metodo para crear carpeta 
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns>Ruta de la carpeta creada</returns>
        /// <exception cref="Exception"></exception>
        public static string CrearCarpeta(string ruta)
        {
            #region CrearCarpeta
            try
            {
                string carpeta = ruta;

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                return carpeta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
            #endregion

        }

        /// <summary>
        /// Metodo que valida si hay Pipe seguidos o si se debe ageregar balnco al final de la cadena
        /// </summary>
        /// <param name="linea">String a verificar</param>
        /// <returns>linea con el blanco adicionado</returns>
        public static string ValidarPipePipe(string linea)
        {
            #region ValidarPipePipe

            linea = linea.Replace("||", "| |").Replace("||", "| |");

            if (linea != "")
            {
                if (linea.Last() == '|')
                {
                    return $"{linea} ";
                }
                else
                {
                    return linea;
                }
            }
            else
            {
                return linea;
            }

            #endregion
        }

        /// <summary>
        /// Metodo que remplaza un carcater en la linea enviada.
        /// </summary>
        /// <param name="caracterRemplazar">Caracter que se quiere remplazar.</param>
        /// <param name="caracterNuevo">Carcater con el que se quiere remplazar.</param>
        /// <param name="linea">Linea donde se va a hacer el remplazo.</param>
        /// <returns>Linea con carcater remplazado</returns>
        public static string RemplazarCaracteres(char caracterRemplazar, char caracterNuevo, string linea)
        {
            #region RemplazarCaracteres
            linea = linea.Replace(caracterRemplazar, caracterNuevo);
            return linea;
            #endregion

        }

        /// <summary>
        /// Metodo que hace trim a los campos de una linea separa por caracter especial.
        /// </summary>
        /// <param name="separador">caracter de separador</param>
        /// <param name="linea">linea de datos</param>
        /// <returns>linea con trim de campos</returns>
        public static string TrimCamposLinea(char separador, string linea)
        {
            #region TrimCamposLinea
            string resultado = string.Empty;
            List<string> lines = linea.Split(separador).ToList();

            foreach (string campo in lines)
            {
                if (!string.IsNullOrEmpty(resultado))
                { resultado += "|"; }

                resultado += $"{campo.Trim()}";
            }

            return resultado;
            #endregion
        }

        /// <summary>
        /// Metodo que extrae los campos de una linea de posiciones fijas
        /// </summary>
        /// <param name="posCortes">LIsta de Cortes</param>
        /// <param name="linea">Linea</param>
        /// <returns></returns>
        public static string ExtraccionCamposSpool(List<PosCortes> posCortes, string linea)
        {
            #region ExtraccionCamposSpool
            string lineaResultado = string.Empty;
            string campo = string.Empty;
            foreach (PosCortes campos in posCortes)
            {
                if (!string.IsNullOrEmpty(lineaResultado))
                { lineaResultado += "|"; }

                if (campos.PosInicial != null && campos.Cantidad != null)
                {
                    try
                    {
                        if (campos.Cantidad > 0)
                        {
                            campo = linea.Substring(campos.PosInicial.Value, campos.Cantidad.Value).Trim();
                        }
                        else
                        {
                            campo = linea.Substring(campos.PosInicial.Value).Trim();
                        }
                    }
                    catch
                    {
                        campo = " ";
                    }
                }
                else
                {
                    campo = " ";
                }

                if (string.IsNullOrEmpty(campo))
                { campo = " "; }

                lineaResultado += campo;
            }

            return lineaResultado;
            #endregion

        }

        /// <summary>
        /// Completa una estructura con campos blancos
        /// </summary>
        /// <param name="linea">Liea</param>
        /// <param name="cantPosiciones"> Posiciones que se necesitan</param>
        /// <returns></returns>
        public static string CompletarEspaciosLinea(string linea, int cantPosiciones)
        {
            #region CompletarEspaciosLinea
            string lineaResultado = linea;
            for (int i = linea.Length; i < cantPosiciones; i++)
            {
                lineaResultado += " ";
            }


            return lineaResultado;
            #endregion

        }

        /// <summary>
        /// Da formato de monea a campo
        /// </summary>
        /// <param name="pCampo">Campo</param>
        /// <param name="pPosDecimales">Cantidad Decimales</param>
        /// <returns>Campo Formateado</returns>
        public static string StringToMoneda(string pCampo, int pPosDecimales)
        {
            #region StringToMoneda
            string resultado = pCampo;
            try
            {
                if (string.IsNullOrEmpty(pCampo))
                {
                    pCampo = "0";
                }
                NumberFormatInfo nfi = new CultureInfo(CultureInfo.InvariantCulture.Name, false).NumberFormat;
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = pPosDecimales;
                nfi.CurrencyDecimalDigits = pPosDecimales;
                nfi.CurrencySymbol = String.Empty;

                decimal Valor = Convert.ToDecimal(pCampo);
                resultado = Valor.ToString("C", nfi);

            }
            catch (Exception)
            {
                resultado = pCampo;
            }

            return resultado;
            #endregion
        }

        /// <summary>
        /// Retorna una lidea a partir de una lista de campos
        /// </summary>
        /// <param name="campos">Lista Campos</param>
        /// <param name="separador">Separador</param>
        /// <returns>Linea</returns>
        public static string ListaCamposToLinea(List<string> campos, char separador)
        {
            #region ListaCamposToLinea
            string resultado = string.Empty;

            foreach (string campo in campos)
            {
                if (!string.IsNullOrEmpty(resultado))
                { resultado += separador; }

                resultado += $"{campo.Trim()}";
            }

            return resultado;
            #endregion
        }

        /// <summary>
        /// Metodo para copiar archivo filtrando por extensión
        /// </summary>
        /// <param name="RutaEntrada">Ruta carpeta original</param>
        /// <param name="Extension">Extension a filtrar</param>
        /// <param name="RutaSalida">Ruta a copiar los archivos</param>
        public static void MoverArchivosExtension(string RutaEntrada, string Extension, string RutaSalida)
        {
            #region Mover Archivos
            try
            {
                foreach (var _Archivo in Directory.GetFiles(RutaEntrada, Extension))
                {
                    File.Copy(_Archivo, RutaSalida + "\\" + Path.GetFileName(_Archivo));
                }
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                throw;
            }
            #endregion
        }


        /// <summary>
        /// Metodo para Mover archivos de una carpeta a otra filtrada por extensión
        /// </summary>
        /// <param name="RutaEntrada">Ruta carpeta original</param>
        /// <param name="Extension">Extension a filtrar</param>
        /// <param name="RutaSalida">Ruta a mover los archivos</param>
        public static void CortarMoverArchivosExtension(string RutaEntrada, string Extension, string RutaSalida)
        {
            #region Mover Archivos
            try
            {
                foreach (var _Archivo in Directory.GetFiles(RutaEntrada, Extension))
                {
                    File.Move(_Archivo, RutaSalida + "\\" + Path.GetFileName(_Archivo));
                }
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                throw;
            }
            #endregion
        }

        /// <summary>
        /// Metodo para mover archivos de una carpeta a otra bajo condicinados
        /// </summary>
        /// <param name="RutaEntrada">Ruta carpeta original</param>
        /// <param name="Extension">Extension a filtrar</param>
        /// <param name="RutaSalida">Ruta a mover los archivos</param>
        /// <param name="CondicionNomre">Condicion nombre</param>
        /// <param name="pNombreFinal">Nombre Final Archivo</param>
        public static void MoverArchivosCondicionados(string RutaEntrada, string Extension, string RutaSalida, string CondicionNomre, string pNombreFinal)
        {
            #region Mover Archivos
            try
            {
                foreach (var _Archivo in Directory.GetFiles(RutaEntrada, Extension))
                {
                    if (Path.GetFileNameWithoutExtension(_Archivo).Contains(CondicionNomre))
                    {
                        File.Move(_Archivo, RutaSalida + "\\" + Path.GetFileName(_Archivo));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                throw;
            }
            #endregion
        }

        /// <summary>
        /// Metodo para mover archivos de una carpeta a otra
        /// </summary>
        /// <param name="RutaEntrada">Ruta carpeta original</param>        
        /// <param name="RutaSalida">Ruta a mover los archivos</param>
        public static void MoverArchivos(string RutaEntrada, string RutaSalida)
        {
            #region Mover Archivos
            try
            {
                foreach (var _Archivo in Directory.GetFiles(RutaEntrada))
                {
                    File.Move(_Archivo, RutaSalida + "\\" + Path.GetFileName(_Archivo));
                }
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                throw;
            }
            #endregion
        }
        /// <summary>
        /// Metodo que se encarga de validar si la orden de servicio termino de procesar.
        /// </summary>
        /// <param name="orden">Orden a verificar</param>
        /// <returns>Regresa el estado en el que se encuentra la orden.</returns>
        public static string ValidarOrden(string orden)
        {
            #region Valida Orden de Servicio
            try
            {
                string[] parametros = new string[3];

                parametros[0] = Utilidades.LeerAppConfig("UserWebService");
                parametros[1] = Utilidades.Base64Encode(Utilidades.LeerAppConfig("Password"));
                parametros[2] = orden;

                System.Threading.Thread.Sleep(3000);

                string estado = ServicioDelta.ValidarEstadoOrden(parametros).First().estadoCargue;

                return estado;
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                throw;
            }
            #endregion
        }

        /// <summary>
        /// Metodo para realizar la Zonificación en Delta
        /// </summary>
        /// <param name="ordenServicio">Numero Orden</param>
        /// <param name="nombreProceso">Nombre Proceso</param>
        /// <param name="courier">Numero Courrier</param>
        /// <param name="cliente">Cliente</param>
        /// <param name="proyecto">Proyecto</param>
        /// <param name="codigoParametro">Parametro</param>
        /// <param name="tipoCargue">Tipo Cargue</param>
        /// <param name="rutaArchivo">Ruta Archivo</param>
        /// <returns></returns>
        public static string RealizarSalidasZonificadas(string ordenServicio, string nombreProceso, string courier, string cliente, string proyecto, string codigoParametro, string tipoCargue, string rutaArchivo)
        {
            #region Salidas Zonificadas
            string[] parametros = new string[12];

            parametros[0] = Utilidades.LeerAppConfig("UserWebService");                        // Usuario delta
            parametros[1] = Utilidades.Base64Encode(Utilidades.LeerAppConfig("Password"));     // Paswword delta
            parametros[2] = ordenServicio;                                                     // Orden Servicio
            parametros[3] = courier;                                                           // Courrier
            parametros[4] = cliente;                                                           // Codigo Cliente  
            parametros[5] = proyecto;                                                          // Codigo Proceso
            parametros[6] = DateTime.Now.ToString("yyyy-MM-dd");                               // FechaCorte
            parametros[7] = codigoParametro;                                                   // Parametros
            parametros[8] = "diegoandresc507@gmail.com";                                       // Envio Mail Salidas
            parametros[9] = rutaArchivo;                                                       // Ruta FTP
            parametros[10] = Path.GetFileName(rutaArchivo);                                    // Nombre Archivo
            parametros[11] = "";                                                               // Archivo Base64

            procPredStruct[] resultadoDelta = ServicioDelta.SalidaZonificada(parametros);

            return resultadoDelta.Last().estadoCargue;
            #endregion
        }

        /// <summary>
        /// Metodo para desencriptar archivos
        /// </summary>
        /// <param name="ArchivosFordecrypt">Archivo encriptado</param>
        /// <param name="llave">Llave de desencriptar</param>
        /// <param name="pRutaGnuPg">Ruta GnuPg</param>
        /// <param name="pClaveDesencripcion">Clave desencriptación</param>
        public static void DesencriptarArchivos(string ArchivosFordecrypt, string llave, string pRutaGnuPg, string pClaveDesencripcion)
        {
            #region DesencriptarArchivos
            try
            {
                Gpg ArchivoEncriptado = new Gpg(pRutaGnuPg);//172.19.37.10\proyectos\Ingenieria\Diego\GNU\GnuPG\gpg2.exe
                ArchivoEncriptado.Recipient = "<" + llave + ">";
                ArchivoEncriptado.Passphrase = pClaveDesencripcion;
                string EncryptedFile = ArchivosFordecrypt;
                string NombreArchivo = Path.GetFileNameWithoutExtension(EncryptedFile);
                string UnencryptedFile = Path.GetDirectoryName(EncryptedFile) + "\\" + NombreArchivo;
                var ArchivoDesencriptado = DecryptFile(EncryptedFile, UnencryptedFile, ArchivoEncriptado);
                if (ArchivoDesencriptado == null)
                {
                    throw new Exception("Error al momento de desencriptar el archivo: " + NombreArchivo);
                }
                else
                {
                    EscribirVentanaLog("Desencripto Correctamente: " + NombreArchivo);
                }
            }
            catch (Exception ex)
            {
                DatosError StructError = new DatosError
                {
                    Clase = nameof(Helpers),
                    Metodo = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetMethod().ToString(),
                    LineaError = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber(),
                    Error = ex.Message
                };

                Helpers.EscribirLogVentana(StructError, true);
            }
            #endregion
        }

        /// <summary>
        /// Metodo para desencrptar Archivos.
        /// </summary>
        /// <param name="encryptedSourceFile">Archivo a desencriptar </param>
        /// <param name="decryptedFile">Archivo desencriptado</param>
        /// <param name="gpg">Llave de desencriptar</param>
        /// <returns>Retorna un FileInfo del archivo desencriptado.</returns>
        public static FileInfo DecryptFile(string encryptedSourceFile, string decryptedFile, Gpg gpg)
        {
            #region DecryptFile
            FileStream encryptedSourceFileStream = null;
            FileStream decryptedFileStream = null;
            try
            {
                // Mirara si el archivo encriptado viene en blanco
                if (string.IsNullOrEmpty(encryptedSourceFile))
                    throw new ArgumentException("encryptedSourceFile Parámetro está vacío o nulo", "encryptedSourceFile");
                // Mirara si el archivo desencriptado viene en blanco
                if (string.IsNullOrEmpty(decryptedFile))
                    throw new ArgumentException("decryptedFile Parámetro está vacío o nulo", "decryptedFile");

                encryptedSourceFileStream = new FileStream(encryptedSourceFile, FileMode.Open, FileAccess.Read);
                // Se asegura de que estamos al inicio del archivo.
                encryptedSourceFileStream.Position = 0;
                decryptedFileStream = new FileStream(decryptedFile, FileMode.Create, FileAccess.Write);

                // Metodo que desencripta el archivo
                gpg.Decrypt(encryptedSourceFileStream, decryptedFileStream);
                //Cerrar stream
                encryptedSourceFileStream.Close();
                decryptedFileStream.Close();
                //retorna el archivo desencriptado.
                return new FileInfo(decryptedFile);
            }
            catch (Exception)
            {
                if (encryptedSourceFileStream != null)
                {
                    encryptedSourceFileStream.Close();
                }

                if (decryptedFileStream != null)
                {
                    decryptedFileStream.Close();
                }
              
                //EscribirLog(ex.Message);
                return null;
            }
            #endregion
        }
        /// <summary>
        /// Metodo para escribir en archivo plano
        /// </summary>
        /// <param name="ruta">Ruta del archivo</param>
        /// <param name="listado">Lista string a escribir</param>
        public static void EscribirEnArchivo(string ruta, List<string> listado)
        {
            #region EscribirEnArchivo
            if (File.Exists(ruta))
            {
                using (StreamWriter streamWriter = new StreamWriter(ruta, true, Encoding.Default))
                {
                    foreach (var item in listado)
                    {
                        streamWriter.WriteLine(item);
                    }
                }
            }
            else
            {
                FileStream escritor = File.Create(ruta);

                using (StreamWriter streamWriter = new StreamWriter(escritor, Encoding.Default))
                {
                    foreach (var item in listado)
                    {
                        streamWriter.WriteLine(item);
                    }
                }

                escritor.Close();
            }
            #endregion
        }

        /// <summary>
        /// Metodo para escribir en el Log y la ventan ade ejecucion // Dependiendo el error se cierra la aplicacion
        /// </summary>
        /// <param name="strucDatosError">Estructura de Datos error</param>
        /// <param name="finalizaProceso">Bandera para finalizar proceso // True = Cierra - False = Continua </param>
        public static void EscribirLogVentana(DatosError strucDatosError, bool finalizaProceso = false)
        {
            #region EscribirLogVentana
            string Error = $"Clase: {strucDatosError.Clase} -|- Metodo:{strucDatosError.Metodo} -|- linea Error: {strucDatosError.LineaError} -|- Mensaje: {strucDatosError.Error}";

            if (!string.IsNullOrEmpty(strucDatosError.Error))
            {
                Console.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Error));
                Utilidades.EscribirLog(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Error), Utilidades.LeerAppConfig("RutaLog"));
            }

            if (finalizaProceso)
            {
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                Console.WriteLine("Presione una tecla para cerrar...");
                Console.ReadKey();
                Environment.Exit(1);
            }
            #endregion
        }

        /// <summary>
        /// Metodo creado para escribir en el Log y le Ventana de ejecucion
        /// </summary>
        /// <param name="Mensaje">Mensaje a escribir</param>
        public static void EscribirVentanaLog(string Mensaje)
        {
            #region EscribirVentanaLog
            Console.WriteLine(string.Format("{0} {1}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Mensaje));
            Utilidades.EscribirLog(Mensaje, Utilidades.LeerAppConfig("RutaLog"));
            #endregion
        }

        /// <summary>
        /// Metodo para escribir el usuario en el Log
        /// </summary>
        /// <param name="usuario">Usuario que ejecuta la aplicación</param>
        public static void EscribirLogUsuario(string usuario)
        {
            Utilidades.EscribirLog("*** Nuevo proceso ejecutado por: " + usuario, Utilidades.LeerAppConfig("RutaLog"));
        }

        /// <summary>
        /// Metodo encargado de descomprimir archivos, en este caso guias.
        /// </summary>
        /// <param name="archivos">Archivos a descomprimir</param>
        public static void DescomprimirGuias(string[] archivos)
        {
            #region Descomprimir Archivos
            foreach (string archivo in archivos)
            {
                string extension = Path.GetExtension(archivo);
                string nombre = Path.GetFileNameWithoutExtension(archivo);

                if (nombre == null || (extension == null) ||
                                       (extension.ToLower() != ".rar"))
                    continue;
                string ruta = archivo;

                IArchive iArchivo = ArchiveFactory.Open(ruta);

                ExtractionOptions opcionesDeExtraccion = new ExtractionOptions { Overwrite = true };

                foreach (IArchiveEntry item in iArchivo.Entries)
                {
                    if (!item.IsDirectory)
                    {
                        item.WriteToFile(Path.GetDirectoryName(archivo) + "\\" + nombre.Replace(".rar", ""), opcionesDeExtraccion);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Metodo para cargar las Guias
        /// </summary>
        /// <param name="rutaGuias"></param>
        /// <param name="poscicion"></param>
        /// <param name="canal"></param>
        public static void CargarGuias(string[] rutaGuias, int poscicion, string canal)
        {
            #region CargarGuias
            foreach (string archivo in rutaGuias)
            {
                string nombreArchivo = Path.GetFileNameWithoutExtension(archivo);
                string extension = Path.GetExtension(archivo);

                if (nombreArchivo != null && (extension != null && (extension.ToLower() == ".sal" && nombreArchivo.Contains("guias"))))
                {
                    Variables.Variables.Lector = new StreamReader(archivo, Encoding.Default);
                    Dictionary<string, string> dicGuiasTemp = new Dictionary<string, string>();
                    string Linea = string.Empty;
                    string[] Separador = null;

                    while ((Linea = Variables.Variables.Lector.ReadLine()) != null)
                    {
                        if (Linea.Substring(0, 4) == canal)
                        {
                            Separador = Linea.Split('|');

                            if (!dicGuiasTemp.ContainsKey(Separador[poscicion].Trim()))
                            {
                                if (Separador[poscicion].Trim() == "")
                                {
                                    if (!dicGuiasTemp.ContainsKey(Separador[poscicion].Trim()))
                                    {
                                        dicGuiasTemp.Add(Separador[poscicion].Trim(), Separador[1]);
                                    }
                                }
                                else
                                {
                                    dicGuiasTemp.Add(Separador[poscicion].Trim(), Separador[1]);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (!Variables.Variables.DicGuias.ContainsKey(nombreArchivo))
                    {
                        Variables.Variables.DicGuias.Add(nombreArchivo, dicGuiasTemp);
                    }

                    Variables.Variables.Lector.Close();
                }
            }
            #endregion
        }

        /// <summary>
        /// Metodo encargado de mover un archivo a una carpeta especifica
        /// </summary>
        /// <param name="rutaInsumoActual">Ruta archivo a mover</param>
        /// <param name="nuevaRutaDirectorioInsumo">Ruta cerpeta donde se va a mover el archivo</param>
        /// <param name="nombreInsumo">Nombre del archivo</param>
        public static void MoverArchivoaCarpeta(string rutaInsumoActual, string nuevaRutaDirectorioInsumo, string nombreInsumo)
        {
            #region MoverArchivoaCarpeta
            if (!Directory.Exists(nuevaRutaDirectorioInsumo))
            {
                Directory.CreateDirectory(nuevaRutaDirectorioInsumo);
            }

            File.Move(rutaInsumoActual, $@"{nuevaRutaDirectorioInsumo}\{nombreInsumo}");
            #endregion
        }

        public static string ObtenerNombrePaquete(string pCedula)
        {
            string resultado = " ";

            if (Variables.Variables.InsumoEtiquetasMail.ContainsKey(pCedula))
            {
                resultado = Variables.Variables.InsumoEtiquetasMail[pCedula].InsumoLinea.ElementAt(0).Substring(0, 31).Trim();
            }
            else if(Variables.Variables.InsumoEtiquetasFisico.ContainsKey(pCedula))
            {
                resultado = Variables.Variables.InsumoEtiquetasFisico[pCedula].InsumoLinea.ElementAt(0).Substring(0, 31).Trim();
            }

            return resultado;
        }

        public static string ObtenerEmail(string pCedula)
        {
            string resultado = " ";

            if (Variables.Variables.InsumoEtiquetasMail.ContainsKey(pCedula))
            {
                resultado = Variables.Variables.InsumoEtiquetasMail[pCedula].InsumoLinea.ElementAt(0).Substring(219, 60).Trim();
            }

            return resultado;
        }

    }

    public struct PosCortes
    {
        public Int32? PosInicial;
        public Int32? Cantidad;

        public PosCortes(Int32 posInicial, Int32 cantidad)
        {
            this.PosInicial = posInicial;
            this.Cantidad = cantidad;
        }

        public PosCortes(Int32? posInicial, Int32? cantidad)
        {
            this.PosInicial = posInicial;
            this.Cantidad = cantidad;
        }
    }

    /// <summary>
    /// Structura de datos Error para las Exception
    /// </summary>
    public struct DatosError
    {
        public string Metodo;
        public string Clase;
        public string Error;
        public int? LineaError;
    }
}
