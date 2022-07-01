using System;
using App.Controlnsumos;
using App.ControlProcesos;

namespace AppMultiextracto
{
    /// <summary>
    /// Clase Program
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            #region Main
            using (GestionProcesos objProcesos = new GestionProcesos())
            {                
                Helpers.EscribirLogUsuario(Environment.UserName);
                objProcesos.Menu();
            }

            Console.ReadKey();
            #endregion
        }
    }
}
