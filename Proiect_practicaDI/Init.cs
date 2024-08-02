using System;
using NivelStocareDate;
using LibrarieClase;
namespace Proiect_practicaDI
{
    public static class Init
    {
        public static void Initialize(out Administrare_FisierText admin, out Utilizator utilizatornou)
        {
            // Inițializarea calea către fișier
            string numeFisier = System.Configuration.ConfigurationManager.AppSettings["NumeFisier"];
            string locatieFisierSolutie = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string caleCompletaFisier = System.IO.Path.Combine(locatieFisierSolutie, "Proiect_practicaDI", numeFisier);
            // Inițializare obiecte
            admin = new Administrare_FisierText(caleCompletaFisier);
            utilizatornou = new Utilizator();
        }
        public static void InitializeTB(out AdminstrareTB_FisierText adminTB, out TestBench testBenchnou)
        {
            // Inițializarea calea către fișier
            string numeFisierTB = System.Configuration.ConfigurationManager.AppSettings["NumeFisierTB"];
            string locatieFisierSolutie = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string caleCompletaFisier = System.IO.Path.Combine(locatieFisierSolutie, "Proiect_practicaDI", numeFisierTB);
            // Inițializare obiecte
            adminTB = new AdminstrareTB_FisierText(caleCompletaFisier);
            testBenchnou = new TestBench();
        }
    }
}
