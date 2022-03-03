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

        public static Dictionary<string, List<DatosExtractos>> DiccionarioExtractos = new Dictionary<string, List<DatosExtractos>>();
    }

    public struct DatosExtractos
    {
        public string producto;
        public List<string> Extracto;
    }




}
