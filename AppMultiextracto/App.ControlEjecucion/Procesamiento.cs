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

        /// <summary>
        /// Carga los archivos de3 manera global y temporal.
        /// </summary>
        /// <typeparam name="TEntidad">Entidad que se va a cargar</typeparam>
        /// <param name="pArchivo">ruta del archivo</param>
        /// <param name="pEntidadArchivo">Entidad del proceso</param>
        /// <returns>True o False segun proceso.</returns>
        public bool CargueArchivosGlobal<TEntidad>(string pArchivo, TEntidad pEntidadArchivo)
        {
            #region CargueArchivosGlobal
            var newObject = (Type)(object)pEntidadArchivo;

            if (newObject.Name == "Etiquetas" && Path.GetFileNameWithoutExtension(pArchivo).ToUpper().Contains("I"))
            {
                Helpers.RutaBaseMaestraFisico = GenerarBaseMaestra(pArchivo);
            }

            object invoke = newObject.InvokeMember(newObject.Name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance, null,
                newObject, new object[] { pArchivo });

            return true; 
            #endregion
        }

        public string GenerarBaseMaestra(string pArchivo)
        {
            #region GenerarBaseMaestra
            string rutaResult = $"{Helpers.RutaProceso}\\BaseMaestra.csv";
            List<string> datosBaseMaestra = new List<string>();

            //Encabezado
            datosBaseMaestra.Add($"cedula;cuenta;nombres;direccion;ciudad;dpto");

            foreach (var linea in Utilidades.LeerArchivoPlanoANSI(pArchivo))
            {
                string cedula = $"{linea.Substring(143, 6)}{linea.Substring(151, 5)}".TrimStart('0');
                string cuenta = $"{linea.Substring(143, 6)}{linea.Substring(151, 5)}".TrimStart('0');
                string nombres = linea.Substring(0, 31).Trim();
                string direccion = linea.Substring(31, 60).Trim();
                string segmentoCiudadDpto = linea.Substring(92, 37).Trim();
                string ciudad = segmentoCiudadDpto.LastIndexOf('(') != -1 ? segmentoCiudadDpto.Substring(0, segmentoCiudadDpto.LastIndexOf('(')).Trim() : segmentoCiudadDpto;
                string dpto = segmentoCiudadDpto.LastIndexOf('(') != -1 ? segmentoCiudadDpto.Substring(segmentoCiudadDpto.LastIndexOf('(')).Trim().Replace("(","").Replace(")", "") : segmentoCiudadDpto;

                datosBaseMaestra.Add($"{cedula};{cuenta};{nombres};{direccion};{ciudad};{dpto}");
            }

            File.WriteAllLines(rutaResult, datosBaseMaestra);

            datosBaseMaestra.Clear();

            return rutaResult; 
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
                DatosVerificacionArchivos.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

        
    }
}
