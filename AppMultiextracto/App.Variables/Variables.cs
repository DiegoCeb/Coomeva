using System;
using System.Collections.Generic;
using System.IO;
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
        public static Dictionary<string, Dictionary<string, string>> DicGuias = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, Dictionary<string, DatosExtractos>> DiccionarioExtractos = new Dictionary<string, Dictionary<string, DatosExtractos>>();
        public static Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> DiccionarioExtractosFormateados = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
        public static Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> DiccionarioExtractosMuestras = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
        public static Dictionary<string, DatosInsumos> InsumoDiccionarioDatos = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoPlanoBeneficios = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoBaseTerceros = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoBaseAsociados = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoMuestras = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoNuevosAsociadosFisicos = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoPinos = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoAsociadosInactivos = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoEtiquetasMail = new Dictionary<string, DatosInsumos>();
        public static Dictionary<string, DatosInsumos> InsumoEtiquetasFisico = new Dictionary<string, DatosInsumos>();
        public static SortedDictionary<int, Dictionary<string, DatosPlanoBeneficios>> EstructuraBeneficios = new SortedDictionary<int, Dictionary<string, DatosPlanoBeneficios>>();
        public static CheckList CheckListProceso = new CheckList();
        public static string Orden = string.Empty;
        public static string RutaBaseDelta;
        public static StreamReader Lector;
        public string NombreCorte { get; set; }
        public static string RutaProcesoVault { get; set; }
        public static List<string> CedulasSinTipoEnvio = new List<string>();
    }

    public struct DatosExtractos
    {
        public List<string> Extracto;
        public char Separador;
        public Type TipoClase;
    }

    public struct DatosInsumos
    {
        public List<string> InsumoLinea;
        public char Separador;
    }

    public class DatosPlanoBeneficios
    {
        public string Formato;
    }


}
