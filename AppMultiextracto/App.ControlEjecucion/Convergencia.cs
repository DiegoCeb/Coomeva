using App.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Variables;
using System.Reflection;

namespace App.ControlEjecucion
{
    public class Convergencia : IConvergencia
    {
        public Convergencia()
        {
            Formatear(Variables.Variables.DiccionarioExtractos);
            //Separar mail de fisico
            //ordenar el extracto final

        }

        public void Formatear(Dictionary<string, Dictionary<string, DatosExtractos>> datosOriginales)
        {
            foreach (var Paquete in datosOriginales)
            {
                if (Paquete.Value.ContainsKey("EstadoCuenta")) // Con Estado de cuenta
                {
                    foreach (var ElementosPaquete in Paquete.Value)
                    {
                        if (!ElementosPaquete.Value.Insumo)
                        {
                            var type = ElementosPaquete.Value.TipoClase;

                            //Obtenemos el Constructor
                            var constructorSinParametros = type.GetConstructor(Type.EmptyTypes); //Constructor genérico

                            //Creamos el objeto de manera dinámica
                            var objetoConParametros = constructorSinParametros.Invoke(new object[] { });

                            // Creamos una referencia al método   
                            var method = type.GetMethod("FormatearArchivo");

                            //Llamamos al método pasandole el objeto creado dinámicamente y los argumentos dentro de un object[]
                            var retConstructorParametrizado = method.Invoke(objetoConParametros, new object[] { ElementosPaquete.Value.Extracto });

                            //Llenar el formateado

                            //debo crear otro diccionario donde se vaya creando ya la salida final con division de fisico e email
                            



                        }
                        else
                        {
                            //Reglas especificas de que debe traer cuando es un insumo


                        }
                    }
                }
                else
                {
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
                }
            }
        }
    }
}
