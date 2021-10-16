using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace App.ControlFtp
{
    public class ClaseFtp : IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        private readonly SftpClient Conexion;

        public ClaseFtp(string ftpDireccion, int ftpPuerto, string ftpUsuario, string ftpClave)
        {
            Conexion = new SftpClient(ftpDireccion, ftpPuerto, ftpUsuario, ftpClave);
        }

        public void ConectarFtp()
        {
            #region Abre Conexion
            if (!Conexion.IsConnected)
            {
                Conexion.Connect();
            }
            #endregion
        }

        public void DesconectarFtp()
        {
            #region Cierra Conexion
            if (Conexion.IsConnected)
            {
                Conexion.Disconnect();
            }
            #endregion
        }

        public bool CrearcarpetaFtp(string nombreCarpeta)
        {
            #region Crea carpetas
            try
            {
                Conexion.CreateDirectory(nombreCarpeta);
                return true;
            }
            catch
            {
                return false;
            }
            #endregion
        }

        public bool CargarArchivoFtp(string rutaOriginal, string rutaFtp)
        {
            #region Carga Archivo al FTP
            try
            {
                FileStream archivoCargar = new FileStream(rutaOriginal, FileMode.Open, FileAccess.ReadWrite);
                Conexion.UploadFile(archivoCargar, rutaFtp);
                return true;
            }
            catch
            {
                return false;
            }
            #endregion
        }

        public bool DescargarArchivosFtp(string rutaFtp, string rutaDescarga, string extension, string extensionAuxiliar)
        {
            #region DescargaArchivos de un FTP.
            try
            {
                IEnumerable<SftpFile> archivosCarpetaDelta = Conexion.ListDirectory(rutaFtp);
                foreach (SftpFile archvoFtp in archivosCarpetaDelta)
                {
                    if (Path.GetExtension(archvoFtp.Name) != extension || Path.GetExtension(archvoFtp.Name) != extensionAuxiliar) continue;
                    using (FileStream fs = new FileStream(rutaDescarga + "\\" + archvoFtp.Name, FileMode.Create))
                    {
                        Conexion.DownloadFile(archvoFtp.FullName, fs);
                        fs.Flush();
                        fs.Close();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            #endregion
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }
    }
}
