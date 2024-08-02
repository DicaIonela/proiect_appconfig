using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieClase
{
    public class TestBench
    {
        private const char SEPARATOR_FISIER = ';';
        public string Tb { get; set; }
        public string AdresaMAC { get; set; }
        private const int TB = 0;
        private const int ADRESAMAC = 1;
        public TestBench()/*CONSTRUCTOR FARA PARAMETRI*/
        {
            Tb = AdresaMAC = string.Empty;
        }
        public TestBench(string Tb, string AdresaMAC)/*CONSTRUCTOR CU PARAMETRI*/
        {
            this.Tb = Tb;
            this.AdresaMAC = AdresaMAC;
        }
        public TestBench(string linieFisier) /*CONSTRUCTOR PENTRU LINIILE FISIERULUI*/
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_FISIER);
            this.Tb = dateFisier[TB];
            this.AdresaMAC = dateFisier[ADRESAMAC];
        }
        public string Conversie_PentruFisier()/*CONVERTESTE INFORMATIILE UTILIZATORULUI DIN OBIECT IN STRING IN FORMATUL CORESPUNZATOR PENTRU FISIERUL CSV; PENTRU SALVARE IN FISIER*/
        {
            string UtilizatorInFisier = string.Format("{1}{0}{2}",
                SEPARATOR_FISIER,
                (Tb ?? " NECUNOSCUT "),
                (AdresaMAC ?? "NECUNOSCUT"));
            return UtilizatorInFisier;
        }
        public string Info()/*FORMAREA UNUI SIR DE CARACTERE CORESPUNZATOR PENTRU AFISAREA IN CONSOLA A INFORMATIILOR UTILIZATORULUI*/
        {
            string info = $"Test Bench:\nNume:{Tb ?? " NECUNOSCUT "} \nAdresa MAC: {AdresaMAC ?? " NECUNOSCUT "}";
            return info;
        }
    }
}
