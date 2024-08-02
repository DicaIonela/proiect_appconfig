using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NivelStocareDate;
using System.Configuration;
using LibrarieClase;
using System.Windows.Forms;
using InterfataUtilizator;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
namespace Proiect_practicaDI
{
    internal class Program
        {
        [STAThread]
        static void Main(string[] args)
        {    
            Metode.StartCommandPromptMode(); /*Continua cu modul Command Prompt*/
        }
    }
}
