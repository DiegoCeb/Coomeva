using App.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using App.Controlnsumos;

namespace App.ControlEjecucion
{
    /// <summary>
    /// 
    /// </summary>
    public class Convergencia : IConvergencia
    {
        private bool _disposed = false;
        private string RutaSalidaProcesoFisico = string.Empty;
        private string RutaSalidaProcesoVault = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Convergencia()
        {
            #region Convergencia
            LlenarEstructuraDatosBeneficios();
            Formatear(Variables.Variables.DiccionarioExtractos);
            RutaSalidaProcesoFisico = Directory.CreateDirectory($@"{Path.GetDirectoryName(Variables.Variables.RutaBaseDelta)}\Impresion").FullName;
            RutaSalidaProcesoVault = Directory.CreateDirectory($@"{Path.GetDirectoryName(Variables.Variables.RutaBaseDelta)}\Vault").FullName;
            GenerarSalidaVirtualPublicacion();
            OrdenarExtractoFinal();
            Dispose();
            #endregion
        }

        private void GenerarSalidaVirtualPublicacion()
        {
            List<string> publicacion = new List<string>();

            foreach (var paqueteExtracto in Variables.Variables.DiccionarioExtractosFormateados)
            {
                string CanalInicio = $"1MUL| |{paqueteExtracto.Key}";

                bool InicioEnPaquete = false;

                foreach (var tipoenvio in paqueteExtracto.Value)
                {
                    if (tipoenvio.Key != "NA")
                    {
                        foreach (var extracto in tipoenvio.Value)
                        {
                            if (!InicioEnPaquete)
                            {
                                publicacion.Add(CanalInicio);
                                InicioEnPaquete = true;
                            }

                            publicacion.AddRange(extracto.Value);
                        }
                    }
                }
            }

            Variables.Variables.RutaProcesoVault = $"{RutaSalidaProcesoVault}\\Unificado{DateTime.Now:yyyyMMddhhmmss}.sal";
            Helpers.EscribirEnArchivo(Variables.Variables.RutaProcesoVault, publicacion);
        }

        private void OrdenarExtractoFinal()
        {
            #region OrdenarExtractoFinal
            Variables.Variables.RutaBaseDelta = @"C:\ProcesoCoomeva\Salida\1320229999_20220614\1320229998";
            Helpers.DescomprimirGuias(Directory.GetFiles(Variables.Variables.RutaBaseDelta));
            Helpers.CargarGuias(Directory.GetFiles(Variables.Variables.RutaBaseDelta), Convert.ToInt16(DLL_Utilidades.Utilidades.LeerAppConfig("CampoCrucePDF")), "1AAA");

            foreach (var guias in Variables.Variables.DicGuias)
            {
                foreach (var ordenImpresion in guias.Value)
                {
                    if (guias.Key.ToLower().Contains("bogota"))
                    {
                        ProcesarUnificado(ordenImpresion.Key, guias.Key.Split('_').ElementAt(1), ordenImpresion.Value);
                    }
                    else
                    {
                        ProcesarPlantas(ordenImpresion.Key, guias.Key.Split('_').ElementAt(1), ordenImpresion.Value);
                    }
                }
            }
            #endregion
        }

        private void ProcesarPlantas(string pCedula, string pRegional, string pConsecutivo)
        {
            #region ProcesarPlantas
            if (Variables.Variables.DiccionarioExtractosFormateados.ContainsKey(pCedula))
            {
                string rutaFinalInterna = Directory.CreateDirectory($@"{RutaSalidaProcesoFisico}\{pRegional}").FullName;

                var tipoExtracto = Variables.Variables.DiccionarioExtractosFormateados[pCedula];

                string CanalInicio = $"1MUL|{pConsecutivo}|{pCedula}";

                if (tipoExtracto.Count == 1)
                {
                    if (tipoExtracto.FirstOrDefault().Key == "Fisico")
                    {
                        var paqueteExtracto = tipoExtracto["Fisico"];
                        bool extractoEscrito = false;
                        bool InicioEnPaquete = false;

                        foreach (var extracto in paqueteExtracto)
                        {
                            extractoEscrito = false;

                            for (int i = 0; i <= 4; i++)
                            {
                                var producto = (OrdenExtracto)i;

                                if (producto.ToString() == extracto.Key)
                                {
                                    if (!InicioEnPaquete)
                                    {
                                        extracto.Value.Insert(0, CanalInicio);
                                        InicioEnPaquete = true;
                                    }

                                    Helpers.EscribirEnArchivo($@"{rutaFinalInterna}\{Variables.Variables.Orden}_{pRegional}_MultiExtracto.sal", extracto.Value);
                                    extractoEscrito = true;
                                    break;
                                }
                            }

                            if (!extractoEscrito)
                            {
                                extracto.Value.Insert(0, CanalInicio);
                                Helpers.EscribirEnArchivo($@"{RutaSalidaProcesoFisico}\{Variables.Variables.Orden}_{pRegional}_{extracto.Key}.sal", extracto.Value);
                            }
                        }
                    }
                    else
                    {
                        //No deberia entrar aca por que en teoria se zonifico solo lo fisico
                    }
                }
                else
                {
                    //no deberia entrar aca por que una dcedula solo puede tener un tipo de envio Fisico o Virtual
                }

            }
            else
            {
                //Si entra aca es por que la cedula esta sin tipo de envio ver variable CedulasSinTipoEnvio
            }

            #endregion
        }

