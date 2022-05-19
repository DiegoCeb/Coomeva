using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Variables
{
    public class Variables
    {
        public string Proceso { get; set; }
        public string NumeroOrdenProceso { get; set; }
        public string MensajeError { get; set; }

        public static Dictionary<string, Dictionary<string, DatosExtractos>> DiccionarioExtractos = new Dictionary<string, Dictionary<string, DatosExtractos>>();
        public static Dictionary<string, Dictionary<string, List<string>>> DiccionarioExtractosFormateados = new Dictionary<string, Dictionary<string, List<string>>>();
        public static Dictionary<string, DatosInsumos> InsumoDiccionarioDatos = new Dictionary<string, DatosInsumos>();

        public static CheckList CheckListProceso = new CheckList();
        public static string Orden = string.Empty;
        public static string RutaBaseDelta;
        public string NombreCorte { get; set; }
        public static List<string> CedulasSinProducto = new List<string>();
    }

    public struct DatosExtractos
    {
        public List<string> Extracto;
        public char Separador;
        public Type TipoClase;
        public bool Insumo;
    }

    public struct DatosInsumos
    {
        public List<string> InsumoLinea;
        public char Separador;
        public Type TipoClase;
    }




}
