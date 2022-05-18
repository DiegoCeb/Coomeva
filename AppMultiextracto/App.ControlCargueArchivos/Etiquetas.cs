using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;

namespace App.ControlCargueArchivos
{
    /// <summary>
    /// Clase que se encarga de cargar el archivo de Etiquetas
    /// </summary>
    public class Etiquetas : App.Variables.Variables, ICargue
    {
        private const string _productoMail = "EtiquetaMail";
        private const string _productoFisico = "EtiquetaFisico";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public Etiquetas(string pArchivo)
        {
            #region Etiquetas
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
        /// Cosntructor General
        /// </summary>
        public Etiquetas()
        {}

        /// <summary>
        /// Metodo Encargado de cargar al diccionario Principal los datos PUROS, solo con limpieza.
        /// </summary>
        /// <param name="pArchivo">ruta del archivo a cargar</param>
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            string producto = string.Empty;

            if (Path.GetFileNameWithoutExtension(pArchivo).Split('_').ElementAt(0).Last() == 'E')
            {
                producto = _productoMail;
            }
            else
            {
                producto = _productoFisico;
            }

            while ((linea = lector.ReadLine()) != null)
            {
                string llaveCruce = $"{linea.Substring(143, 6)}{linea.Substring(151, 5)}".TrimStart('0');

                if (DiccionarioExtractos.ContainsKey(llaveCruce))
                {
                    if (DiccionarioExtractos[llaveCruce].ContainsKey(producto))
                    {
                        DiccionarioExtractos[llaveCruce][producto].Extracto.Add(linea);
                    }
                    else
                    {
                        DiccionarioExtractos[llaveCruce].Add(producto, new Variables.DatosExtractos
                        {
                            Separador = 'P',
                            Extracto = new List<string>() { linea },
                            TipoClase = typeof(Etiquetas),
                            Insumo = true
                        });
                    }
                }
                else
                {
                    DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { producto, new Variables.DatosExtractos
                            {
                                Separador = 'P',
                                Extracto = new List<string>() { linea },
                                TipoClase = typeof(Etiquetas),
                                Insumo = true
                            } }
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
            throw new NotImplementedException();
        }
    }
}
