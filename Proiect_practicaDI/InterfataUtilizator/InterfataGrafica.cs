using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibrarieClase;
using NivelStocareDate;
using System.IO.Ports;
namespace InterfataUtilizator
{
    public partial class InterfataGrafica : Form
    {
        private Size originalSizeC;
        private Point originalLocationC;
        private Size originalSizeA;
        private Point originalLocationA;
        public Administrare_FisierText admin;
        private Utilizator[] utilizatori;
        private string numeFisier;
        static SerialPort serialPort;
        static StringBuilder dataBuffer = new StringBuilder();
        Label lblIntrare;
        private void UpdateDateTime()
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        public InterfataGrafica()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Interfață Grafică";
            InitializeComponent();
            UpdateDateTime();
            string macAddress = GetMacAddress();
            /*Seteaza textul pentru Label*/
            lblMAC.Text = macAddress;
            originalLocationC = btnCitire.Location;
            originalSizeC = btnCitire.Size;
            originalLocationA = btnAfisare.Location;
            originalSizeA = btnAfisare.Size;
            string numeFisier = System.Configuration.ConfigurationManager.AppSettings["NumeFisier"];
            string locatieFisierSolutie = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            //string caleCompletaFisier = System.IO.Path.Combine(locatieFisierSolutie, numeFisier);
            string caleCompletaFisier = System.IO.Path.Combine(locatieFisierSolutie, "Proiect_practicaDI", numeFisier);
            admin = new Administrare_FisierText(caleCompletaFisier);
            /*ListenToSerialPort("COM4", 115200);*/
            IncarcaUtilizatori();
            InitializeSerialPort("COM4", 115200);
        }
        public static string GetMacAddress()
        {
            /*Obtine lista de placi de retea*/
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            /*Cautam prima placa de retea activa si obtinem adresa MAC*/
            foreach (var networkInterface in networkInterfaces)
            {
                /*Verifica daca placa de retea nu este de tip Loopback si este activs*/
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var macAddress = networkInterface.GetPhysicalAddress();
                    if (macAddress != null)
                    {
                        /*Formateaza adresa MAC intr-un sir de caractere hexazecimale*/
                        return string.Join("::", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));
                    }
                }
            }
            return "Adresa MAC nu a putut fi gasita";
        }
        private void btnCitire_MouseEnter(object sender, EventArgs e)
        {
            btnCitire.Size = new Size(280, 64);/*in momentul in care mouse ul este deasupra butonului de Adaugare, butonul se va mari*/
            Point newLocation = new Point(btnCitire.Location.X - 15, btnCitire.Location.Y - 7);
            btnCitire.Location = newLocation;/*se schimba pozitia butonului in functie de cat este marit*/
        }
        private void btnCitire_MouseLeave(object sender, EventArgs e)
        {
            btnCitire.Size = originalSizeC;/*butonul revine la starea initiala la plecarea mouse-ului*/
            btnCitire.Location = originalLocationC;
        }
        private void btnAfisare_MouseEnter(object sender, EventArgs e)
        {
            btnAfisare.Size = new Size(280, 64);
            Point newLocation = new Point(btnAfisare.Location.X - 15, btnAfisare.Location.Y - 7);
            btnAfisare.Location = newLocation;
        }
        private void btnAfisare_MouseLeave(object sender, EventArgs e)
        {
            btnAfisare.Size = originalSizeA;
            btnAfisare.Location = originalLocationA;
        }
        private void btnAfisare_Click(object sender, EventArgs e)
        {
            dGUtilizatori.DataSource = null;/*se reseteaza continutul datagrid-ului*/
            IncarcaUtilizatori();/*se foloseste metoda IncarcaUtilizatori pentru afisarea listei din fisier*/
        }
        private void IncarcaUtilizatori()
        {
            Utilizator[] utilizatori = admin.GetUtilizatori(out int nrUtilizatori);/*se preiau utilizatorii din fisier*/
            dGUtilizatori.DataSource = utilizatori.Select(utilizator => new
            {
                utilizator.Nume,
                utilizator.Numar,
                utilizator.AdresaMAC
            }).ToList();/*Datele sunt puse in datagrid*/
        }
        private void btnCitire_Click(object sender, EventArgs e)
        {
            FormaCitire formaCitire = new FormaCitire();
            formaCitire.ShowDialog();/*se deschide o noua forma pentru citirea si adaugarea noului utilizator*/
            IncarcaUtilizatori();/*in momentul in care se va inchide formaCitire, metoda IncarcaUtilizatori este apelata din nou si va afisa lista actualizata utilizatorilor*/
        }
        private void btnSterge_Click(object sender, EventArgs e)
        {
            utilizatori = admin.GetUtilizatori(out int nrUtilizatori);/*se preiau utilizatorii din fisier*/
            if (dGUtilizatori.SelectedRows.Count > 0)/*verifica daca exista vreun rand selecdtat*/
            {
                int selectedIndex = dGUtilizatori.SelectedRows[0].Index;/*preia indexul randului selectat*/
                if (selectedIndex >= 0 && selectedIndex < utilizatori.Length)
                {
                    Utilizator utilizator = utilizatori[selectedIndex];
                    admin.StergeUtilizator(utilizator.Nume);/*se sterge utilizatorul*/
                    IncarcaUtilizatori();/*refresh la datagrid*/
                    MessageBox.Show("Elementul a fost sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);/*mesaj de confirmare*/
                }
                else
                {
                    MessageBox.Show("Indexul selectat este in afara limitelor.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Selectati un rand pentru a sterge.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);/*mesaj eroare cand nu este selectat un rand*/
            }
        }
        private void btnModifica_Click(object sender, EventArgs e)
        {
            //utilizatori = admin.GetUtilizatori(out int nrUtilizatori);/*se preia tabloul de utilizatori din fisier*/
            /*se creeaza o noua forma de tip FormaCitire*/
            if (dGUtilizatori.SelectedRows.Count > 0)/*verificare randuri selectate*/
            {
                int selectedIndex = dGUtilizatori.SelectedRows[0].Index;/*memorare index rand*/
                Utilizator[] utilizatori = GetUtilizatoriDinDataGrid();
                if (selectedIndex >= 0 && selectedIndex < utilizatori.Length)
                {
                    Utilizator utilizator = utilizatori[selectedIndex];/*se atribuie obiectului utilizator valoarea selectata din datagrid, prin accesarea lui din tablou cu ajutorul indexului*/
                    /*Seteaza valorile inițiale în FormaCitire*/
                    FormaModifica formaModifica = new FormaModifica(utilizator.Nume);
                    formaModifica.Nume = utilizator.Nume;
                    formaModifica.Nr = utilizator.Numar;
                    formaModifica.Adresa = utilizator.AdresaMAC;
                    string numeOriginal = utilizator.Nume;
                    admin.StergeUtilizator(numeOriginal);/*se sterge utilizatorul initial*/
                    /*Afiseaza forma si asteapta pana cand este inchisa*/
                    DialogResult result = formaModifica.ShowDialog();
                    if (result == DialogResult.OK /*||formaModifica.ShowDialog() == DialogResult.Cancel*/)
                    {
                        utilizator.Nume = formaModifica.Nume;
                        utilizator.Numar = formaModifica.Nr;
                        utilizator.AdresaMAC = formaModifica.Adresa;
                        /*Actualizeaza utilizatorul in sistemul de administrare*/
                        admin.AddUtilizator(utilizator);/*se adauga utilizatorul cu datele modificate*/
                        MessageBox.Show("Elementul a fost modificat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);/*mesaj confirmare*/
                    }
                    if (result== DialogResult.Cancel)
                    {
                        admin.AddUtilizator(utilizator);
                        formaModifica.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Indexul selectat este in afara limitelor.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Selectati un rand pentru a sterge.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            IncarcaUtilizatori();/*Reincarca utilizatorii in data grid*/
        }
        private void btnCautare_Click(object sender, EventArgs e)
        {
            string numeCautat = txtCautare.Text.Trim();/*se preia criteriul/numele din casuta de cautare*/
            if (string.IsNullOrEmpty(numeCautat))/*verifica daca textBox-ul este de fapt gol (niciun criteriu pentru cautare)*/
            {
                MessageBox.Show("Introduceti un nume pentru cautare.");
                return;
            }
            var utilizatorigasiti = admin.CautaUtilizator(numeCautat);/*se creeaza un alt tablou pentru utilizatorii gasiti ce indeplinesc conditia*/
            dGUtilizatori.DataSource = null;/*se sterge continutul din datagrid*/
            dGUtilizatori.DataSource = utilizatorigasiti;/*se inlocuieste continutul din datagrid cu utilizatorii gasiti*/
        }
        private Utilizator[] GetUtilizatoriDinDataGrid()
        {
            Utilizator[] utilizatori = new Utilizator[dGUtilizatori.Rows.Count];
            int index = 0;
            foreach (DataGridViewRow row in dGUtilizatori.Rows)
            {
                if (row.Cells["Nume"].Value != null && row.Cells["Numar"].Value != null && row.Cells["AdresaMAC"].Value != null)
                {
                    utilizatori[index] = new Utilizator()
                    {
                        Nume = row.Cells["Nume"].Value.ToString(),
                        Numar = row.Cells["Numar"].Value.ToString(),
                        AdresaMAC = row.Cells["AdresaMAC"].Value.ToString()
                    };
                    index++;
                }
            }
            return utilizatori;
        }
        private void InitializeSerialPort(string portName, int baudRate)
        {
            serialPort = new SerialPort(portName)
            {
                BaudRate = baudRate,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None
            };
            /*Seteaza evenimentul pentru primirea datelor*/
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                serialPort.Open();
                UpdateLblIntrare("Portul serial este deschis. Ascultând date...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la deschiderea portului serial: " + ex.Message);
            }
        }
        /*Metoda pentru procesarea datelor primite*/
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            dataBuffer.Append(indata);
            ProcessBuffer();
        }
        private void ProcessBuffer()
        {
            string delimiter = "\nRING"; /*Delimitatorul pentru date complete*/
            while (dataBuffer.ToString().Contains(delimiter))
            {
                int delimiterIndex = dataBuffer.ToString().IndexOf(delimiter);
                string completeMessage = dataBuffer.ToString().Substring(0, delimiterIndex);
                dataBuffer.Remove(0, delimiterIndex + delimiter.Length);
                string cleanedMessage = completeMessage.Trim();
                string callerNumber = ExtractPhoneNumber(cleanedMessage);
                if (cleanedMessage.Contains("+CLIP:"))
                {
                    UpdateLblIntrare("Date primite: " + cleanedMessage + "\nNumar: " + callerNumber);
                    SendCommand("ATH");
                    WaitForOkResponse();
                    if (admin.CautaUtilizator(callerNumber) != null)
                    {
                        Utilizator[] utilizatorigasiti = admin.CautaUtilizator(callerNumber);
                        if (utilizatorigasiti.Length>0)
                        {
                            foreach (Utilizator user in utilizatorigasiti)
                            {
                                UpdateLblIntrare(user.Info());
                            }
                        }
                        else
                        {
                            UpdateLblIntrare("Utilizator necunoscut. Refuzat.");
                        }
                    }
                    SendCommand("AT");
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
                return message.Substring(startIndex, endIndex - startIndex);
            }
            return string.Empty; /*Returneaza un sir gol daca numarul nu a fost gasit*/
        }
        private static void WaitForOkResponse()
        {
            string okResponse = "OK\r"; /*Raspunsul asteptat*/
            string receivedData = string.Empty;
            /*Continua să verifice pana cand gaseste confirmarea OK*/
            while (true)
            {
                /*Verifica daca exista date disponibile in buffer*/
                if (serialPort.BytesToRead > 0)
                {
                    /*Citeste datele din portul serial*/
                    receivedData += serialPort.ReadExisting();

                    /*Verifica daca datele contin confirmarea OK*/
                    if (receivedData.Contains(okResponse))
                    {
                        //Elimina confirmarea OK din bufferul de date*/
                        int okIndex = receivedData.IndexOf(okResponse);
                        receivedData = receivedData.Substring(okIndex + okResponse.Length);
                        return; /*Iese din bucla dupa ce a primit confirmarea*/
                    }
                }
                /*Asteapta putin inainte de a verifica din nou*/
                System.Threading.Thread.Sleep(100); /*100 ms pentru a preveni consumul excesiv de CPU*/
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la trimiterea comenzii: " + ex.Message);
                }
            }
        }
        private void UpdateLblIntrare(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateLblIntrare), text);
            }
            else
            {
                lblIntrari.Text += "\n"+text;
            }
        }
    }
}

