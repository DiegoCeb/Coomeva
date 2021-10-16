using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using App.ControlProcesos;

namespace AppMultiextracto
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (GestionProcesos objProcesos = new GestionProcesos())
            {
                objProcesos.Menu();
            }

            Console.ReadKey();
        }
    }
}
