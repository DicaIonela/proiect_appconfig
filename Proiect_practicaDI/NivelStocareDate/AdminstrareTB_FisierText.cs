using LibrarieClase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace NivelStocareDate
{
    public class AdminstrareTB_FisierText
    {
        private const int NR_MAX = 50;
        private string numeFisier;
        public AdminstrareTB_FisierText(string numeFisier)/*CONSTRUCTOR LINII FISIER*/
        {
            this.numeFisier = numeFisier;
            Stream streamFisier = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisier.Close();
            

        }
        public void AddTB(TestBench testBench)/*ADAUGARE TB IN FISIER */
        {
            using (StreamWriter streamwriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamwriterFisierText.WriteLine(testBench.Conversie_PentruFisier());
            }
        }
        public bool ExistaTB(string tb, string adresa)/*VERIFICA EXISTENTA TBULUI IN FISIER PENTRU A EVITA SCRIEREA UNEI COPII*/
        {
            if (!File.Exists(numeFisier))
                return false;
            using (StreamReader streamReader = new StreamReader(numeFisier))
            {
                string linie;
                while ((linie = streamReader.ReadLine()) != null)
                {
                    TestBench testBench = new TestBench(linie);
                    if (testBench.Tb == tb || testBench.AdresaMAC == adresa)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public TestBench[] GetTB(out int nrTB)/*STOCHEAZA TB-URILE DIN FISIER INTR-UN TABLOU DE OBIECTE*/
        {
            TestBench[] testBenchuri = new TestBench[NR_MAX];
            using (StreamReader streamrdr = new StreamReader(numeFisier))
            {
                string linieFisier;
                nrTB = 0;
                while ((linieFisier = streamrdr.ReadLine()) != null)
                {
                    if (linieFisier != "TestBench; Adresa MAC")
                    {
                        testBenchuri[nrTB++] = new TestBench(linieFisier);
                    }
                }
                Array.Resize(ref testBenchuri, nrTB);
            }
            return testBenchuri;
        }
        public TestBench[] CautaTB(string criteriu)
        {
            int nrTB = 0;
            TestBench[] testBenchuri = GetTB(out nrTB);
            List<TestBench> tbgasite = new List<TestBench>();/*se creeaza o lista pentru TB-URILE gasiti*/
            foreach (TestBench testBench in testBenchuri)/*se parcurge tabloul de obiecte*/
            {
                if (testBench.Tb.Trim().Equals(criteriu.Trim(), StringComparison.OrdinalIgnoreCase))/*se verifica daca numele contine caracterele introduse*/
                {
                   tbgasite.Add(testBench);/*daca numele a indeplinit conditia, se va adauga obiectul la lista de TB-URI gasite*/
                }
            }
            return tbgasite.ToArray();
        }
        public void StergeTB(string tb)
        {
            if (!File.Exists(numeFisier))
            {
                Console.WriteLine("Fișierul nu există.");
                return;
            }
            var linii = new List<string>(File.ReadAllLines(numeFisier));/*Citeste toate liniile din fisier*/
            var liniiNou = new List<string>();/*Cream o lista pentru liniile care nu contin tbul de sters*/
            foreach (var linie in linii)/*se parcurge lista*/
            {
                var testBech = new TestBench(linie);
                if (!testBech.Tb.Equals(tb, StringComparison.OrdinalIgnoreCase))/*Verificam daca linia contine numele tbului care trebuie sters*/
                {
                    liniiNou.Add(linie);/*daca numele nu indeplineste criteriul, se va adauga in lista noua*/
                }
            }
            File.WriteAllLines(numeFisier, liniiNou);/*se pun inapoi in fisier liniile care nu au fost sterse*/
            Console.WriteLine("TB-ul '{0}' a fost sters, daca a fost gasit.", tb);
        }
        public void UpdateTB(string vechiTB, TestBench testBench)
        {
            TestBench[] testBenchuri = GetTB(out int nrTB);
            bool tbFound = false;
            for (int i = 0; i < nrTB; i++)
            {
                if (string.Equals(testBenchuri[i].Tb.Trim(), vechiTB.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    testBenchuri[i] = testBench;
                    tbFound = true;
                    break;
                }
            }

            if (!tbFound)
            {
                Console.WriteLine($"TB-ul {vechiTB} nu a fost găsit.");
                return;
            }

            using (StreamWriter writer = new StreamWriter(numeFisier))
            {
                foreach (TestBench testb in testBenchuri)
                {
                    writer.WriteLine(testb.Conversie_PentruFisier());
                }
            }
            Console.WriteLine("TB-ul a fost actualizat cu succes.");
        }
    }
}