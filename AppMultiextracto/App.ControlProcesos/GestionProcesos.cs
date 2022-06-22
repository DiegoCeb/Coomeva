using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using App.ControlEjecucion;
using App.Variables;
using DLL_Utilidades;
using App.ControlCargueArchivos;
using App.Controlnsumos;

namespace App.ControlProcesos
{
    public class GestionProcesos : Variables.Variables, IControl, IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        public Dictionary<string, Type> InsumosCarga = new Dictionary<string, Type>();
        public List<string> InsumosActulizarCarga = new List<string>();
        private Procesamiento _objProceso = new Procesamiento();
        public GestionProcesos()
        {
            CargarClaves();
            CargarClavesInsumos();
        }
        public void Ejecutar()
        {
            CheckListProceso.FechaHoraIncio = DateTime.Now;
            _objProceso.CargueDiccionarioCheckList(this.NumeroOrdenProceso);

            //if (!_objProceso.DescargaArchivos())
            //{
            //    Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
            //    System.Threading.Thread.Sleep(2000);
            //    Environment.Exit(1);
            //}

            //Console.WriteLine("---Descargue Correcto de Archivos");
            //Console.ReadKey();

            ////Creacion carpeta donde se almacenaran los archivos originales del proceso
            //Helpers.RutaOriginales = Directory.CreateDirectory($"{Utilidades.LeerAppConfig("RutaOriginales")}\\{NumeroOrdenProceso}_{DateTime.Now:yyyyMMdd}").FullName;
            //_objProceso.DesencriptarArchivos();

            //Console.WriteLine("---Desencriptado Correcto de Archivos");
            //Console.ReadKey();

            if (!_objProceso.VerificacionArchivosEntrada())
            {
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
            }

            Console.WriteLine("");
            Console.WriteLine("---Verificacion de Archivos Correcta");
            Console.WriteLine("");

            //Cargamos Archivos Entrada
            CargueArchivosInsumo(Utilidades.LeerAppConfig(RXGeneral.RutaEntrada));

            //Creacion carpeta de salida del proceso
            Helpers.RutaProceso = Directory.CreateDirectory($"{Utilidades.LeerAppConfig("RutaSalida")}\\{NumeroOrdenProceso}_{DateTime.Now:yyyyMMdd}").FullName;

            //Cargamos Archivos Insumos
            CargueGeneralArchivos(Utilidades.LeerAppConfig("RutaInsumos"));

            //Cargamos Archivos Entrada
            CargueGeneralArchivos(Utilidades.LeerAppConfig(RXGeneral.RutaEntrada));

            //Console.WriteLine("Cambie los datos de la base para pruebas");
            //Console.ReadKey();

            //if (!_objProceso.IniciarZonificacion("fisico", $"MutiExtracto{DateTime.Now:yyyyMMdd}"))
            //{
            //    Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo (IniciarZonificacion)");
            //    System.Threading.Thread.Sleep(2000);
            //    Environment.Exit(1);
            //}

            //Console.WriteLine("termino zonificacion revise antes de convergencia");
            //Console.ReadKey();

            //Convergencia
            _ = new Convergencia();

            //Generación de Muestras

            //_ = new ExtraccionMuestras();

            //Parte Mail, Generar journal PS - Cargue a vault - Cargue journal delta - cargue adjuntos en linea

            //1. generar PS - JRN con este al inicio EX_FID
            //3. validar que queden como "solo publicacion"
            //4. validar el tema de la plantilla
            //5. Hacer lo de adjuntos en linea _adicional probarlo

            //preguntar lo de adjuntos en linea antes de seguir par auqe generen los PDFs

            _objProceso.CargueProcesoDigital($"Corte{Orden}_{DateTime.Now:yyyyMMddhhmmss}", Utilidades.LeerAppConfig("CodigoCliente"), 
                Utilidades.LeerAppConfig("CodigoProcesoVirtual"), Utilidades.LeerAppConfig("CodigoCourier"), Utilidades.LeerAppConfig("ConfiguracionMapeoVirtual"),
                false/*llevapdfs de adjuntos en linea*/, "ruta de los archivos para cargar en adjuntos en linea", Utilidades.LeerAppConfig("ClienteDoc1"), Utilidades.LeerAppConfig("ProductoDoc1"), Utilidades.LeerAppConfig("TipoSalida"), RutaProcesoVault);

            //Proceso SMS

            //Reportes

            _objProceso.RegistrarDatosHistoCantidades();

        }

        /// <summary>
        /// Metodo que carga los insumos por ruta de Archivos.
        /// </summary>
        /// <param name="pRuta">Ruta de Archivos</param>
        private void CargueGeneralArchivos(string pRuta)
        {
            foreach (var archivoEntrada in Directory.GetFiles(pRuta))
            {
                var nombreArchivo = Path.GetFileNameWithoutExtension(archivoEntrada);

                if (nombreArchivo == "HistoricoCantidades")
                { continue; }

                _objProceso.CargueArchivosGlobal(archivoEntrada, IdentificarArchivo(nombreArchivo) ?? throw new Exception("No se identifico el archivo de entrada."));
            }
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

                foreach (var nombreInusmo in InsumosActulizarCarga)
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

        public List<string> CargarClavesInsumos()
        {
            #region CargarClavesInsumos
            InsumosActulizarCarga.Add("BASEESTADOCUENTAASOCIADOS");
            InsumosActulizarCarga.Add("BASEESTADOCUENTATERCEROS");
            InsumosActulizarCarga.Add("DICCIONARIO");
            InsumosActulizarCarga.Add("MUESTRAS");
            InsumosActulizarCarga.Add("PLANOBENEFICIOSESTADOCUENTA");

            return InsumosActulizarCarga;
            #endregion
        }

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

            return InsumosCarga;
            #endregion
        }

        private Type IdentificarArchivo(string pNombreArchivo)
        {
            foreach (var insumo in InsumosCarga)
            {
                if (pNombreArchivo.ToUpper().Contains(insumo.Key))
                {
                    if (pNombreArchivo.ToUpper().Contains("EXTV"))
                    {
                        if (pNombreArchivo.ToUpper().Substring(0, 4) == "EXTV")
                        {
                            return InsumosCarga["EXTV"];
                        }
                        else
                        {
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
        }

        public void Inicio()
        {
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
        }

        public void Menu()
        {
            Inicio();
            Console.WriteLine("Seleccione el proceso que desea ejecutar:");
            Console.WriteLine("");
            Console.WriteLine("1. Multiextracto");
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

                default:
                    Console.WriteLine("Ingrese el numero de proceso valido");
                    System.Threading.Thread.Sleep(1500);
                    Console.Clear();
                    Menu();
                    break;
            }
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
                InsumosActulizarCarga.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

    }
}
