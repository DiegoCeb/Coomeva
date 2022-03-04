﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using System.IO;

namespace App.ControlCargueArchivos
{
    public class Diccionario : ICargue
    {
        public Diccionario(string pArchivo)
        {
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));
                Environment.Exit(1);
            }
        }

        public void CargueArchivoDiccionario(string pArchivo)
        {
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;

            while ((linea = lector.ReadLine()) != null)
            {

            }

            lector.Close();
        }

        public void Ejecutar(string pArchivo)
        {
            CargueArchivoDiccionario(pArchivo);
        }
    }
}