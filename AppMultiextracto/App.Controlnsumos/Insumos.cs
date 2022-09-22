using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Controlnsumos
{
    /// <summary>
    /// Clase Insumo
    /// </summary>
    public class Insumos : Variables.Variables
    {
        /// <summary>
        /// Metodo relacionando los archivos y su nombre para procesar
        /// </summary>
        /// <returns>Diccionario de nombre e identificador de archivo</returns>
        public static Dictionary<string, string> CargarNombresArchivos()
        {
            return new Dictionary<string, string>() {
                {"ACTIVACION-PROTECCIONES" , "Activacion Protecciones"},
                {"HABEASDATA" , "Habeas Data"},
                {"CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE" , "Cobranza Habeas Data"},
                {"ExtractoFundacion" , "Microcredito"},
                {"F99TODOSXX" , "Estado de cuenta Asociados"},
                {"R99TODOSXX" , "Estado de cuenta Ex-Asociados"},
                {"RXX" , "Stickers"},
                {"SMS" , "SMS"},
                {"SOAT" , "Cartas Soat"},
                {"E0" , "Extractos de cuentas de ahorro"},
                {"Asociados_Inactivos" , "Asociados Inactivos"},
                {"Extracto_rotativo" , "Creditos rotativo"},
                {"EXTV" , "Tarjetas de credito"},
                {"Fiducoomeva" , "Extractos Fiducoomeva"},
                {"PAPEXTVIVV" , "Extractos de vivienda"},
                {"Pinos" , "Puntos Coomeva"},
                {"BaseEstadoCuentaAsociados" , "Base Asociados"},
                {"BaseEstadoCuentaTerceros" , "Base Asociados"},
                {"muestras" , "Base de muestras"},
                {"PlanoBeneficiosEstadoCuenta" , "Beneficios Estado Cuenta"},
                {"PAPEXTSUBV" , "Libranza"},                
                {"Nuevos_Asociados_Fisicos" , "Base Asociados"}

            };
        }

        /// <summary>
        /// Metodo para cargar los nombre de los archivos en  el chekList
        /// </summary>
        /// <param name="tamañoUltimoCorte">Tamaño archivo ultimo corte procesado</param>
        public static void CargarNombresArchivosChekList(List<string> tamañoUltimoCorte)
        {
            CheckListProceso.DiccionarioCantidadesArchivos = 
                new Dictionary<string, Variables.CantidadesArchivos>() {
                    { "ACTIVACION-PROTECCIONES", new Variables.CantidadesArchivos { NombreArchivo = "Activacion Protecciones", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,2)  } },
                    {"HABEASDATA" , new Variables.CantidadesArchivos { NombreArchivo = "Habeas Data", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,3) } },
                    {"CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE" , new Variables.CantidadesArchivos { NombreArchivo = "Cobranza Habeas Data" , PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,4) } },
                    {"ExtractoFundacion" , new Variables.CantidadesArchivos { NombreArchivo = "Microcredito", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,5) } },
                    {"F99TODOSXX" , new Variables.CantidadesArchivos { NombreArchivo = "Estado de cuenta Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,6) } },
                    {"R99TODOSXX" , new Variables.CantidadesArchivos { NombreArchivo = "Estado de cuenta Ex-Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,7) } },
                    {"RXXI" , new Variables.CantidadesArchivos { NombreArchivo = "Stickers", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,8) } },
                    {"RXXE" , new Variables.CantidadesArchivos { NombreArchivo = "Stickers", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,9) } },
                    {"SMS" , new Variables.CantidadesArchivos { NombreArchivo = "SMS", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,10) } },
                    {"SOAT" , new Variables.CantidadesArchivos { NombreArchivo = "Cartas Soat", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,11) } },
                    {"E0" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos de cuentas de ahorro", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,12) } },
                    {"Asociados_Inactivos" , new Variables.CantidadesArchivos { NombreArchivo = "Asociados Inactivos", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,13) } },
                    {"Extracto_rotativo" , new Variables.CantidadesArchivos { NombreArchivo = "Creditos rotativo", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,14) } },
                    {"EXTV" , new Variables.CantidadesArchivos { NombreArchivo = "Tarjetas de credito", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,15) } },
                    {"Fiducoomeva" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos Fiducoomeva", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,16) } },
                    {"PAPEXTVIVV" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos de vivienda", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,17) } },
                    {"Pinos" , new Variables.CantidadesArchivos { NombreArchivo = "Puntos Coomeva", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,18) } },
                    {"BaseEstadoCuentaAsociados" , new Variables.CantidadesArchivos { NombreArchivo = "Base Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,19) } },
                    {"BaseEstadoCuentaTerceros" , new Variables.CantidadesArchivos { NombreArchivo =  "Base Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,20) } },
                    {"muestras" , new Variables.CantidadesArchivos { NombreArchivo = "Base de muestras", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,21) } },
                    {"PlanoBeneficiosEstadoCuenta" , new Variables.CantidadesArchivos { NombreArchivo = "Beneficios Estado Cuenta", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,22) } },
                    {"PAPEXTSUBV" , new Variables.CantidadesArchivos { NombreArchivo = "Libranza", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,23) } },
                    {"Nuevos_Asociados_Fisicos" , new Variables.CantidadesArchivos { NombreArchivo = "Base Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,24) } },
                    {"BASE_ACTIVOS_TAC" , new Variables.CantidadesArchivos { NombreArchivo = "Base Activos Tac", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,25) } },
                    {"BASE_INACTIVOS_TAC" , new Variables.CantidadesArchivos { NombreArchivo = "Base Inactivos Tac", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,26) } }
            };
        }

        /// <summary>
        /// Metodo para cargar las cantidades del ultimo corte procesado
        /// </summary>
        /// <param name="camposUltimoCorte">Campos ultimo corte procesado</param>
        public static void CargarCantidadesExtractos(List<string> camposUltimoCorte)
        {
            CheckListProceso.CantidadesProducto.Extractos.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 27);
            CheckListProceso.CantidadesProducto.Extractos.Nombre = "Extractos";
            CheckListProceso.CantidadesProducto.EstadoCuenta.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 28);
            CheckListProceso.CantidadesProducto.EstadoCuenta.Nombre = "EstadoCuenta";
            CheckListProceso.CantidadesProducto.Despositos.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 29);
            CheckListProceso.CantidadesProducto.Despositos.Nombre = "Despositos";
            CheckListProceso.CantidadesProducto.TarjetasCredito.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 30);
            CheckListProceso.CantidadesProducto.TarjetasCredito.Nombre = "TarjetasCredito";
            CheckListProceso.CantidadesProducto.ExtractosFundacion.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 31);
            CheckListProceso.CantidadesProducto.ExtractosFundacion.Nombre = "ExtractosFundacion";
            CheckListProceso.CantidadesProducto.ExtractosRotativo.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 32);
            CheckListProceso.CantidadesProducto.ExtractosRotativo.Nombre = "ExtractosRotativo";
            CheckListProceso.CantidadesProducto.ExtractosVivienda.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 33);
            CheckListProceso.CantidadesProducto.ExtractosVivienda.Nombre = "ExtractosVivienda";
            CheckListProceso.CantidadesProducto.Libranza.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 34);
            CheckListProceso.CantidadesProducto.Libranza.Nombre = "Libranza";
            CheckListProceso.CantidadesProducto.Fiducoomeva.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 35);
            CheckListProceso.CantidadesProducto.Fiducoomeva.Nombre = "ExtraFiducoomevactos";
            CheckListProceso.CantidadesProducto.ActivacionProtecciones.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 36);
            CheckListProceso.CantidadesProducto.ActivacionProtecciones.Nombre = "ActivacionProtecciones";
            CheckListProceso.CantidadesProducto.CartasCobranzaHabeasData.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 37);
            CheckListProceso.CantidadesProducto.CartasCobranzaHabeasData.Nombre = "CartasCobranzaHabeasData";
            CheckListProceso.CantidadesProducto.HabeasData.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 38);
            CheckListProceso.CantidadesProducto.HabeasData.Nombre = "HabeasData";
            CheckListProceso.CantidadesProducto.CartasTAC.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 39);
            CheckListProceso.CantidadesProducto.CartasTAC.Nombre = "CartasTAC";
            CheckListProceso.CantidadesProducto.CartasSOAT.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 40);
            CheckListProceso.CantidadesProducto.CartasSOAT.Nombre = "CartasSOAT";
        }
    }
}
