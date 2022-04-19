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

        public static void CargarNombresArchivosChekList()
        {
            CheckListProceso.DiccionarioCantidadesArchivos = 
                new Dictionary<string, Variables.CantidadesArchivos>() {
                    { "ACTIVACION-PROTECCIONES", new Variables.CantidadesArchivos { NombreArchivo = "Activacion Protecciones" } },
                    {"HABEASDATA" , new Variables.CantidadesArchivos { NombreArchivo = "Habeas Data" } },
                    {"CARTAS_COBRANZA_HABEAS_DATA_COOMEVA_CORTE" , new Variables.CantidadesArchivos { NombreArchivo = "Cobranza Habeas Data", PesoArchivoMesActual = 0, PesoArchivoMesAnterior = 0 } },
                    {"ExtractoFundacion" , new Variables.CantidadesArchivos { NombreArchivo = "Microcredito" } },
                    {"F99TODOSXX" , new Variables.CantidadesArchivos { NombreArchivo = "Estado de cuenta Asociados" } },
                    {"R99TODOSXX" , new Variables.CantidadesArchivos { NombreArchivo = "Estado de cuenta Ex-Asociados" } },
                    {"RXX" , new Variables.CantidadesArchivos { NombreArchivo = "Stickers" } },
                    {"SMS" , new Variables.CantidadesArchivos { NombreArchivo = "SMS" } },
                    {"SOAT" , new Variables.CantidadesArchivos { NombreArchivo = "Cartas Soat" } },
                    {"E0" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos de cuentas de ahorro" } },
                    {"Asociados_Inactivos" , new Variables.CantidadesArchivos { NombreArchivo = "Asociados Inactivos" } },
                    {"Extracto_rotativo" , new Variables.CantidadesArchivos { NombreArchivo = "Creditos rotativo" } },
                    {"EXTV" , new Variables.CantidadesArchivos { NombreArchivo = "Tarjetas de credito" } },
                    {"Fiducoomeva" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos Fiducoomeva" } },
                    {"PAPEXTVIVV" , new Variables.CantidadesArchivos { NombreArchivo = "Extractos de vivienda" } },
                    {"Pinos" , new Variables.CantidadesArchivos { NombreArchivo = "Puntos Coomeva" } },
                    {"BaseEstadoCuentaAsociados" , new Variables.CantidadesArchivos { NombreArchivo = "Base Asociados" } },
                    {"BaseEstadoCuentaTerceros" , new Variables.CantidadesArchivos { NombreArchivo =  "Base Asociados" } },
                    {"muestras" , new Variables.CantidadesArchivos { NombreArchivo = "Base de muestras" } },
                    {"PlanoBeneficiosEstadoCuenta" , new Variables.CantidadesArchivos { NombreArchivo = "Beneficios Estado Cuenta" } },
                    {"PAPEXTSUBV" , new Variables.CantidadesArchivos { NombreArchivo = "Libranza" } },
                    {"Nuevos_Asociados_Fisicos" , new Variables.CantidadesArchivos { NombreArchivo = "Base Asociados" } }

            };
        }


    }
}
