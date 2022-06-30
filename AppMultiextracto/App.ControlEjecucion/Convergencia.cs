using App.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using App.Controlnsumos;

namespace App.ControlEjecucion
{
    /// <summary>
    /// Clase de la Convergencia del proceso
    /// </summary>
    public class Convergencia : IConvergencia
    {
        private bool _disposed = false;
        private string RutaSalidaProcesoFisico = string.Empty;
        private string RutaSalidaProcesoVault = string.Empty;

        /// <summary>
        /// Constructor Convergencia
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
            //Dispose();
            #endregion
        }

        /// <summary>
        /// Metodo para generar las salidas Virtual y publicación
        /// </summary>
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

        /// <summary>
        /// Metodo para el Ordenamiento final en base a las Guias
        /// </summary>
        private void OrdenarExtractoFinal()
        {
            #region OrdenarExtractoFinal
            Variables.Variables.RutaBaseDelta = @"C:\ProcesoCoomeva\Salida\1320229999_20220617\1320229998";
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

        /// <summary>
        /// Metodo Procesamiento en Plantas
        /// </summary>
        /// <param name="pCedula">Cedula del registro</param>
        /// <param name="pRegional">Regional del Registro</param>
        /// <param name="pConsecutivo">Consecutivo del registro</param>
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
                                Helpers.EscribirEnArchivo($@"{rutaFinalInterna}\{Variables.Variables.Orden}_{pRegional}_{extracto.Key}.sal", extracto.Value);
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
                    //no deberia entrar aca por que una cedula solo puede tener un tipo de envio Fisico o Virtual
                }

            }
            else
            {
                //Si entra aca es por que la cedula esta sin tipo de envio ver variable CedulasSinTipoEnvio
            }

            #endregion
        }

        /// <summary>
        /// Metodo Procesamiento Unificado
        /// </summary>
        /// <param name="pCedula">Cedula del registro</param>
        /// <param name="pRegional">Regional del Registro</param>
        /// <param name="pConsecutivo">Consecutivo del registro</param>
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
        /// Metodo para formatear data
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
        /// Metodo para agregar registro Formateado
        /// </summary>
        /// <param name="pCedula">Cedula registro</param>
        /// <param name="pTipoEnvio">Tipo Envio</param>
        /// <param name="pExtracto">Lista del extracto</param>
        /// <param name="pProducto">Procto Procesado</param>
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
        /// Intermediador para llamar el metodo de formateo
        /// </summary>
        /// <param name="pTipoObjeto">Tipo clase a invocar</param>
        /// <param name="pParametroEnvio">Parametros segun clase invocada</param>
        /// <returns>Objeto de la clase invocada</returns>
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
        /// Verificacion tipo de envio
        /// </summary>
        /// <param name="pCedula">Cedula registro</param>
        /// <returns>Strin con tipo de Salida</returns>
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

        /// <summary>
        /// Metodo para liberar Memoria
        /// </summary>
        public void Dispose()
        {
            #region Dispose
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
            #endregion
        }

        // Protected implementation of Dispose pattern.
        /// <summary>
        /// Metodo para liberar Memoria
        /// </summary>
        /// <param name="disposing">Bandera para limpiar variables</param>
        protected virtual void Dispose(bool disposing)
        {
            #region Dispose
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

            #endregion
        }

        /// <summary>
        /// Metodo para llenar los datps de beneficios
        /// </summary>
        private void LlenarEstructuraDatosBeneficios()
        {
            #region LlenarEstructuraDatosBeneficios();

            #region Financiero

            Variables.Variables.EstructuraBeneficios.Add(1, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Servicios*",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Financieras|Servicios*| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(2, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Productos de crédito",  new Variables.DatosPlanoBeneficios
                    {
                         Formato = "1GGG|Soluciones Financieras|Productos de crédito| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(3, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Productos de ahorro",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Financieras|Productos de ahorro| | | | "
                    }
                }
            });

            #endregion

            #region Salud

            Variables.Variables.EstructuraBeneficios.Add(4, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Medicina Prepagada",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Salud|Medicina Prepagada| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(5, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Coomeva Emergencia Médica",  new Variables.DatosPlanoBeneficios
                    {
                         Formato = "1GGG|Soluciones Salud|Coomeva Emergencia Médica| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(6, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Salud Oral",  new Variables.DatosPlanoBeneficios
                    {
                         Formato = "1GGG|Soluciones Salud|Salud Oral| | | | "
                    }
                }
            });

            #endregion

            #region Cooperativos
            Variables.Variables.EstructuraBeneficios.Add(7, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Cuota manejo TAC MasterCard",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Cooperativas|Cuota manejo TAC MasterCard| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(8, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Educativas (Bonos de descuento)",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Cooperativas|Educativas (Bonos de descuento)| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(9, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Beneficio Tasa Compensada (Coomeva Educa)",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Cooperativas|Beneficio Tasa Compensada (Coomeva Educa)| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(10, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Fondo Social de Vivienda",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Cooperativas|Fondo Social de Vivienda| | | | "
                    }
                }
            });

            #endregion

            #region Proteccion
            Variables.Variables.EstructuraBeneficios.Add(11, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Asistencia Jurídica",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Proteccion|Asistencia Jurídica| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(12, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Asistencia Pensional",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Proteccion|Asistencia Pensional| | | | "
                    }
                }
            });

            Variables.Variables.EstructuraBeneficios.Add(13, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Segunda Opinión Médica",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Proteccion|Segunda Opinión Médica| | | | "
                    }
                }
            });

            #endregion

            #region Desarrollo Empresarial
            Variables.Variables.EstructuraBeneficios.Add(14, new Dictionary<string, DatosPlanoBeneficios>()
            {
                { "Microcrédito",  new Variables.DatosPlanoBeneficios
                    {
                        Formato = "1GGG|Soluciones Desarrollo Empresarial|Microcrédito| | | | "
                    }
                }
            });
            #endregion

            #endregion
        }
    }

    /// <summary>
    /// Enumerable Orden del extracto
    /// </summary>
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
