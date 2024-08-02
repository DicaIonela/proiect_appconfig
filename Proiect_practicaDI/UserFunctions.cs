using LibrarieClase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
namespace Proiect_practicaDI
{
    public class UserFunctions
    {
        public static void AfisareUtilizatori(Utilizator[] utilizatori, int nrUtilizatori)
        {
            if (utilizatori.Length > 0)
            {
                //Console.WriteLine("Utilizatorii salvati in fisier gasiti sunt:");
                for (int contor = 0; contor < nrUtilizatori; contor++)/*SE PARCURGE TABLOUL DE OBIECTE SI SE AFISEAZA INFORMATIILE IN FORMATUL CORESPUNZATOR*/
                {
                    string infoUseri = utilizatori[contor].Info();
                    Console.WriteLine(infoUseri);
                }
            }
            else
                Console.WriteLine("Nu au fost gasiti utilizatori.");
        }
        public static void AfisareTBURI(TestBench[] testBenchuri, int nrtestBenchuri)
        {
            if (testBenchuri.Length > 0)
            {
                //Console.WriteLine("Utilizatorii salvati in fisier gasiti sunt:");
                for (int contor = 0; contor < nrtestBenchuri; contor++)/*SE PARCURGE TABLOUL DE OBIECTE SI SE AFISEAZA INFORMATIILE IN FORMATUL CORESPUNZATOR*/
                {
                    string infoTB = testBenchuri[contor].Info();
                    Console.WriteLine(infoTB);
                }
            }
            else
                Console.WriteLine("Nu au fost gasite TB-uri.");
        }
        public static string ValidareSiCorectareNumar(string numar)
        {
            if (numar.StartsWith("+40"))
            {
                if (numar.Length == 13 && numar.Substring(3).All(char.IsDigit))/*Verificare daca numarul are exact 13 caractere si sunt doar cifre dupa +40*/
                {
                    return "0040" + numar.Substring(3);
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
            if (numar.StartsWith("0040"))
            {
                if (numar.Length == 13 && numar.Substring(3).All(char.IsDigit))/*Verificare daca numarul are exact 13 caractere si sunt doar cifre dupa +40*/
                {
                    return numar;
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
            else
            {
                if (numar.Length == 10 && numar.All(char.IsDigit) && numar.StartsWith("0"))/*Verificare daca numarul are exact 10 caractere si sunt doar cifre*/
                {
                    return "004" + numar; /*Adaugam prefixul +4 -> 0 fiind deja inclus*/
                }
                else
                {
                    return null; /*Invalid*/
                }
            }
        }
        public static string ValidareSiFormatareAdresaMac(string adresamac)
        {
            var cleanAddress = new string(adresamac
                .Where(c => "0123456789ABCDEF".Contains(char.ToUpper(c)))
                .ToArray());/*Elimina toate caracterele non-hexadecimale si normalizeaza*/
            if (cleanAddress.Length == 12)/*Verificare daca are exact 12 caractere (6 octeti)*/
            {
                return string.Join("-", Enumerable.Range(0, cleanAddress.Length / 2)
                    .Select(i => cleanAddress.Substring(i * 2, 2)));/*Formateaza in formatul 00-11-22-33-44-55*/
            }
            else
            {
                return null; /*Invalid*/
            }
        }
        public static Utilizator CitireUtilizatorTastatura()
        {
            Console.WriteLine("Introduceti datele utilizatorului:");
            Console.WriteLine("Nume:");
            string nume = Console.ReadLine();
            string numarcitit;
            do
            {
                Console.WriteLine("Numar:");
                numarcitit = Console.ReadLine();
                numarcitit = ValidareSiCorectareNumar(numarcitit);
                if (numarcitit == null)
                {
                    Console.WriteLine("Numarul de telefon introdus nu este valid. Te rugam sa incerci din nou.");
                }
            } while (numarcitit == null);
            string adresamac;
            do
            {
                Console.WriteLine("Adresa MAC (format: 00-11-22-33-44-55):");
                adresamac = Console.ReadLine();
                adresamac = ValidareSiFormatareAdresaMac(adresamac);
                if (adresamac == null)
                {
                    Console.WriteLine("Adresa MAC introdusa nu este valida. Te rugam sa incerci din nou.");
                }
            } while (adresamac == null);
            Utilizator utilizator = new Utilizator(nume, numarcitit, adresamac);
            return utilizator;
        }
        public static TestBench CitireTBTastatura()
        {
            Console.WriteLine("Introduceti datele TB-ului:");
            Console.WriteLine("Nume:");
            string Tb = Console.ReadLine();
            string numarcitit;
            string adresamac;
            do
            {
                Console.WriteLine("Adresa MAC (format: 00-11-22-33-44-55):");
                adresamac = Console.ReadLine();
                adresamac = ValidareSiFormatareAdresaMac(adresamac);
                if (adresamac == null)
                {
                    Console.WriteLine("Adresa MAC introdusa nu este valida. Te rugam sa incerci din nou.");
                }
            } while (adresamac == null);
            TestBench testBench = new TestBench(Tb, adresamac);
            return testBench;
        }
        public static string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();/*Obtine lista de placi de retea*/
            foreach (var networkInterface in networkInterfaces)/*Cautam prima placa de retea activa si obtinem adresa MAC*/
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)/*Verifica daca placa de retea nu este de tip Loopback si este activs*/
                {
                    var macAddress = networkInterface.GetPhysicalAddress();
                    if (macAddress != null)
                    {
                        return string.Join("-", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));/*Formateaza adresa MAC intr-un sir de caractere hexazecimale*/
                    }
                }
            }
            return "Adresa MAC nu a putut fi gasita";
        }
    }
}
