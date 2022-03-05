using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using Helpers = App.Controlnsumos.Helpers;
using var = App.Variables.Variables;

namespace App.ControlCargueArchivos
{
    public class CartasCobranzaHabeasData : ICargue
    {
        private const string _producto = "CartasCobranzaHabeasData";

        public CartasCobranzaHabeasData(string pArchivo)
        {
            try
            {
                Ejecutar(pArchivo);
            }
            catch (Exception ex)
            {
                Utilidades.EscribirLog(ex.Message, Utilidades.LeerAppConfig("RutaLog"));

                Console.WriteLine("Existe un problema en la ejecucion revise el log y de ser necesario comuniquelo al ingeniero a cargo");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(1);
            }
        }

        public void CargueArchivoDiccionario(string pArchivo)
        {
            string llaveCruce = string.Empty;
            List<string> archivo = Helpers.ConvertirExcel(pArchivo);

            foreach (string linea in archivo)
            {
                if (linea.Split('|')[0].Trim().ToUpper() != "FECHA")
                {
                    llaveCruce = linea.Split('|')[1].Trim();
                    if (!var.DiccionarioExtractos.ContainsKey(llaveCruce))
                    {
                        var.DiccionarioExtractos.Add(llaveCruce, new Dictionary<string, Variables.DatosExtractos>
                            {
                                {_producto, new Variables.DatosExtractos
                                {
                                    Separador = '|',
                                    Extracto = new List<string>(){ linea}

                                }
                                }
                            });
                    }
                    else
                    {
                        if (!var.DiccionarioExtractos[llaveCruce].ContainsKey(_producto))
                        {
                            var.DiccionarioExtractos[llaveCruce].Add(_producto, new Variables.DatosExtractos
                            {
                                Separador = '|',
                                Extracto = new List<string>() { linea }
                            });
                        }
                        else
                        {
                            var.DiccionarioExtractos[llaveCruce][_producto].Extracto.Add(linea);
                        }

                    }
                }

            }

        }

        public void Ejecutar(string pArchivo)
        {
            CargueArchivoDiccionario(pArchivo);
        }
    }
}
