using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ControlCargueArchivos
{
    public interface ICargue
    {
        void Ejecutar(string pArchivo);

        void CargueArchivoDiccionario(string pArchivo);
    }
}
