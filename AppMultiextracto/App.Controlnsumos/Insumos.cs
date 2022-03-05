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
                {"BaseEstadoCuentaTerceros" , "Base Asociados"}
            };
        }


    }
}
