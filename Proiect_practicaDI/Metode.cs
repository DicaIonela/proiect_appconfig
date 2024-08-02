using LibrarieClase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using NivelStocareDate;
using System.Runtime.InteropServices;
using InterfataUtilizator;
//using System.Windows.Forms;
using System.Data.SqlTypes;
using System.IO.Ports;
//using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Timers;
using System.Globalization;
using System.Xml.Linq;
namespace Proiect_practicaDI
{
    public static class Metode
    {
        /*INITIALIZARI PENTRU A PUTEA ASCUNDE CONSOLA LA RULARE*/
        //[DllImport("kernel32.dll")]/*Se importa functii pentru a ascunde/afisa consola*/
        //static extern IntPtr GetConsoleWindow();
        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;
        static SerialPort serialPort;
        static StringBuilder dataBuffer = new StringBuilder();
        private static Timer messagePollingTimer;
        static Timer messageTimer;
        [STAThread]
        public static void Start()
        {
            //IntPtr handle = GetConsoleWindow();/*Obtine handle-ul ferestrei consolei*/
            //ShowWindow(handle, SW_HIDE);/*Ascunde consola*/
            ///*Setam aplicatia pentru a folosi Windows Forms*/
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ///*Afisam fereastra de alegere*/
            //DialogResult result = MessageBox.Show(
            //    "Doriti sa intrati in interfata grafica a aplicatiei?\n\n" +
            //    "*Selectand butonul No alegeti optiunea de a folosi aplicatia in consola.\n",
            //    "Selectați modul de utilizare",
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Question);
            //if (result == DialogResult.Yes)
            //{
            //    Application.Run(new InterfataGrafica());/*Deschide interfata grafica*/
            //}
            //else
            //{
            //    ShowWindow(handle, SW_SHOW);/*reafiseaza consola*/
                Metode.StartCommandPromptMode(); /*Continua cu modul Command Prompt*/
            //}
        }
        public static void Meniu()
        {
            Console.WriteLine("----MENIU----");
            Console.WriteLine("C. Citire utilizator.");
            Console.WriteLine("S. Salvare utilizator.");
            Console.WriteLine("A. Afisare utilizatori din fisier.");
            Console.WriteLine("L. Cautare utilizator dupa nume.");
            Console.WriteLine("M. Afiseaza adresa MAC a acestui PC.");
            Console.WriteLine("E. Sterge un utilizator din fisier.");
            Console.WriteLine("N. Modifica un utilizator deja existent.");
        }
        public static void StartCommandPromptMode()
        {
            Init.Initialize(out Administrare_FisierText admin, out Utilizator utilizatornou); /*initializari*/
            Init.InitializeTB(out AdminstrareTB_FisierText adminTB, out TestBench testBenchnou);
            ListenToSerialPort("COM4", 115200);
            Utilizator[] utilizatori=admin.Getutilizatori();
            UserFunctions.AfisareUtilizatori(utilizatori, utilizatori.Length);
            string optiune;
            //testBenchnou = UserFunctions.CitireTBTastatura();
            //adminTB.AddTB(testBenchnou);
            //Console.WriteLine("TestBench adaugat.");
            //TestBench[] testbenchuri = adminTB.GetTB(out int nrTB);
            //UserFunctions.AfisareTBURI(testbenchuri, nrTB);
            do
            {
                optiune = Console.ReadLine();
            } while (optiune != "X");
            //Meniu();/*text meniu*/
            //do
            //{
            //    Console.WriteLine("\nIntrodu optiunea dorita:");
            //    string optiune1 = Console.ReadLine();
            //    switch (optiune1)
            //    {
            //        case "C":
            //            utilizatornou = UserFunctions.CitireUtilizatorTastatura();
            //            break;
            //        case "S":
            //            /* Verificare daca a fost introdus un utilizator nou */
            //            if (utilizatornou.Nume != string.Empty)
            //            {
            //                admin.AddUtilizator(utilizatornou);/*daca a fost introdus un utilizator nou, se adauga in fisier*/
            //                Console.WriteLine("Utilizatorul a fost adaugat cu succes.");
            //            }
            //            else
            //            {
            //                Console.WriteLine("Salvare nereusita. Nu ati introdus niciun utilizator nou.");
            //            }
            //            break;
            //        case "A":
            //            Utilizator[] utilizatori = admin.GetUtilizatori(out int nrUtilizatori);/*SE CREEAZA UN TABLOU DE OBIECTE*/
            //            UserFunctions.AfisareUtilizatori(utilizatori, nrUtilizatori);
            //            break;
            //        case "L":
            //            Console.WriteLine("Introduceti criteriul de cautare:");
            //            string criteriu = Console.ReadLine();
            //            Utilizator[] utilizatoriGasiti = admin.CautaUtilizator(criteriu);
            //            if (utilizatoriGasiti.Length > 0)
            //            {
            //                UserFunctions.AfisareUtilizatori(utilizatoriGasiti, utilizatoriGasiti.Length);
            //            }
            //            else
            //            {
            //                Console.WriteLine("Nu s-au găsit utilizatori care să corespundă criteriului.");
            //            }
            //            break;
            //        case "M":
            //            string adresam = UserFunctions.GetMacAddress();
            //            Console.WriteLine("Adresa MAC a calculatorului este: " + adresam);
            //            break;
            //        case "E":
            //            Console.WriteLine("Introdu numele utilizatorului de sters:");
            //            string numedesters = Console.ReadLine();
            //            admin.StergeUtilizator(numedesters);
            //            break;
            //        case "N":
            //            Console.WriteLine("Introduceti numele complet al utilizatorului de modificat.");
            //            string username = Console.ReadLine();
            //            Console.WriteLine("Introduceti noul nume al utilizatorului:");
            //            string nume = Console.ReadLine();
            //            Console.WriteLine("Introduceti noul numar de telefon:");
            //            string numar = Console.ReadLine();
            //            Console.WriteLine("Introduceti noua adresa MAC:");
            //            string adresa = Console.ReadLine();
            //            Utilizator utilizator = new Utilizator(nume, numar, adresa);
            //            admin.UpdateUtilizator(username, utilizator);
            //            break;
            //    }
            //} while (true);
        }
        static void ListenToSerialPort(string portName, int baudRate)
        {
            serialPort = new SerialPort(portName);
            /*Setarile portului serial cu valori implicite pentru parametrii care nu sunt specificati*/
            serialPort.BaudRate = baudRate;
            serialPort.Parity = Parity.None; /* Fara paritate*/
            serialPort.DataBits = 8; /*8 biti de date*/
            serialPort.StopBits = StopBits.One; /*Un bit de stop*/
            serialPort.Handshake = Handshake.None; /*Fara control al fluxului*/
            /*Evenimentul care se declanseaza cand se primesc date*/
            try
            {
                /*Deschide portul serial*/
                serialPort.Open();

                SendCommand("AT");
                System.Threading.Thread.Sleep(500);
                string indata = serialPort.ReadExisting();
                if (!indata.Contains("OK"))
                {
                    Console.WriteLine("Conexiune nereusita.");
                    return;
                }

                    Console.WriteLine("Portul serial este deschis. Ascultând date...");
                    SendCommand("AT+CLIP=1");
                    SendCommand("AT+CNMI=1,1,0,0,0");
                    //messagePollingTimer = new Timer(10000); // Interval de 500 ms-nu e functional, se suprapune cu apelurile
                    //messagePollingTimer.Elapsed += OnMessagePollingEvent;
                    //messagePollingTimer.AutoReset = true;
                    //messagePollingTimer.Enabled = true;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la deschiderea portului serial: " + ex.Message);
            }
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            //string indata;
            System.Threading.Thread.Sleep(500);
            //do
            //{
            string indata = sp.ReadExisting();
            dataBuffer.Append(indata);
            //} while (!string.IsNullOrEmpty(indata));
            //dataBuffer.Append(indata);
            //string indata;
            ////System.Threading.Thread.Sleep(500);
            //do
            //{
            //    indata = sp.ReadExisting();
            //    dataBuffer.Append(indata);
            //} while (!string.IsNullOrEmpty(indata));
            //dataBuffer.Append(indata);
            if (indata.Contains("+CMTI:"))
            {
                string[] date = indata.Split(',');
                string index = date[1];
                SendCommand("AT + CMGF = 1");
                System.Threading.Thread.Sleep(500);
                SendCommand("AT+CMGR=" +index);
                System.Threading.Thread.Sleep(500);
            }

            if ((indata.Contains("+CMGR")||indata.Contains("REC UNREAD") || indata.Contains("+CMGL"))&& !indata.Contains("CLIP"))
            {
                ProcessMessageBuffer();  // Procesează datele de mesaj
                dataBuffer.Clear();
            }
            else if (indata.Contains("+CLIP"))
            {
                ProcessBufferCall();  // Procesează datele de apel
                dataBuffer.Clear();
                //SendCommand("AT");
            }
            //else
            //{
            //    Console.WriteLine(indata);
            //}
        }
        private static void ProcessBufferCall()
        {
            Init.Initialize(out Administrare_FisierText admin, out Utilizator utilizatornou); /*initializari*/
            string delimiter = "\nRING";/*delimitator pentru a verifica daca buffer ul a stocat toate datele complete*/
            while (dataBuffer.ToString().Contains(delimiter))
            {
                /*Extrage mesajul complet din buffer*/
                int delimiterIndex = dataBuffer.ToString().IndexOf(delimiter)+5;
                int stopIndex = dataBuffer.ToString().Length-1;
                string completeMessage = dataBuffer.ToString().Substring(delimiterIndex);
                /*Elimina mesajul complet din buffer*/
                dataBuffer.Remove(0, delimiterIndex + delimiter.Length);
                string cleanedMessage = completeMessage.Trim();
                string callerNumber = ExtractPhoneNumber(cleanedMessage);
                if (cleanedMessage.Contains("+CLIP:"))
                {
                    Console.WriteLine("----Apel primit----");
                    Console.WriteLine("Numar:"+callerNumber);
                    SendCommand("ATH");
                    for (int i = 0; i < 5; i++)
                    {
                        SendCommand("ATH");
                        //System.Threading.Thread.Sleep(100);
                    }
                    if (admin.CautaUtilizator(callerNumber) != null&&callerNumber.Length>11)
                    {
                        Utilizator[] utilizatorigasiti = admin.CautaUtilizator(callerNumber);
                        UserFunctions.AfisareUtilizatori(utilizatorigasiti, utilizatorigasiti.Length);
                        if(utilizatorigasiti.Length==1)
                        {
                            try
                            {
                                // Trimiterea pachetului WoL
                                WakeOnLan.SendWakeOnLan(utilizatorigasiti[0].AdresaMAC);
                                Console.WriteLine("Pachetul WoL a fost trimis.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Eroare la trimiterea pachetului WoL: " + ex.Message);
                            }
                        }
                        else
                            Console.WriteLine("Nu s-a trimis niciun pachet. Acces neautorizat.");
                    }
                    SendCommand("AT");
                }
            }
        }
        private static void ProcessMessageBuffer()
        {
            Init.InitializeTB(out AdminstrareTB_FisierText adminTB, out TestBench testBenchnou);
            Init.Initialize(out Administrare_FisierText admin, out Utilizator utilizatornou);
            string delimiter = "+CMGR:"; // Delimitator pentru mesaje
            string data = dataBuffer.ToString();
            string[] dateseparate = data.Split(new string[] { delimiter }, StringSplitOptions.None);
            dataBuffer.Clear(); // Resetează buffer-ul pentru a procesa datele viitoare
            if (dateseparate[1].Contains("REC UNREAD"))
            {
                Console.WriteLine("----Mesaj primit----");
                string[] parti = dateseparate[1].Split('\n');
                string[] parts = parti[0].Split(',');
                string phoneNumber = ExtractPhoneNumberFromParts(parts);
                string message="";
                Console.WriteLine("Număr: " + phoneNumber);
                if (parti.Length > 1)
                {
                    message = parti[1];
                    Console.WriteLine("Mesaj: " + message);
                }
                else
                {
                    Console.WriteLine("ERROR");
                }
                if (admin.CautaUtilizator(phoneNumber).Length>0 && phoneNumber.Length > 11)
                {
                    TestBench[] testBenchuri = adminTB.CautaTB(message);
                    if (testBenchuri != null)
                    {
                        UserFunctions.AfisareTBURI(testBenchuri, testBenchuri.Length);
                        if (testBenchuri.Length == 1)
                        {
                            try
                            {
                                // Trimiterea pachetului WoL
                                WakeOnLan.SendWakeOnLan(testBenchuri[0].AdresaMAC);
                                Console.WriteLine("Pachetul WoL a fost trimis.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Eroare la trimiterea pachetului WoL: " + ex.Message);
                            }
                        }
                        else
                            Console.WriteLine("Criteriu invalid.");
                    }
                    else
                    {
                        Console.WriteLine("Test bench nerecunoscut.");
                    }
                }
                else
                {
                    Console.WriteLine("Utilizator neautorizat. Acces refuzat.");
                }
            }
        }
        private static string ExtractPhoneNumberFromParts(string[] parts)
        {
            // Extrage numărul de telefon din partea de informații (de obicei în formatul "079104770000")
            if (parts.Length >= 3)
            {
                string phoneNumberPart = parts[1].Trim('"'); // Poate fi nevoie să adaptezi indexul
                phoneNumberPart = FormatPhoneNumber(phoneNumberPart);
                return phoneNumberPart;
            }
            return string.Empty;
        }
        private static string FormatPhoneNumber(string number)
        {
            // Înlătură prefixul "+" și transformă în "004"
            if (number.StartsWith("+40"))
            {
                return "0040" + number.Substring(3); // Formatează în "00407"
            }
            else if (number.StartsWith("40"))
            {
                return "0040" + number.Substring(2); // Formatează în "00407"
            }
            else if (number.StartsWith("07") && number.Length == 10)
            {
                return "0040" + number; // Adaugă prefixul "0040"
            }
            else
            {
                // Returnează numărul original dacă nu se potrivește niciun format specificat
                return number;
            }
        }
        private static void SendCommand(string command)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    /*Adauga o noua linie dupa comanda AT*/
                    serialPort.WriteLine(command + "\r");
                    //Console.WriteLine("Comandă trimisă: " + command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Eroare la trimiterea comenzii: " + ex.Message);
                }
            }
        } 
        private static string ExtractPhoneNumber(string message)
        {
            /*Cauta inceputul si sfarsitul numarului de telefon intre ghilimele*/
            int startIndex = message.IndexOf('"') + 1;
            int endIndex = message.IndexOf('"', startIndex);
            /*Verifica daca indecsii sunt valizi*/
            if (startIndex > 0 && endIndex > startIndex)
            {
                string phoneNumber= message.Substring(startIndex, endIndex - startIndex);
                phoneNumber= FormatPhoneNumber(phoneNumber);
                return phoneNumber;
            }
            return string.Empty; /*Returneaza un sir gol daca numarul nu a fost gasit*/
        }
    }
}