        private void ProcesarUnificado(string pCedula, string pRegional, string pConsecutivo)
        {
            #region ProcesarUnificado
            if (Variables.Variables.DiccionarioExtractosFormateados.ContainsKey(pCedula))
            {
                string RutafinalInterna = Directory.CreateDirectory($@"{RutaSalidaProcesoFisico}\{pRegional}").FullName;

                var tipoExtracto = Variables.Variables.DiccionarioExtractosFormateados[pCedula];

                string CanalInicio = $"1MUL|{pConsecutivo}|{pCedula}";
                bool InicioEnPaquete = false;

                if (tipoExtracto.Count == 1)
                {
                    if (tipoExtracto.FirstOrDefault().Key == "Fisico")
                    {
                        var paqueteExtracto = tipoExtracto["Fisico"];

                        foreach (var extracto in paqueteExtracto)
                        {
                            if (!InicioEnPaquete)
                            {
                                extracto.Value.Insert(0, CanalInicio);
                                InicioEnPaquete = true;
                            }

                            Helpers.EscribirEnArchivo($@"{RutafinalInterna}\{Variables.Variables.Orden}_{pRegional}_MultiExtracto.sal", extracto.Value);
                        }
                    }
                }
                else
                {
                    //no deberia entrar aca por que una dcedula solo puede tener un tipo de envio Fisico o Virtual
                }
            }
            else
            {
                //Si entra aca es por que la cedula esta sin tipo de envio ver variable CedulasSinTipoEnvio
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datosOriginales"></param>
        public void Formatear(Dictionary<string, Dictionary<string, DatosExtractos>> datosOriginales)
        {
            #region Formatear
            string tipoEnvio = string.Empty;

            foreach (var Paquete in datosOriginales)
            {
                tipoEnvio = VerificarTipoEnvio(Paquete.Key);

                if (tipoEnvio == "NA")
                {
                    continue;
                }

                foreach (var ElementosPaquete in Paquete.Value)
                {
                    AgregarFormateado(Paquete.Key, tipoEnvio, InvocarMetodoFormateo(ElementosPaquete.Value.TipoClase, ElementosPaquete.Value.Extracto) as List<string>, ElementosPaquete.Value.TipoClase.Name);
                }
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCedula"></param>
        /// <param name="pTipoEnvio"></param>
        /// <param name="pExtracto"></param>
        private void AgregarFormateado(string pCedula, string pTipoEnvio, List<string> pExtracto, string pProducto)
        {
            #region AgregarFormateado
            if (Variables.Variables.DiccionarioExtractosFormateados.ContainsKey(pCedula))
            {
                if (Variables.Variables.DiccionarioExtractosFormateados[pCedula].ContainsKey(pTipoEnvio))
                {
                    if (Variables.Variables.DiccionarioExtractosFormateados[pCedula][pTipoEnvio].ContainsKey(pProducto))
                    {
                        Variables.Variables.DiccionarioExtractosFormateados[pCedula][pTipoEnvio][pProducto].AddRange(pExtracto);
                    }
                    else
                    {
                        Variables.Variables.DiccionarioExtractosFormateados[pCedula][pTipoEnvio].Add(pProducto, pExtracto);
                    }
                }
                else
                {
                    //No deberia entrar aca Cedula que tiene envio fisico y mail ???
                }
            }
            else
            {
                Variables.Variables.DiccionarioExtractosFormateados.Add(pCedula, new Dictionary<string, Dictionary<string, List<string>>>
                {
                    { pTipoEnvio, new Dictionary<string, List<string>> { { pProducto, pExtracto } } }
                });
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTipoObjeto"></param>
        /// <param name="pParametroEnvio"></param>
        /// <returns></returns>
        private object InvocarMetodoFormateo(Type pTipoObjeto, List<string> pParametroEnvio)
        {
            #region InvocarMetodoFormateo
            Type type = pTipoObjeto;

            //Obtenemos el Constructor
            ConstructorInfo constructorSinParametros = type.GetConstructor(Type.EmptyTypes); //Constructor genérico

            //Creamos el objeto de manera dinámica
            object objetoConParametros = constructorSinParametros.Invoke(new object[] { });

            // Creamos una referencia al método   
            MethodInfo method = type.GetMethod("FormatearArchivo");

            //Llamamos al método pasandole el objeto creado dinámicamente y los argumentos dentro de un object[]
            object retornoConstructorParametrizado = method.Invoke(objetoConParametros, new object[] { pParametroEnvio });

            return retornoConstructorParametrizado;
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCedula"></param>
        /// <returns></returns>
        private string VerificarTipoEnvio(string pCedula)
        {
            #region Identificacion Tipo de envio

            string resultado = string.Empty;

            if (Variables.Variables.InsumoEtiquetasMail.ContainsKey(pCedula))
            {
                resultado = "Virtual";
            }
            else if (Variables.Variables.InsumoEtiquetasFisico.ContainsKey(pCedula))
            {
                resultado = "Fisico";
            }
            else
            {
                resultado = "NA";
                Variables.Variables.CedulasSinTipoEnvio.Add(pCedula);
            }

            return resultado;
            #endregion
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
                Variables.Variables.DiccionarioExtractos.Clear();
                Variables.Variables.InsumoDiccionarioDatos.Clear();
                Variables.Variables.InsumoPlanoBeneficios.Clear();
                Variables.Variables.InsumoBaseTerceros.Clear();
                Variables.Variables.InsumoBaseAsociados.Clear();
                Variables.Variables.InsumoNuevosAsociadosFisicos.Clear();
                Variables.Variables.InsumoPinos.Clear();
                Variables.Variables.InsumoAsociadosInactivos.Clear();
                Variables.Variables.InsumoEtiquetasMail.Clear();
                Variables.Variables.InsumoEtiquetasFisico.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

        private void LlenarEstructuraDatosBeneficios()
        {
            #region LlenarEstructuraDatosBeneficios();

            Variables.Variables.EstructuraBeneficios.Add("Servicios*", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Financieras|Servicios*| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Productos de crédito", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Financieras|Productos de crédito| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Productos de ahorro", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Financieras|Productos de ahorro| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Medicina Prepagada", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Salud|Medicina Prepagada| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Coomeva Emergencia Médica", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Salud|Coomeva Emergencia Médica| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Salud Oral", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Salud|Salud Oral| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Cuota manejo TAC MasterCard", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Cooperativas|Cuota manejo TAC MasterCard| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Educativas (Bonos de descuento)", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Cooperativas|Educativas (Bonos de descuento)| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Beneficio Tasa Compensada (Coomeva Educa)", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Cooperativas|Beneficio Tasa Compensada (Coomeva Educa)| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Fondo Social de Vivienda", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Cooperativas|Fondo Social de Vivienda| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Asistencia Jurídica", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Proteccion|Asistencia Jurídica| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Asistencia Pensional", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Proteccion|Asistencia Pensional| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Segunda Opinión Médica", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Proteccion|Segunda Opinión Médica| | | | "
            });
            Variables.Variables.EstructuraBeneficios.Add("Microcrédito", new Variables.DatosPlanoBeneficios
            {
                Formato = "1GGG|Soluciones Desarrollo Empresarial|Microcrédito| | | | "
            });
            #endregion
        }
    }

    public enum OrdenExtracto
    {
        [System.ComponentModel.Description("EstadoCuenta")]
        EstadoCuenta = 0,
        [System.ComponentModel.Description("ExtractoAhorros")]
        ExtractoAhorros = 1,
        [System.ComponentModel.Description("ExtractosVivienda")]
        ExtractosVivienda = 2,
        [System.ComponentModel.Description("CartasTAC")]
        CartasTAC = 3
    }

}
