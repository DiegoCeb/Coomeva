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

namespace App.ControlProcesos
{
    public class GestionProcesos : Variables.Variables, IControl, IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        public Dictionary<string, Type> InsumosCarga = new Dictionary<string, Type>();
        private Procesamiento _objProceso = new Procesamiento();
        public GestionProcesos()
        {
            CargarClaves();
        }
        public void Ejecutar()
        {
            //if (!objProceso.DescargaArchivos())
            //{
            //    Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
            //    System.Threading.Thread.Sleep(2000);
            //    Environment.Exit(1);
            //}

            Console.WriteLine("");
            Console.WriteLine("---Descargue Correcto de Archivos");
            Console.WriteLine("");

            if (!_objProceso.VerificacionArchivosEntrada())
            {
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
            }

            Console.WriteLine("");
            Console.WriteLine("---Verificacion de Archivos Correcta");
            Console.WriteLine("");

            //Cargamos Archivos Insumos
            CargueGeneralArchivos(Utilidades.LeerAppConfig("RutaInsumos"));

            //Cargamos Archivos Entrada
            CargueGeneralArchivos(Utilidades.LeerAppConfig("RutaEntrada"));
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

                _objProceso.CargueArchivosGlobal(archivoEntrada, IdentificarArchivo(nombreArchivo) ?? throw new Exception("No se identifico el archivo de entrada."));
            }
        }

        public Dictionary<string, Type> CargarClaves()
        {
            #region Cargar Insumos
            InsumosCarga.Add("F99TODOSXX", typeof(EstadoCuenta));
            InsumosCarga.Add("HABEASDATA", typeof(HabeasData));
            InsumosCarga.Add("EXTV", typeof(TarjetasCredito));
            InsumosCarga.Add("BASEESTADOCUENTAASOCIADOS", typeof(BaseEstadosCuentaAsociados));
            InsumosCarga.Add("BASEESTADOCUENTATERCEROS", typeof(BaseEstadosCuentaTerceros));
            InsumosCarga.Add("VIVV", typeof(ExtractosVivienda));
            InsumosCarga.Add("CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE", typeof(CartasCobranzaHabeasData));
            InsumosCarga.Add("EXTRACTOFUNDACION", typeof(ExtractosFundacion));
            InsumosCarga.Add("ACTIVACION-PROTECCIONES", typeof(ActivacionProtecciones));
            InsumosCarga.Add("TODO999", typeof(ExtractoAhorros));
            InsumosCarga.Add("FIDUCOOMEVA", typeof(Fiducoomeva));
            InsumosCarga.Add("RXX", typeof(Etiquetas));
            InsumosCarga.Add("Diccionario", typeof(Diccionario));
            //InsumosCarga.Add("Extracto_rotativo", "8");
            //InsumosCarga.Add("PAPEXTSUBV", "11");
            //InsumosCarga.Add("PlanoBeneficiosEstadoCuenta", "12");
            //InsumosCarga.Add("Pinos", "13");
            //InsumosCarga.Add("Carta_Incremento_Aportes", "14");

            return InsumosCarga;
            #endregion
        }

        private Type IdentificarArchivo(string pNombreArchivo)
        {
            foreach (var insumo in InsumosCarga)
            {
                if (pNombreArchivo.ToUpper().Contains(insumo.Key))
                {
                    return insumo.Value;
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
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

    }
}
