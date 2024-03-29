﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using App.ControlEjecucion;
using DLL_Utilidades;
using App.ControlCargueArchivos;
using App.Controlnsumos;

namespace App.ControlProcesos
{
    /// <summary>
    /// Clase GestionProcesos
    /// </summary>
    public class GestionProcesos : Variables.Variables, IControl, IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        public Dictionary<string, Type> InsumosCarga = new Dictionary<string, Type>();
        public List<string> InsumosActualizarCarga = new List<string>();
        private Procesamiento _objProceso = new Procesamiento();

        /// <summary>
        /// Constructor
        /// </summary>
        public GestionProcesos()
        {
            #region GestionProcesos
            CargarClaves();
            CargarClavesInsumos();
            #endregion
        }

        /// <summary>
        /// Metodo para desencader el procesamiento
        /// </summary>
        public void Ejecutar(bool pReproceso = false)
        {
            #region Ejecutar
            CheckListProceso.FechaHoraIncio = DateTime.Now;
            CheckListProceso.UsuarioSesion = Environment.UserName;
            _objProceso.CargueDiccionarioCheckList(this.NumeroOrdenProceso);

            if (pReproceso)
            {
                NombreProceso = $"Reproceso Multiextracto Corte {CheckListProceso.Corte} ";
            }
            else
            {
                NombreProceso = $"Multiextracto Corte {CheckListProceso.Corte} {DateTime.Now.Month} ";
            }

            //if (!_objProceso.DescargaArchivos())
            //{
            //    Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
            //    System.Threading.Thread.Sleep(2000);
            //    Environment.Exit(1);
            //}

            ////Console.WriteLine("---Descargue Correcto de Archivos");
            ////Console.ReadKey();

            ////Creacion carpeta donde se almacenaran los archivos originales del proceso
            //Helpers.RutaOriginales = Directory.CreateDirectory($"{Utilidades.LeerAppConfig("RutaOriginales")}\\{NumeroOrdenProceso}_{DateTime.Now:yyyyMMdd}").FullName;
            //_objProceso.DesencriptarArchivos();

            ////Console.WriteLine("---Desencriptado Correcto de Archivos");
            //////Console.ReadKey();

            if (!_objProceso.VerificacionArchivosEntrada())
            {
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
            }

            Helpers.EscribirVentanaLog("");
            Helpers.EscribirVentanaLog("Verificacion de Archivos Correcta");
            Helpers.EscribirVentanaLog("");

            //Cargamos Archivos Entrada
            CargueArchivosInsumo(Utilidades.LeerAppConfig(RXGeneral.RutaEntrada));

            //Creacion carpeta de salida del proceso
            Helpers.RutaProceso = Directory.CreateDirectory($"{Utilidades.LeerAppConfig("RutaSalida")}\\{NumeroOrdenProceso}_{DateTime.Now:yyyyMMdd}").FullName;

            //Cargamos Archivos Insumos
            CargueGeneralArchivos(Utilidades.LeerAppConfig("RutaInsumos"));

            //Cargamos Archivos Entrada
            CargueGeneralArchivos(Utilidades.LeerAppConfig(RXGeneral.RutaEntrada));

            Console.WriteLine("Cambie los datos de la base para pruebas");
            Console.ReadKey();

            if (!_objProceso.IniciarZonificacion("fisico", NombreProceso))
            {
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo (IniciarZonificacion)");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
            }

            Helpers.EscribirVentanaLog("Finalización de Zonificación");

            //Convergencia
            Helpers.EscribirVentanaLog("Inicia Convergencia");
            _ = new Convergencia();

            //Generación de Muestras

            Helpers.EscribirVentanaLog("Inicia Extracción Muestras");
            _ = new ExtraccionMuestras();

            //Parte Mail, Generar journal PS - Cargue a vault - Cargue journal delta - cargue adjuntos en linea

            #region Pdfs Adicionales
            //preguntar lo de adjuntos en linea antes de seguir par auqe generen los PDFs

            Helpers.EscribirVentanaLog("Ingrese la ruta de PDFs adicionales en caso de existir, en caso de no llevar presione 1.");
            RutaPdfsAdicionales = Console.ReadLine();

            if (RutaPdfsAdicionales != "1")
            {
                foreach (var archivosAdicionales in Directory.GetFiles(RutaPdfsAdicionales, "*.pdf"))
                {
                    if (!PdfsCargarAdjuntosEnLinea.ContainsKey(Path.GetFileNameWithoutExtension(archivosAdicionales)))
                    {
                        PdfsCargarAdjuntosEnLinea.Add(Path.GetFileNameWithoutExtension(archivosAdicionales), archivosAdicionales);
                    }
                }
            } 
            #endregion

            Helpers.EscribirVentanaLog("Inicia Cargue de Proceso Digital");
            _objProceso.CargueProcesoDigital(NombreProceso, Utilidades.LeerAppConfig("CodigoCliente"), 
                Utilidades.LeerAppConfig("CodigoProcesoVirtual"), Utilidades.LeerAppConfig("CodigoCourier"), Utilidades.LeerAppConfig("ConfiguracionMapeoVirtual"),
                true, PdfsCargarAdjuntosEnLinea, Utilidades.LeerAppConfig("ClienteDoc1"), Utilidades.LeerAppConfig("ProductoDoc1"), Utilidades.LeerAppConfig("TipoSalida"), RutaProcesoVault);

            // Extraccion de Cantidades
            CheckListProceso.FechaHoraFin = DateTime.Now;
            Helpers.EscribirVentanaLog("Inicia Reporte de Cantidades");
            _ = new ReporteCantidades();

            _objProceso.RegistrarDatosHistoCantidades();
            Helpers.EscribirVentanaLog("Final Existoso del Proceso, revise la carpeta salidas !!!");
            Helpers.EscribirVentanaLog("Presione una tecla para cerrar...");
            Console.ReadKey();
            Environment.Exit(1);
            #endregion
        }

