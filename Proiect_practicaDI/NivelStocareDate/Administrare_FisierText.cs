using LibrarieClase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace NivelStocareDate
{
    public class Administrare_FisierText
    {
        private const int NR_MAX = 50;
        private string numeFisier;
        public Administrare_FisierText(string numeFisier)/*CONSTRUCTOR LINII FISIER*/
        {
            this.numeFisier = numeFisier;
            Stream streamFisier = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisier.Close();
        }
        public void AddUtilizator(Utilizator utilizator)/*ADAUGARE UTILIZATOR IN FISIER */
        {
            using (StreamWriter streamwriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamwriterFisierText.WriteLine(utilizator.Conversie_PentruFisier());
            }
        }
        public bool UtilizatorExista(string nume, string numar)/*VERIFICA EXISTENTA UTILIZATORULUI IN FISIER PENTRU A EVITA SCRIEREA UNEI COPII*/
        {
            if (!File.Exists(numeFisier))
                return false;
            using (StreamReader streamReader = new StreamReader(numeFisier))
            {
                string linie;
                while ((linie = streamReader.ReadLine()) != null)
                {
                    Utilizator utilizator = new Utilizator(linie);
                    if (utilizator.Nume == nume && utilizator.Numar == numar)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Utilizator[] GetUtilizatori(out int nrUtilizatori)/*STOCHEAZA UTILIZATORII DIN FISIER INTR-UN TABLOU DE OBIECTE*/
        {
            Utilizator[] utilizatori = new Utilizator[NR_MAX];
            using (StreamReader streamrdr = new StreamReader(numeFisier))
            {
                string linieFisier;
                nrUtilizatori = 0;
                while ((linieFisier = streamrdr.ReadLine()) != null)
                {
                    if (linieFisier != "Nume;Numar;Adresa MAC")
                    {
                        utilizatori[nrUtilizatori++] = new Utilizator(linieFisier);
                    }
                }
                Array.Resize(ref utilizatori, nrUtilizatori);
            }
            return utilizatori;
        }
        public Utilizator[] Getutilizatori()
        {
            NameValueCollection utilizatoriSection = (NameValueCollection)ConfigurationManager.GetSection("utilizatoriSection");
            Utilizator[] utilizatori = new Utilizator[utilizatoriSection.Count];
            int index = 0;
            foreach (string key in utilizatoriSection)
            {
                string[] values = utilizatoriSection[key].Split(';');
                utilizatori[index++] = new Utilizator
                {
                    Nume = key,
                    Numar = values[0],
                    AdresaMAC = values[1]
                };
            }
            return utilizatori;
        }
        public Utilizator[] CautaUtilizator(string criteriu)
        {
            int nrUtilizatori = 0;
            Utilizator[] utilizatori = GetUtilizatori(out nrUtilizatori);
            List<Utilizator> utilizatorigasiti = new List<Utilizator>();/*se creeaza o lista pentru utilizatorii gasiti*/
            foreach (Utilizator utilizator in utilizatori)/*se parcurge tabloul de obiecte*/
            {
                if (utilizator != null && (utilizator.Nume.Contains(criteriu) || (utilizator.Numar.Contains(criteriu))))/*se verifica daca numele contine caracterele introduse/numarul*/
                {
                    utilizatorigasiti.Add(utilizator);/*daca numele a indeplinit conditia, se va adauga obiectul la lista de utilizatori gasiti*/
                }
            }
            return utilizatorigasiti.ToArray();
        }
        public void StergeUtilizator(string nume)
        {
            if (!File.Exists(numeFisier))
            {
                Console.WriteLine("Fișierul nu există.");
                return;
            }
            var linii = new List<string>(File.ReadAllLines(numeFisier));/*Citeste toate liniile din fisier*/
            var liniiNou = new List<string>();/*Cream o lista pentru liniile care nu contin utilizatorul de sters*/
            foreach (var linie in linii)/*se parcurge lista*/
            {
                var utilizator = new Utilizator(linie);
                if (!utilizator.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase))/*Verificam daca linia contine numele utilizatorului care trebuie sters*/
                {
                    liniiNou.Add(linie);/*daca numele nu indeplineste criteriul, se va adauga in lista noua*/
                }
            }
            File.WriteAllLines(numeFisier, liniiNou);/*se pun inapoi in fisier liniile care nu au fost sterse*/
            Console.WriteLine("Utilizatorul cu numele '{0}' a fost sters, daca a fost gasit.", nume);
        }
        public void UpdateUtilizator(string vechiNume, Utilizator utilizator)
        {
            Utilizator[] utilizatori = GetUtilizatori(out int nrUtilizatori);
            bool userFound = false;
            for (int i = 0; i < nrUtilizatori; i++)
            {
                if (string.Equals(utilizatori[i].Nume.Trim(), vechiNume.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    utilizatori[i] = utilizator;
                    userFound = true;
                    break;
                }
            }

            if (!userFound)
            {
                Console.WriteLine($"Utilizatorul cu numele {vechiNume} nu a fost găsit.");
                return;
            }

            using (StreamWriter writer = new StreamWriter(numeFisier))
            {
                foreach (Utilizator user in utilizatori)
                {
                    writer.WriteLine(user.Conversie_PentruFisier());
                }
            }
            Console.WriteLine("Utilizatorul a fost actualizat cu succes.");
        }
    }
}
