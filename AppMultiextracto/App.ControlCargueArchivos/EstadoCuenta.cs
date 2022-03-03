﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ControlCargueArchivos
{
    public class EstadoCuenta : ICargue
    {
        public EstadoCuenta(string archivo)
        {
            Ejecutar(archivo);
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