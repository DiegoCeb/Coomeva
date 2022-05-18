using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Variables;

namespace App.ControlEjecucion
{
    public interface IConvergencia
    {
        void Formatear(Dictionary<string, Dictionary<string, DatosExtractos>> datosOriginales);

    }
}
