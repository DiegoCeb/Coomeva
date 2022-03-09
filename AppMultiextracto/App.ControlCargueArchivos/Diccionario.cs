using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using var = App.Variables.Variables;
using DLL_Utilidades;
using System.IO;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Diccionario
    /// </summary>
    public class Diccionario : ICargue
    {
        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Diccionario(string pArchivo)
        {
            #region Diccionario
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
            #endregion
        }

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

            while ((linea = lector.ReadLine()) != null)
            {
                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = linea.Split(';').ElementAt(3).Trim();

                if (var.InsumoDiccionarioDatos.ContainsKey(llaveCruce))
                {
                    var.InsumoDiccionarioDatos[llaveCruce].Extracto.Add(linea);
                }
                else
                {
                    var.InsumoDiccionarioDatos.Add(llaveCruce, new Variables.DatosExtractos
                    {
                        Separador = ';',
                        Extracto = new List<string> { linea }
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
    }
}
