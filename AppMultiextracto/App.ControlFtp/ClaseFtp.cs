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

        /// <summary>
        /// Constructor de la clase: ClaseFtp
        /// </summary>
        /// <param name="ftpDireccion"></param>
        /// <param name="ftpPuerto"></param>
        /// <param name="ftpUsuario"></param>
        /// <param name="ftpClave"></param>
        public ClaseFtp(string ftpDireccion, int ftpPuerto, string ftpUsuario, string ftpClave)
        {
            Conexion = new SftpClient(ftpDireccion, ftpPuerto, ftpUsuario, ftpClave);
        }

        /// <summary>
        /// Metodo para conectarse a FTP
        /// </summary>
        public void ConectarFtp()
        {
            #region Abre Conexion
            if (!Conexion.IsConnected)
            {
                Conexion.Connect();
            }
            #endregion
        }

        /// <summary>
        /// Metodo para desconectarse del FTP
        /// </summary>
        public void DesconectarFtp()
        {
            #region Cierra Conexion
            if (Conexion.IsConnected)
            {
                Conexion.Disconnect();
            }
            #endregion
        }

        /// <summary>
        /// Metodo para crear una carpeta en el FTP
        /// </summary>
        /// <param name="nombreCarpeta"></param>
        /// <returns>Booleano True = OK / False = KO </returns>
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

        /// <summary>
        /// Metodo para cargar archivos en el FTP
        /// </summary>
        /// <param name="rutaOriginal"></param>
        /// <param name="rutaFtp"></param>
        /// <returns>Booleano True = OK / False = KO </returns>
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

        /// <summary>
        /// Metodo para descargar archivos del FTP
        /// </summary>
        /// <param name="rutaFtp"></param>
        /// <param name="rutaDescarga"></param>
        /// <param name="extension"></param>
        /// <param name="extensionAuxiliar"></param>
        /// <returns>Booleano True = OK / False = KO </returns>
        public bool DescargarArchivosFtp(string rutaFtp, string rutaDescarga, string extension, string extensionAuxiliar)
        {
            #region DescargaArchivos de un FTP.
            try
            {
                Console.WriteLine("entro DescargarArchivosFtp");
                IEnumerable<SftpFile> archivosCarpetaDelta = Conexion.ListDirectory(rutaFtp);
                foreach (var item in archivosCarpetaDelta)
                {
                    Console.WriteLine("nombresCarpetas");
                    Console.WriteLine(item);
                }
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

        /// <summary>
        /// Metodo para descargar archivos del FTP
        /// </summary>
        /// <param name="rutaFtpDelta"></param>
        /// <param name="rutaDescarga"></param>
        /// <returns>Booleano True = OK / False = KO </returns>
        public bool DescargarArchivosFtpOrden(string rutaFtpDelta, string rutaDescarga)
        {
            #region DescargaArchivos de un FTP.
            try
            {
                IEnumerable<SftpFile> archivosCarpetaDelta = Conexion.ListDirectory(rutaFtpDelta);
                foreach (SftpFile archvoFtp in archivosCarpetaDelta)
                {
                    if (!archvoFtp.Name.Contains("guias")) continue;
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

        /// <summary>
        /// Metodo para crear carpeta en el FTP de Delta
        /// </summary>
        /// <param name="nombreCarpeta">Nombre de la carpeta</param>
        /// <returns>Booleano True = OK / False = KO </returns>
        public bool CrearcarpetaFtpDelta(string nombreCarpeta)
        {
            #region Crea carpetas en DELTA
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

        /// <summary>
        /// Metodo para cragar archivo a FTP de Delta
        /// </summary>
        /// <param name="rutaOriginal">Ruta origen del archivo</param>
        /// <param name="rutaFtp">Ruta del FTP</param>
        /// <returns>Booleano True = OK / False = KO </returns>
        public bool CargarArchivoFtpDelta(string rutaOriginal, string rutaFtp)
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

        /// <summary>
        /// Metodo para descargar archivos del FTP de Delta
        /// </summary>
        /// <param name="rutaFtpDelta">Ruta del FTP</param>
        /// <param name="rutaDescarga">Ruta de descarga</param>
        /// <returns>Booleano True = OK / False = KO </returns>
        public bool DescargarArchivosFtpDelta(string rutaFtpDelta, string rutaDescarga)
        {
            #region DescargaArchivos de un FTP.
            try
            {
                IEnumerable<SftpFile> archivosCarpetaDelta = Conexion.ListDirectory(rutaFtpDelta);
                foreach (SftpFile archvoFtp in archivosCarpetaDelta)
                {
                    if (!archvoFtp.Name.ToLower().Contains("guias") && !archvoFtp.Name.ToLower().Contains("ordenamiento"))
                    {
                        continue;
                    }

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
        
        /// <summary>
        /// Metodo para liberar Memoria
        /// </summary>        
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.        
        /// <summary>
        /// Metodo para liberar Memoria
        /// </summary>
        /// <param name="disposing">Bandera para limpiar variables</param>
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
