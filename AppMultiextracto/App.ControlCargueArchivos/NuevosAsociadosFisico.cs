using App.Controlnsumos;
using DLL_Utilidades;
using System;
using System.Collections.Generic;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de NuevosAsociadosFisico
    /// </summary>
    public class NuevosAsociadosFisico : App.Variables.Variables, ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public NuevosAsociadosFisico(string pArchivo)
        {
            #region AsociadosInactivos
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
        public NuevosAsociadosFisico()
        { }

        /// <summary>
        /// Metodo Encargado de cargar al diccionario Principal los datos PUROS, solo con limpieza.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            List<string> archivo = Helpers.ConvertirExcel(pArchivo);

            bool encabezado = true;

            foreach (string linea in archivo)
            {
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = linea.Split('|')[0].Trim();

                if (InsumoNuevosAsociadosFisicos.ContainsKey(llaveCruce))
                {
                    InsumoNuevosAsociadosFisicos[llaveCruce].InsumoLinea.Add(linea);
                }
                else
                {
                    InsumoNuevosAsociadosFisicos.Add(llaveCruce, new Variables.DatosInsumos
                    {
                        Separador = '|',
                        InsumoLinea = new List<string> { linea }
                    });
                }
            }

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
