using DLL_Utilidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using var = App.Variables.Variables;
using System.Threading.Tasks;

namespace App.ControlCargueArchivos
{
    public class AsociadosInactivos : ICargue
    {
        private const string _producto = "AsociadosInactivos";
        public AsociadosInactivos (string pArchivo)
        {
            #region AsociadosInactivos
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
        public void CargueArchivoDiccionario(string pArchivo)
        {
            #region CargueArchivoDiccionario
            StreamReader lector = new StreamReader(pArchivo, Encoding.Default);

            string linea = string.Empty;
            bool encabezado = true;

            while ((linea = lector.ReadLine()) != null)
            {
                string[] datosLinea = linea.Split(';');

                if (encabezado)
                {
                    encabezado = false;
                    continue;
                }

                string llaveCruce = datosLinea.ElementAt(0).Replace('"', ' ').Trim();

                if (var.DiccionarioExtractos.ContainsKey(llaveCruce))
                {
                    if (var.DiccionarioExtractos[llaveCruce].ContainsKey(_producto))
                    {
                        var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea.Replace('"', ' ').Trim());
                    }
                    else
                    {
                        var.DiccionarioExtractos[llaveCruce].Add(_producto, new Variables.DatosExtractos
                        {
                            Separador = ';',
                            Extracto = new List<string>() { linea.Replace('"', ' ').Trim() }
                        });
                    }
                }
                else
                {
                    var.DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                        {
                            { _producto, new Variables.DatosExtractos
                            {
                                Separador = ';',
                                Extracto = new List<string>() { linea.Replace('"',' ').Trim() }
                            } }
                        });
                }
            }

            lector.Close();
            #endregion
        }

        public void Ejecutar(string pArchivo)
        {
            CargueArchivoDiccionario(pArchivo);
        }
    }
}
