﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App.ControlEjecucion {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class RXGeneral {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RXGeneral() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("App.ControlEjecucion.RXGeneral", typeof(RXGeneral).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Los dos últimos dígitos del número de la orden no corresponden a ningún corte de Coomeva, por favor verifique e intenten de nuevo..
        /// </summary>
        internal static string ErrorNumCorte {
            get {
                return ResourceManager.GetString("ErrorNumCorte", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El tamaño de caracteres del numero de la orden es incorrecto, el número de orden debe ser de 10 caracteres, por favor verifique y vuelva a intentar procesar..
        /// </summary>
        internal static string ErrorTamañoNumOrden {
            get {
                return ResourceManager.GetString("ErrorTamañoNumOrden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a CORTE|FECHA|ACTIVACION-PROTECCIONES|HABEASDATA|CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE|ExtractoFundacion|F99TODOSXX|R99TODOSXX|RXXI|RXXE|SMS|SOAT|E0|Asociados_Inactivos|Extracto_rotativo|EXTV|Fiducoomeva|PAPEXTVIVV|Pinos|BaseEstadoCuentaAsociados|BaseEstadoCuentaTerceros|muestras|PlanoBeneficiosEstadoCuenta|PAPEXTSUBV|Nuevos_Asociados_Fisicos|BASE_ACTIVOS_TAC|BASE_INACTIVOS_TAC|Extractos|EstadoCuenta|Despositos|TarjetasCredito|ExtractosFundacion|ExtractosRotativo|ExtractosVivienda|Libranza|Fiducoomeva|Act [resto de la cadena truncado]&quot;;.
        /// </summary>
        internal static string TitulosHistoricoCantidades {
            get {
                return ResourceManager.GetString("TitulosHistoricoCantidades", resourceCulture);
            }
        }
    }
}
