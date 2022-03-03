using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ControlEjecucion
{
    public interface IProcess
    {
        bool DescargaArchivos();

        bool VerificacionArchivosEntrada();

        bool CargueArchivosGlobal<TEntidad>(string pArchivo, TEntidad pEntidadArchivo);
    }
}
