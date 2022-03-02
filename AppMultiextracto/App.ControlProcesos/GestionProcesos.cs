using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using App.ControlEjecucion;
using App.Variables;
using DLL_Utilidades;

namespace App.ControlProcesos
{
    public class GestionProcesos : Variables.Variables, IControl, IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;

        public void Ejecutar()
        {
            using (Procesamiento objProceso = new Procesamiento())
            {
                //if (!objProceso.DescargaArchivos())
                //{
                //    Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                //    System.Threading.Thread.Sleep(2000);
                //    Environment.Exit(1);
                //}

                //Console.WriteLine("");
                //Console.WriteLine("---Descargue Correcto de Archivos");
                //Console.WriteLine("");

                if (!objProceso.VerificacionArchivosEntrada())
                {
                    Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                    System.Threading.Thread.Sleep(2000);
                    Environment.Exit(1);
                }

                Console.WriteLine("");
                Console.WriteLine("---Verificacion de Archivos Correcta");
                Console.WriteLine("");

            }
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
