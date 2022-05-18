﻿using App.ControlFtp;
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
                MensajeError = ex.Message;
                Utilidades.EscribirLog(MensajeError, Utilidades.LeerAppConfig("RutaLog"));
                return false;
            }

        }

        /// <summary>
        /// Metodo para verificar archivos de entrada
        /// </summary>
        /// <returns></returns>
        public bool VerificacionArchivosEntrada()
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
                    MensajeError = $"El siguiente archivo {Path.GetFileName(archivo)} no se reconoce dentro de los nombres configurados para el proceso.";
                    Utilidades.EscribirLog(MensajeError, Utilidades.LeerAppConfig("RutaLog"));
                    return false;
                }

            }

            return true;
        }


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
                string direccion = linea.Substring(31, 60).Trim();
                string segmentoCiudadDpto = linea.Substring(92, 37).Trim();
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
                MensajeError = ex.Message;
                Utilidades.EscribirLog(MensajeError, Utilidades.LeerAppConfig("RutaLog"));
                return false;
            }            
        }

        public void CargueDiccionarioCheckList()
        {
            Insumos.CargarNombresArchivosChekList();
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
