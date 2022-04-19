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
        public static Dictionary<string, DatosExtractos> InsumoDiccionarioDatos = new Dictionary<string, DatosExtractos>();
    }

    public struct DatosExtractos
    {
        public List<string> Extracto;
        public char Separador;
    }




}