        /// <summary>
        /// Metodo que carga los insumos por ruta de Archivos.
        /// </summary>
        /// <param name="pRuta">Ruta de Archivos</param>
        private void CargueGeneralArchivos(string pRuta)
        {
            #region CargueGeneralArchivos
            foreach (var archivoEntrada in Directory.GetFiles(pRuta))
            {
                var nombreArchivo = Path.GetFileNameWithoutExtension(archivoEntrada);

                if (nombreArchivo == "HistoricoCantidades")
                { continue; }

                //_objProceso.CargueArchivosGlobal(archivoEntrada, IdentificarArchivo(nombreArchivo) ?? throw new Exception("No se identifico el archivo de entrada."));

                Type tipo = IdentificarArchivo(nombreArchivo);
                if (tipo != null)
                {
                    _objProceso.CargueArchivosGlobal(archivoEntrada, IdentificarArchivo(nombreArchivo));
                }
                else
                {
                    Helpers.EscribirVentanaLog($"Archivo {nombreArchivo} no identificado en la lista de insumos del proceso.");
                }
            }
            #endregion
        }

        /// <summary>
        /// Metodo que carga los insumos por ruta de Archivos.
        /// </summary>
        /// <param name="pRuta">Ruta de Archivos</param>
        private void CargueArchivosInsumo(string pRuta)
        {
            #region CargueArchivosInsumo
            foreach (var archivoEntrada in Directory.GetFiles(pRuta))
            {
                var nombreArchivo = Path.GetFileNameWithoutExtension(archivoEntrada);

                if (nombreArchivo == "HistoricoCantidades")
                { continue; }

                foreach (var nombreInusmo in InsumosActualizarCarga)
                {
                    if (nombreArchivo.ToUpper().Contains(nombreInusmo))
                    {
                        _objProceso.ActulizaInsumos(nombreInusmo, archivoEntrada);
                        break;
                    }
                }
            } 
            #endregion
        }

        /// <summary>
        /// Metodo para cargar las llaves de los insumos
        /// </summary>
        /// <returns>Lista con las llaves de insumos</returns>
        public List<string> CargarClavesInsumos()
        {
            #region CargarClavesInsumos
            InsumosActualizarCarga.Add("BASEESTADOCUENTAASOCIADOS");
            InsumosActualizarCarga.Add("BASEESTADOCUENTATERCEROS");
            InsumosActualizarCarga.Add("DICCIONARIO");
            InsumosActualizarCarga.Add("MUESTRAS");
            InsumosActualizarCarga.Add("PLANOBENEFICIOSESTADOCUENTA");

            return InsumosActualizarCarga;
            #endregion
        }

