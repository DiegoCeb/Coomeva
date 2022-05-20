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
        private bool _disposed = false;

        /// <summary>
        /// 
        /// </summary>
        public Convergencia()
        {
            #region Convergencia
            Formatear(Variables.Variables.DiccionarioExtractos);
            //ordenar el extracto final
            //Separar por data fisica

            Dispose();
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
                    AgregarFormateado(Paquete.Key, tipoEnvio, InvocarMetodoFormateo(ElementosPaquete.Value.TipoClase, ElementosPaquete.Value.Extracto) as List<string>);
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
                Variables.Variables.InsumoMuestras.Clear();
                Variables.Variables.InsumoNuevosAsociadosFisicos.Clear();
                Variables.Variables.InsumoPinos.Clear();
                Variables.Variables.InsumoAsociadosInactivos.Clear();
                Variables.Variables.InsumoEtiquetasMail.Clear();
                Variables.Variables.InsumoEtiquetasFisico.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

    }
}
