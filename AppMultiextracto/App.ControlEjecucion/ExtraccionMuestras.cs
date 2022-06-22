using System;
using System.Collections.Generic;
using AppVariables = App.Variables.Variables;
using App.Controlnsumos;
using System.IO;
using App.Controlnsumos;

namespace App.ControlEjecucion
{
    /// <summary>
    /// Clase para Extraccion de Muestras
    /// </summary>
    public class ExtraccionMuestras
    {
        private bool _disposed = false;
        List<string> muestrasNoCruzadas = new List<string>();
        private string RutaSalidaProcesoMuestras = string.Empty;
        private string RutaSalidaProcesoMuestrasVirtual = string.Empty;
        private string RutaSalidaProcesoMuestrasFisica = string.Empty;

        /// <summary>
        /// Constructor Extraccion Muestras
        /// </summary>
        public ExtraccionMuestras()
        {
            #region ExtraccionMuestras
            ExraerMuetras();
            RutaSalidaProcesoMuestras = Directory.CreateDirectory($@"{Helpers.RutaProceso}\Muestras").FullName;
            RutaSalidaProcesoMuestrasVirtual = Directory.CreateDirectory($@"{RutaSalidaProcesoMuestras}\MuestrasVirtuales").FullName;
            RutaSalidaProcesoMuestrasFisica = Directory.CreateDirectory($@"{RutaSalidaProcesoMuestras}\MuestrasFisicas").FullName;
            GenerarArchivosMuestras();
            GenerarReporteMuestrasNoCruzadas(); 
            #endregion
        }

        /// <summary>
        /// Metodo que extrae las muestras del diccionario completo
        /// </summary>
        private void ExraerMuetras()
        {
            #region ExraerMuetras
            foreach (var muestras in AppVariables.InsumoMuestras)
            {
                if (AppVariables.DiccionarioExtractosFormateados.ContainsKey(muestras.Key))
                {
                    AppVariables.DiccionarioExtractosMuestras.Add(muestras.Key, AppVariables.DiccionarioExtractosFormateados[muestras.Key]);
                }
                else
                {
                    muestrasNoCruzadas.Add(muestras.Key);
                }

            } 
            #endregion

        }

        /// <summary>
        /// Genera Archivo SAL de Muestras del Proceso
        /// </summary>
        private void GenerarArchivosMuestras()
        {
            #region GenerarArchivosMuestras
            foreach (var extractoCedula in AppVariables.DiccionarioExtractosMuestras)
            {
                foreach (var tipoExtracto in extractoCedula.Value)
                {
                    if (tipoExtracto.Key == "Virtual")
                    {
                        foreach (var paqueteExtracto in tipoExtracto.Value)
                        {
                            Helpers.EscribirEnArchivo($@"{RutaSalidaProcesoMuestrasVirtual}\{Variables.Variables.Orden}_Muestras_{tipoExtracto.Key}.sal", paqueteExtracto.Value);
                        }
                    }
                    else if (tipoExtracto.Key == "Fisico")
                    {
                        foreach (var paqueteExtracto in tipoExtracto.Value)
                        {
                            Helpers.EscribirEnArchivo($@"{RutaSalidaProcesoMuestrasFisica}\{Variables.Variables.Orden}_Muestras_{tipoExtracto.Key}.sal", paqueteExtracto.Value);
                        }
                    }
                }
            } 
            #endregion
        }

        /// <summary>
        /// Genera Archivo CSV de lista de muestras No Cruzadas
        /// </summary>
        private void GenerarReporteMuestrasNoCruzadas()
        {
            #region GenerarReporteMuestrasNoCruzadas
            if (muestrasNoCruzadas.Count > 0)
            {
                Helpers.EscribirEnArchivo($@"{RutaSalidaProcesoMuestras}\{Variables.Variables.Orden}_MuestrasNoCruzadas.csv", muestrasNoCruzadas);
            } 
            #endregion
        }

        public void Dispose()
        {
            #region Dispose
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this); 
            #endregion
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            #region Dispose
            if (_disposed)
                return;

            if (disposing)
            {
                Variables.Variables.DiccionarioExtractosMuestras.Clear();
                Variables.Variables.InsumoMuestras.Clear();
                muestrasNoCruzadas.Clear();
            }

            // Free any unmanaged objects here.
            _disposed = true; 
            #endregion
        }
    }

   
}
