using App.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace App.ControlEjecucion
{
    /// <summary>
    /// 
    /// </summary>
    public class Convergencia : IConvergencia
    {
        /// <summary>
        /// 
        /// </summary>
        public Convergencia()
        {
            #region Convergencia
            Formatear(Variables.Variables.DiccionarioExtractos);
            //Separar mail de fisico
            //ordenar el extracto final

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
                if (Paquete.Value.ContainsKey("EstadoCuenta")) // Con Estado de cuenta
                {
                    #region Producto principal Existe

                    #region Identificacion Tipo de envio
                    if (Paquete.Value.ContainsKey("EtiquetasMail"))
                    {
                        tipoEnvio = "Virtual";
                    }
                    else
                    {
                        tipoEnvio = "Fsiico";
                    }
                    #endregion

                    foreach (var ElementosPaquete in Paquete.Value)
                    {
                        if (!ElementosPaquete.Value.Insumo)
                        {
                            AgregarFormateado(Paquete.Key, tipoEnvio, InvocarMetodoFormateo(ElementosPaquete.Value.TipoClase, ElementosPaquete.Value.Extracto) as List<string>);
                        }
                        else
                        {
                            //Reglas especificas de que debe traer cuando es un insumo


                        }
                    }
                    #endregion
                }
                else
                {
                    #region Producto Principal No existe
                    //Los que no tienen estado de cuenta, se verifica a ver si no tienen otros productos que se impriman o envien mail
                    bool ExisteProducto = false;

                    foreach (var ElementosPaquete in Paquete.Value)
                    {
                        if (ElementosPaquete.Value.Insumo != true)
                        {
                            ExisteProducto = true;
                        }
                    }

                    if (ExisteProducto)
                    {
                        //Existe un producto diferente de estado de cuenta

                    }
                    else
                    {
                        Variables.Variables.CedulasSinProducto.Add(Paquete.Key);
                    }
                    #endregion
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
        private void AgregarFormateado(string pCedula, string pTipoEnvio, List<string> pExtracto)
        {
            #region AgregarFormateado
            if (Variables.Variables.DiccionarioExtractosFormateados.ContainsKey(pCedula))
            {
                if (Variables.Variables.DiccionarioExtractosFormateados[pCedula].ContainsKey(pTipoEnvio))
                {
                    Variables.Variables.DiccionarioExtractosFormateados[pCedula][pTipoEnvio].AddRange(pExtracto);
                }
                else
                {
                    //Cedula que tiene envio fisico y mail ???
                    Variables.Variables.DiccionarioExtractosFormateados[pCedula].Add(pTipoEnvio, pExtracto);
                }
            }
            else
            {
                Variables.Variables.DiccionarioExtractosFormateados.Add(pCedula, new Dictionary<string, List<string>>
                {
                    { pTipoEnvio, pExtracto }
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

    }
}
