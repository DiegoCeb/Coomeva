using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Controlnsumos
{
    public class Insumos : Variables.Variables
    {
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
                    {"RXX" , new Variables.CantidadesArchivos { NombreArchivo = "Stickers", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,8) } },
                    {"SMS" , new Variables.CantidadesArchivos { NombreArchivo = "SMS", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,9) } },
                    {"SOAT" , new Variables.CantidadesArchivos { NombreArchivo = "Cartas Soat", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,10) } },
                    {"E0" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos de cuentas de ahorro", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,11) } },
                    {"Asociados_Inactivos" , new Variables.CantidadesArchivos { NombreArchivo = "Asociados Inactivos", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,12) } },
                    {"Extracto_rotativo" , new Variables.CantidadesArchivos { NombreArchivo = "Creditos rotativo", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,13) } },
                    {"EXTV" , new Variables.CantidadesArchivos { NombreArchivo = "Tarjetas de credito", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,14) } },
                    {"Fiducoomeva" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos Fiducoomeva", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,15) } },
                    {"PAPEXTVIVV" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos de vivienda", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,16) } },
                    {"Pinos" , new Variables.CantidadesArchivos { NombreArchivo = "Puntos Coomeva", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,17) } },
                    {"BaseEstadoCuentaAsociados" , new Variables.CantidadesArchivos { NombreArchivo = "Base Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,18) } },
                    {"BaseEstadoCuentaTerceros" , new Variables.CantidadesArchivos { NombreArchivo =  "Base Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,19) } },
                    {"muestras" , new Variables.CantidadesArchivos { NombreArchivo = "Base de muestras", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,20) } },
                    {"PlanoBeneficiosEstadoCuenta" , new Variables.CantidadesArchivos { NombreArchivo = "Beneficios Estado Cuenta", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,21) } },
                    {"PAPEXTSUBV" , new Variables.CantidadesArchivos { NombreArchivo = "Libranza", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,22) } },
                    {"Nuevos_Asociados_Fisicos" , new Variables.CantidadesArchivos { NombreArchivo = "Base Asociados", PesoArchivoMesAnterior = Helpers.GetTamañoHistorico(tamañoUltimoCorte,23) } }

            };
        }

        public static void CargarCantidadesExtractos(List<string> camposUltimoCorte)
        {
            CheckListProceso.CantidadesExtractosNacional.Extractos.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 24);
            CheckListProceso.CantidadesExtractosNacional.HojasEstadoCuentaSimplex.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 25);
            CheckListProceso.CantidadesExtractosNacional.HojasEstadoCuentaDuplex.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 26);
            CheckListProceso.CantidadesExtractosNacional.HojasViviendaSimplex.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 27);
            CheckListProceso.CantidadesExtractosNacional.HojasViviendaDuplex.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 28);
            CheckListProceso.CantidadesExtractosNacional.HojasDespositosSimplex.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 29);
            CheckListProceso.CantidadesExtractosNacional.HojasDespositosDuplex.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 30);
            CheckListProceso.CantidadesExtractosNacional.ExtractosVisa.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 31);
            CheckListProceso.CantidadesExtractosNacional.ExtractosMaster.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 32);
            CheckListProceso.CantidadesExtractosNacional.CartasSOAT.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 33);
            CheckListProceso.CantidadesExtractosNacional.CartasAsocHabeasData.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 34);
            CheckListProceso.CantidadesExtractosNacional.CartasCobrosHabeasData.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 35);
            CheckListProceso.CantidadesExtractosNacional.ActivacionProtecciones.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 36);
            CheckListProceso.CantidadesExtractosNacional.ExtractosPlanPagosLibranza.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 37);
            CheckListProceso.CantidadesExtractosNacional.ExtractosCreditoRotativo.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 38);
            CheckListProceso.CantidadesExtractosNacional.ExtractosMicroCredito.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 39);
            CheckListProceso.CantidadesExtractosNacional.Fiducoomeva.MesAnterior = Helpers.GetTamañoHistoricoInt(camposUltimoCorte, 40);
        }


    }
}
