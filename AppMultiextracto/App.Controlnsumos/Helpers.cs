using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;

namespace App.Controlnsumos
{
    public static class Helpers
    {
        public static string RutaProceso { get; set; }
        public static string RutaBaseMaestraFisico { get; set; }

        /// <summary>
        /// Metodo para convertir Excel (.xlsx - .xls) en archivo plano
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<string> ConvertirExcel(string archivo)
        {
            #region ConvertirExcel
            DataSet result = new DataSet();
            try
            {
                #region Leer Excel
                if (archivo.EndsWith(".xlsx"))
                {
                    //Reading from a binary Excel file(format; .xlsx)
                    FileStream stream = File.Open(archivo, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    result = excelReader.AsDataSet();
                    excelReader.Close();
                }

                if (archivo.EndsWith(".xls"))
                {
                    //Reading from a binary Excel file('97-2003 format; .xls)
                    FileStream stream = File.Open(archivo, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    result = excelReader.AsDataSet();
                    excelReader.Close();
                }

                #endregion

                #region Pasar Excel a Plano
                List<string> datosExcel = new List<string>();
                int numHoja = Convert.ToInt32(0);
                string a = "";

                for (int j = 0; j < result.Tables[numHoja].Rows.Count; j++)
                {
                    for (int i = 0; i < result.Tables[numHoja].Columns.Count; i++)
                    {
                        if (result.Tables[numHoja].Rows[j][i].ToString() != "")
                        {
                            a += result.Tables[numHoja].Rows[j][i].ToString().Replace("\n", " ");
                        }
                        else
                        {
                            a += " ";
                        }

                        if (i < (result.Tables[numHoja].Columns.Count + 1))
                        {
                            a += "|";
                        }
                    }

                    datosExcel.Add(a);
                    a = "";

                }
                #endregion

                return datosExcel;
            }
            catch (Exception mens)
            {
                throw new Exception("Error: " + mens.Message);
            }
            #endregion
        }


        public static Int64 GetTamañoArchivo(string pRutaArchivo)
        {
            Int64 tamañoArchivo = 0;

            FileInfo fileInfo = new FileInfo(pRutaArchivo);

            if (fileInfo.Exists)
            {
                tamañoArchivo = fileInfo.Length;
            }

            return tamañoArchivo;
        }
        /// <summary>
        /// Metodo para crear carpeta 
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns>Ruta de la carpeta creada</returns>
        /// <exception cref="Exception"></exception>
        public static string CrearCarpeta(string ruta)
        {
            #region CrearCarpeta
            try
            {
                string carpeta = ruta;

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                return carpeta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);                
            }
            #endregion

        }
    }
}