        /// <summary>
        /// Metodo para argar cargar las claves
        /// </summary>
        /// <returns>Diccionario con las llaves y tipo de cada clase</returns>
        public Dictionary<string, Type> CargarClaves()
        {
            #region Cargar Insumos
            InsumosCarga.Add("F99TODOSXX", typeof(EstadoCuenta));
            InsumosCarga.Add("HABEASDATA", typeof(HabeasData));
            InsumosCarga.Add("EXTV", typeof(TarjetasCredito));
            InsumosCarga.Add("BASEESTADOCUENTAASOCIADOS", typeof(BaseEstadosCuentaAsociados));
            InsumosCarga.Add("BASEESTADOCUENTATERCEROS", typeof(BaseEstadosCuentaTerceros));
            InsumosCarga.Add("PAPEXTVIVV", typeof(ExtractosVivienda));
            InsumosCarga.Add("CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE", typeof(CartasCobranzaHabeasData));
            InsumosCarga.Add("EXTRACTOFUNDACION", typeof(ExtractosFundacion));
            InsumosCarga.Add("ACTIVACION-PROTECCIONES", typeof(ActivacionProtecciones));
            InsumosCarga.Add("TODO999", typeof(ExtractoAhorros));
            InsumosCarga.Add("FIDUCOOMEVA", typeof(Fiducoomeva));
            InsumosCarga.Add("RXX", typeof(Etiquetas));
            InsumosCarga.Add("DICCIONARIO", typeof(Diccionario));
            InsumosCarga.Add("ASOCIADOS_INACTIVOS", typeof(AsociadosInactivos));
            InsumosCarga.Add("NUEVOS_ASOCIADOS_FISICOS", typeof(NuevosAsociadosFisico));
            InsumosCarga.Add("PINOS", typeof(Pinos));
            InsumosCarga.Add("MUESTRAS", typeof(Muestras));
            InsumosCarga.Add("PLANOBENEFICIOSESTADOCUENTA", typeof(PlanoBeneficiosEstadoCuenta));
            InsumosCarga.Add("PAPEXTSUBV", typeof(Libranza));
            InsumosCarga.Add("R99TODOSXX", typeof(EstadoCuenta));
            InsumosCarga.Add("EXTRACTO_ROTATIVO", typeof(ExtractosRotativo));
            InsumosCarga.Add("BASE_ACTIVOS_TAC", typeof(CartasTAC));
            InsumosCarga.Add("BASE_INACTIVOS_TAC", typeof(CartasTAC));
            InsumosCarga.Add("SOAT", typeof(CartasSOAT));

            return InsumosCarga;
            #endregion
        }

        /// <summary>
        /// Metodo para identificar los archivos a procesar
        /// </summary>
        /// <param name="pNombreArchivo">Nombre arhivo</param>
        /// <returns>Retorna le valor del insumo cargado</returns>
        private Type IdentificarArchivo(string pNombreArchivo)
        {
            #region IdentificarArchivo
            foreach (var insumo in InsumosCarga)
            {
                if (pNombreArchivo.ToUpper().Contains(insumo.Key))
                {
                    if (pNombreArchivo.ToUpper().Contains("EXTV"))
                    {
                        if (pNombreArchivo.ToUpper().Substring(0, 4) == "EXTV")
                        {
                            //TARJETAS
                            return InsumosCarga["EXTV"];
                        }
                        else
                        {
                            //VIVIENDA
                            return InsumosCarga["PAPEXTVIVV"];
                        }
                    }
                    else
                    {
                        return insumo.Value;
                    }
                }
            }

            return null;
            #endregion
        }

        /// <summary>
        /// Metodo para pintar el encabezado al abrir la aplicación
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Inicio()
        {
            #region Inicio
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem?.GetName();
            Version ver = assemName?.Version;

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()?.Location ?? throw new InvalidOperationException());

            string companyName = versionInfo.CompanyName;

            string version = ver?.ToString();

            Console.WriteLine("");
            Console.WriteLine(" **************");
            Console.WriteLine(" *    *****   *");
            Console.WriteLine(" *   **   **  *      " + "Procesador Multiextracto Coomeva");
            Console.WriteLine(" *  **        *      " + "Nombre:\tMultiextracto Coomeva");
            Console.WriteLine(" *  **        *      " + "Version:\t" + version);
            Console.WriteLine(" *   **   **  *      " + "Compañia:\t" + companyName);
            Console.WriteLine(" *    *****   *");
            Console.WriteLine(" **************");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.Title = "Procesos Coomeva " + version;
            #endregion
        }

        /// <summary>
        /// Metodo para mostrar el menu de la aplicación
        /// </summary>
        public void Menu()
        {
            #region Menu
            Inicio();
            Console.WriteLine("Seleccione el proceso que desea ejecutar:");
            Console.WriteLine("");
            Console.WriteLine("1. Multiextracto");
            Console.WriteLine("");
            Console.WriteLine("2. Reproceso Multiextracto");
            Console.WriteLine("");
            Proceso = Console.ReadKey().KeyChar.ToString();
            Console.WriteLine("");

            switch (Proceso)
            {
                case "1":
                    Console.WriteLine("");
                    Console.WriteLine("Ingrese el numero de orden del proceso:");
                    Console.WriteLine("");
                    NumeroOrdenProceso = Console.ReadLine();
                    Orden = NumeroOrdenProceso;
                    Console.WriteLine("");
                    Ejecutar();
                    break;

                case "2":
                    Console.WriteLine("");
                    Console.WriteLine("Ingrese el numero de orden del Reproceso:");
                    Console.WriteLine("");
                    NumeroOrdenProceso = Console.ReadLine();
                    Orden = NumeroOrdenProceso;
                    Console.WriteLine("");
                    Ejecutar(true);
                    break;

                default:
                    Console.WriteLine("Ingrese el numero de proceso valido");
                    System.Threading.Thread.Sleep(1500);
                    Console.Clear();
                    Menu();
                    break;
            }
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
                InsumosActualizarCarga.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
            #endregion
        }
    }
}
