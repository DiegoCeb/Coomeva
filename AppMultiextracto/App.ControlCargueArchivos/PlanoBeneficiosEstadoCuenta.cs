using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Helpers = App.Controlnsumos.Helpers;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de PlanoBeneficiosEstadoCuenta
    /// </summary>
    public class PlanoBeneficiosEstadoCuenta : App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public PlanoBeneficiosEstadoCuenta(string pArchivo)
        {
            #region PlanoBeneficiosEstadoCuenta
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Helpers.EscribirLogVentana(ex.Message, true);
            }
            #endregion
        }

        /// <summary>
        /// Constructor General
        /// </summary>
        public PlanoBeneficiosEstadoCuenta()
        { }

        /// <summary>
        /// Metodo Encargado de cargar al diccionario Principal los datos PUROS, solo con limpieza.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            bool encabezado = true;

            while (!string.IsNullOrEmpty(linea = lector.ReadLine()))
            {
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = linea.Split('\t')[3].Trim();

                if (InsumoPlanoBeneficios.ContainsKey(llaveCruce))
                {
                    InsumoPlanoBeneficios[llaveCruce].InsumoLinea.Add(linea);
                }
                else
                {
                    InsumoPlanoBeneficios.Add(llaveCruce, new Variables.DatosInsumos
                    {
                        Separador = '\t',
                        InsumoLinea = new List<string> { linea }
                    });
                }
            }

            lector.Close();
            #endregion
        }

        /// <summary>
        /// Metodo que desencadena el cargue.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void Ejecutar(string pArchivo)
        {
            CargueArchivoDiccionario(pArchivo);
        }

        public List<string> FormatearArchivo(List<string> datosOriginales)
        {
            return new List<string>();
        }
    }
}
