using DLL_Utilidades;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.ControlFtp
{
    public class ControlFTP
    {
        private FtpClient _Conexion = null;

        public ControlFTP(string pHost, string pUsuario, string PContraseña)
        {
            _Conexion = new FtpClient(pHost, pUsuario, PContraseña);
        }

        public void Conectar()
        {
            // connect to the server and automatically detect working FTP settings
            _Conexion.AutoConnect();
        }

        public void Desconectar()
        {
            // disconnect! good bye!
            _Conexion.Disconnect();
        }

        private List<FtpListItem> ListarArchivos(string pRuta)
        {
            return _Conexion.GetListing(pRuta).ToList();
        }

        public void DescargarArchivosFtp(string pRutaDestino)
        {
            var archivos = ListarArchivos(Utilidades.LeerAppConfig("RutaProcesoCoomeva"));

            foreach (var archivo in archivos)
            {
                if (archivo.Type == FtpFileSystemObjectType.File)
                {
                    _Conexion.DownloadFile($@"{pRutaDestino}\{archivo.Name}", archivo.FullName);
                }
            }           
        }

    }
}
