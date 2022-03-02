using App.ControlFtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL_Utilidades;
using App.Controlnsumos;
using System.IO;

namespace App.ControlEjecucion
{
    public class Procesamiento : Variables.Variables, IProcess, IDisposable
    {
        // Flag: Has Dispose already been called?
        private bool _disposed = false;
        Dictionary<string, string> DatosVerificacionArchivos;

        public bool DescargaArchivos()
        {
            try
            {
                using (ClaseFtp objFtp = new ClaseFtp(Utilidades.LeerAppConfig("FtpDireccionCoomeva"),
                    Convert.ToInt16(Utilidades.LeerAppConfig("FtpPuertoCoomeva")), Utilidades.LeerAppConfig("FtpUsuarioCoomeva"),
                    Utilidades.LeerAppConfig("FtpClaveCoomeva")))
                {
                    objFtp.ConectarFtp();

                    objFtp.DescargarArchivosFtp(".", Utilidades.LeerAppConfig("RutaEntrada"), ".gpg", ".pgp");

                    objFtp.DesconectarFtp();
                }

                return true;
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                Utilidades.EscribirLog(MensajeError, Utilidades.LeerAppConfig("RutaLog"));
                return false;
            }

        }

        public bool VerificacionArchivosEntrada()
        {
            string resultado = "1";

            DatosVerificacionArchivos = Insumos.CargarNombresArchivos();

            foreach (var archivo in Directory.GetFiles(Utilidades.LeerAppConfig("RutaEntrada")))
            {
                foreach (var insumo in DatosVerificacionArchivos.Keys)
                {
                    if (archivo.Contains(insumo))
                    {
                        resultado = "0";
                        break;
                    }
                }

                if (resultado == "1")
                {
                    MensajeError = $"El siguiente archivo {Path.GetFileName(archivo)} no se reconoce dentro de los nombres configurados para el proceso.";
                    Utilidades.EscribirLog(MensajeError, Utilidades.LeerAppConfig("RutaLog"));
                    return false;
                }

            }

            return true;
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
                DatosVerificacionArchivos.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }
    }
}
