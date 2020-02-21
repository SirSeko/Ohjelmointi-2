using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using CsvHelper;


namespace Henkilotiedot
{

    public partial class Form1 : Form
    {

        bool uusi = true; // onko kyseessä muokkaus
        bool muutettu = false; // onko kenttiä muutettu
        
        // Lokilaskurit
        int PoistaLogi = 0;
        int TallennaLogi = 0;
        int MuokkaaLogi = 0;
        int Lisaalogi = 0;

        int muutetturivi = 0; //Muutetun rivin indexi
        public static string Nimi = Environment.UserName; //käyttäjänimi
        public DateTime loginTime, logoutTime; //ulos/kirjautumisaika
        public List <Hlotiedot> Henkilotiedot = new List<Hlotiedot>(); //Luodaan Henkilotietolista
        
        public BindingSource Bindaus = new BindingSource(); //Luodaan BindingSource
        
        //CSP JA RSA parametrit
        CspParameters cspp = new CspParameters();
        RSACryptoServiceProvider rsa;
        //Salaus, Purku ja Lähde-kansiot
        const string EncrFolder = @"C:\temp\Encrypt\";
        const string DecrFolder = @"C:\temp\Decrypt\";
        const string SrcFolder = @"C:\temp\";

        //julkinen avain
        const string PubKeyFile = @"c:\temp\Encrypt\rsaPublicKey.txt";

        //Avaimen nimi
        const string keyName = "Key01";
        
        //Postinumeron ja Toimipaikan ehdotustoiminto
        public AutoCompleteStringCollection source = new AutoCompleteStringCollection();
        public AutoCompleteStringCollection source2 = new AutoCompleteStringCollection();

        Form2 tf2 = new Form2(); // Esitellään form 2

        

        public Form1()
        {
            
            InitializeComponent();
            tabPage1.Text = @"Henkilotiedot"; // tabien nimet
            tabPage2.Text = @"Muokkaus";
            tabPage1.BackColor = Color.Silver; //tabien taustavärit
            tabPage2.BackColor = Color.Silver;
            tlsNimi.Text = Nimi;
            tf2.txbUusNimike.Text = "Astronautti";
            tf2.txbUusiYksikko.Text = "Avaruus";
            loginTime = DateTime.Now; //Sisäänkirjautumisaika
            Henkilotiedot = new List<Hlotiedot>(); // esitellään henkilotietolista
            dtaHlo.SelectionMode = DataGridViewSelectionMode.FullRowSelect; //Datagridissä valinta koskee aina koko riviä
            
            var Blista2 = new BindingList<Hlotiedot>(Henkilotiedot);
            
            Bindaus.DataSource = Blista2;
            dtaHlo.DataSource = Bindaus;
           
            if (File.Exists(@"c:\temp\Henkilotiedot.csv")) // Jos tiedosto on olemassa avataan se
            {
                string sourceFile = @"c:\temp\Henkilotiedot.csv";
                using (TextReader fileReader = File.OpenText(sourceFile)) // Luetaan csv tiedosto
                {
                    var csv = new CsvReader(fileReader);
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.Encoding = Encoding.UTF8;
                    
                    Henkilotiedot = csv.GetRecords<Hlotiedot>().ToList<Hlotiedot>(); //Luetaan tiedot suoraan listaan
                    fileReader.Close();
                }  
                dtaHlo.DataSource = null;
                Bindaus.DataSource = Henkilotiedot;
                dtaHlo.DataSource = Bindaus; // Listan Tietolähde
                foreach (DataGridViewColumn col in dtaHlo.Columns) 
                {
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
            }
            else
            {
                // Jos tiedostoa ei ole, näytetään tyhjä lista.
                dtaHlo.DataSource = Bindaus;
                
            }
        }
        void Form1_Load(object sender, EventArgs e)
        {

            Tarkista(this); //kutsutaan tarkistaja
            tf2.InitializeNimike();
            
            //Comboboxin lähde
            cmbNimike.DataSource = tf2.Blista; //Comboksin data source
            cmbNimike.DisplayMember = "NimikeNim"; //Mitä comboboxi näytää
            cmbYksikko.DataSource = tf2.Blista;
            cmbYksikko.DisplayMember = "YksikkoNim";
            
            txbToimi.AutoCompleteCustomSource = source; // Mihin Toimipaikan tiedot tallennetaan ennakointia varten
            txbPostinr.AutoCompleteCustomSource = source2; // Mihin Postinumerot tallenetaan ennakointia varten
            foreach (DataGridViewColumn column in dtaHlo.Columns)
            {
                //Kaikki sarakkeiden lajittelu on ohjelmoitavissa
                column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }

        }
       
        void Tarkista(Control tarkistus) //tarkistetaan onko kenttiä muutettu
        {
            foreach (Control trk in tarkistus.Controls)
            {
                if (trk is TextBox)
                {
                    TextBox tb = (TextBox)trk;
                    tb.TextChanged += new EventHandler(tb_TextChanged);
                    // Jos tekstibokseja on muutettu
                }
                else
                {
                    Tarkista(trk);
                }
            }
        } //Jos tekstiboksia muutetaan
        void tb_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            muutettu = true;
        }

        //kun tallenna nappia painetaan
        private void btnTallenna_Click(object sender, EventArgs e)
        {
            Tallenna(); // Kutsutaan tallenna funktio
        }
        Hlotiedot h;
        //Tallenna funktio
        private void Tallenna()
        {

            try
            {
               
                //Ottaa tiedot tekstibokseista
                string Nimet = txbEtunimi.Text;
                string Sukunimi = txbSukunimi.Text;
                string Kutsumanimi = txbKutsu.Text;
                string Osoite = txbOsoite.Text;
                string Postitoimipaikka = txbToimi.Text;
                int Postinumero = int.Parse(txbPostinr.Text);
                string Hetu = mxbHetu.Text;
                string Nimike = cmbNimike.Text;
                string Yksikko = cmbYksikko.Text;
                string Aloitus = dtpAlkamis.Value.ToString("dd-MMM-yyyy");
                string Lopetus = dtpPaattymis.Value.ToString("dd-MMM-yyyy");
                //Jos halutaan muokata tietoa:
                if (!uusi)
                {
                    h = Henkilotiedot[muutetturivi];
                   
                } // Jos halutaan luoda uusi asiakas
                else
                {
                    h = new Hlotiedot();
                }
                h.Nimet = Nimet;
                h.Sukunimi = Sukunimi;
                h.Kutsumanimi = Kutsumanimi;
                h.Osoite = Osoite;
                h.Postitoimipaikka = Postitoimipaikka;
                h.Postinumero = Postinumero;
                h.Hetu = Hetu;
                h.Nimike = Nimike;
                h.Yksikko = Yksikko;
                h.Aloitus = Aloitus;
               
                if (checkBox1.Checked)
                {
                    h.Lopetus = " ";
                }
                else
                {
                    h.Lopetus = Lopetus;
                }
                
                if (uusi)
                {
                    Henkilotiedot.Add(h);
                    Logi2();

                }
                else
                {
                    Henkilotiedot[muutetturivi] = h;
                    
                }
                
                
                source2.Add(txbPostinr.Text);
                source.Add(txbToimi.Text);



                tabControl1.SelectedTab = tabPage1;
                
                uusi = false;
                dtaHlo.DataSource = null;
                dtaHlo.DataSource = Bindaus;
                Tyhjenna();
                
                muutettu = false;
                TallennaLogi++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //kun ohjelmaa suljetaan
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (muutettu == true) //Jos kenttiä on muutettu kysytään halutaanko tiedot tallentaa
            {
                DialogResult result = MessageBox.Show("Haluatko tallentaa muutokset ennen sulkemista?", "Tallenna", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    logoutTime = DateTime.Now; //Uloskirjautumisaika
                    Logi(); // Tallenna tiedot
                    //tallenna tiedot
                    Tallenna();
                    luoTavallinenTiedostoToolStripMenuItem.Click += luoTavallinenTiedostoToolStripMenuItem_Click;
                }
                else if (result == DialogResult.Cancel)
                {
                    //älä sulje
                    e.Cancel = true;
                }

            }
            else
            {
                logoutTime = DateTime.Now;
                Logi();
            }
        }
        // Muokkaa nappia painetaan
        private void btnMuokkaa_Click(object sender, EventArgs e)
        {
            uusi = false;
            muutettu = true;
            
            tabControl1.SelectedTab = tabPage2;
            try
            {
                //Luetaan tiedot listasta
                muutetturivi = dtaHlo.CurrentCell.RowIndex;
                DataGridViewRow row = dtaHlo.Rows[muutetturivi];

                txbEtunimi.Text = row.Cells[0].Value.ToString();
                txbSukunimi.Text = row.Cells[1].Value.ToString();
                txbKutsu.Text = row.Cells[2].Value.ToString();
                txbOsoite.Text = row.Cells[3].Value.ToString();
                txbPostinr.Text = row.Cells[5].Value.ToString();
                txbToimi.Text = row.Cells[4].Value.ToString();
                mxbHetu.Text = row.Cells[6].Value.ToString();
                cmbNimike.Text = row.Cells[7].Value.ToString();
                cmbYksikko.Text = row.Cells[8].Value.ToString();

                dtpAlkamis.Value = Convert.ToDateTime(row.Cells[9].Value.ToString());
                if (row.Cells[10].Value.ToString() == " ")
                {
                    checkBox1.Checked = true; //Jos valintaboksissa on ruksi, jätetään päättymispäivä tyhjäksi
                }
                else
                {
                    dtpPaattymis.Value = Convert.ToDateTime(row.Cells[10].Value.ToString()); //Muunnetaan listassa olevan tiedon muoto datetimepickerille sopivaksi
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            Logi2();

            //Lisätään logiin tieto muokkauksesta
            MuokkaaLogi++;


        }
       
        
        //Kun nimikettä/yksikköä muokataan
        private void btnMuoknimi_Click(object sender, EventArgs e)
        {
            tf2.ShowDialog(); // Näytetään uusi formi
        }

        //Kun halutaan poistaa Nimike ja Yksikkö
        public void btnPoistnimi_Click(object sender, EventArgs e)
        {

            int valittu = cmbNimike.SelectedIndex;

            if (valittu > -1) // Kun valittu on suurempi kuin -1, tällä vältetään index out of range virheet
            {
                tf2.Blista.RemoveAt(valittu);
                MessageBox.Show("Poistettu listasta!");

            }

        }
        // Kun checkboxin valinta muuttuu, eli jos työsuhde on toistaiseksi voimassa oleva
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //Jos valittu niin päättymispäivää ei voi valita.
                dtpPaattymis.Enabled = false;

            }
            else
                dtpPaattymis.Enabled = true;
        }
        // Kun tyhjennä nappia painetaan
        private void btnTyhjenna_Click(object sender, EventArgs e)
        {
            Tyhjenna();

        }
        //Tyhjennetään kaikki tekstiboksit
        private void Tyhjenna()
        { 
            if (true)
            {
                Action<Control.ControlCollection> func = null;
                func = (controls) =>
                {
                    foreach (Control control in controls) //Jokainen tekstiboxi ja maskedboxi tyhjennetään
                    {
                        if (control is TextBox)
                            (control as TextBox).Clear();
                        if (control is MaskedTextBox)
                            (control as MaskedTextBox).Clear();

                        else
                            func(control.Controls);

                    }

                };
                func(Controls);

            }
        }
        //Kun date timepickerin päättymis päivämäärää muutetaan
        private void dtpPaattymis_ValueChanged(object sender, EventArgs e)
        {
            // Jos päättymispäivä on pienempi kuin alkamispäivä
            if (dtpPaattymis.Value < dtpAlkamis.Value && checkBox1.Checked == false)
            {
                MessageBox.Show("Päättymispäivä ei voi olla ennen alkamispäivää!");
                dtpPaattymis.Value = dtpAlkamis.Value.AddDays(01);
            }

        }
        //Validoidaan hetua
        private void mxbHetu_Validating(object sender, CancelEventArgs e)
        {
            string Alkuosa = "";
            string Loppuosa = "";
            string Tarkistusmerkki = "";
           

            bool OikeaHetu = false;
            //Pilkotaan hetu osiin
           


            char kirjain = 'A';
            // Jos Lasku vastaa Charria = Hetu oikein tai jos Loppuosa on  alle 10;
            try
            {
                Alkuosa = mxbHetu.Text.Substring(0, 6);
                Loppuosa = mxbHetu.Text.Substring(7, 3);
                //string Vuosisata = mxbHetu.Text.Substring(8, 1);
                Tarkistusmerkki = mxbHetu.Text.Substring(mxbHetu.Text.Length - 1);
                string Tarkistus = Alkuosa + Loppuosa;
                int Lasku = int.Parse(Tarkistus) % 31;
                //Lasketaan hetun osat ja vertaillaan niitä kirjaimiin
                if (Lasku >= 10 && Lasku <= 15)
                {
                    kirjain = Convert.ToChar(Lasku + 31);
                }
                else if (Lasku == 16)
                {
                    kirjain = Convert.ToChar(Lasku + 56);
                }
                else if (Lasku >= 17 && Lasku <= 21)
                {
                    kirjain = Convert.ToChar(Lasku + 57);
                }
                else if (Lasku == 22)
                {
                    kirjain = Convert.ToChar(Lasku + 58);
                }
                else if (Lasku >= 23 && Lasku <= 30)
                {
                    kirjain = Convert.ToChar(Lasku + 59);
                }
                else if (Lasku == int.Parse(Tarkistusmerkki))
                {
                    OikeaHetu = true;
                }
                else
                {
                    OikeaHetu = false;
                }

                if (kirjain == Convert.ToChar(Tarkistusmerkki))
                {
                    OikeaHetu = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Väärä Muoto");

            }

            if (OikeaHetu == false)
            {
                string viesti = "Väärä muoto";
                e.Cancel = true;
                mxbHetu.Select(0, mxbHetu.Text.Length);
                erpHetu.SetError(mxbHetu, viesti);

            }



        }
        //Kun Hetu on validoitu
        private void mxbHetu_Validated(object sender, EventArgs e)
        {
            MessageBox.Show("Hetu oikein");
            erpHetu.SetError(mxbHetu, "");

        }

     
        //Lisää nappia paianessa
        private void btnLisaa_Click(object sender, EventArgs e)
        {
            uusi = true;
            muutettu = false;
            tabControl1.SelectedTab = tabPage2; // Valitaan Muokkaus tabi
            Lisaalogi++;
            
        }

       

        //Luodaan salausavain
        private void LuoAvain()
        {
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
            if (rsa.PublicOnly == true)
            {
                //Näytetään labelissa jos avain on vain yleinen
                tlsStatus1.Text = "Avain: " + cspp.KeyContainerName + " - Vain Yleinen";
            }
            else
            {
                //Näytetään labelissa jos täysi avainpari
                tlsStatus1.Text = "Avain: " + cspp.KeyContainerName + " - Täysi Avainapari";
            }
        }
        //Salastoiminto
        private void EncryptFileToiminto()
        {
            if (rsa == null)
            {
                //Jos avainta ei ole luotu
                MessageBox.Show("Avainta ei asetettu");

            }
            else
            {
                
                //Näytä mikä tiedosto salataan
                ofpAvaa.InitialDirectory = SrcFolder;
                if (ofpAvaa.ShowDialog() == DialogResult.OK)
                {
                    
                    string fName = ofpAvaa.FileName;
                    if (fName != null)
                    {
                        FileInfo fInfo = new FileInfo(fName);
                        // Antaa nimen ilman polkua.
                        string name = fInfo.FullName;
                        EncryptFile(name);
                    }
                }
            }
        }
        //Luodaan salattu tiedosto
        private void EncryptFile(string inFile)
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.KeySize = 256;
            rijndael.BlockSize = 256;
            rijndael.Mode = CipherMode.CBC;
            ICryptoTransform transform = rijndael.CreateEncryptor();

            //Käytä RSACryptoServiceProvideria

            byte[] keyEncrypted = rsa.Encrypt(rijndael.Key, false);

            //Luo bittitaulokko joka sisältää arvot:

            byte[] PitK = new byte[4];
            byte[] PitIV = new byte[4];

            int lKey = keyEncrypted.Length;
            PitK = BitConverter.GetBytes(lKey);
            int lIV = rijndael.IV.Length;
            PitIV = BitConverter.GetBytes(lIV);

            //Kirjoita striimiin avaimen pituus, IVn pituus, salattu avain, IV ja salattu cipher sisältö

            int aloitaTiedostoNimi = inFile.LastIndexOf("\\") + 1;
            //muuta tiedoston muoto ".enc"
            string outFile = EncrFolder + inFile.Substring(aloitaTiedostoNimi, inFile.LastIndexOf(".") - aloitaTiedostoNimi) + ".enc";
            //Kirjoitetaan salattu tiedosto
            using (FileStream outFs = new FileStream(outFile, FileMode.Create))
            {
                outFs.Write(PitK, 0, 4);
                outFs.Write(PitIV, 0, 4);
                outFs.Write(keyEncrypted, 0, lKey);
                outFs.Write(rijndael.IV, 0, lIV);

                using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {
                    int count = 0;
                    int offset = 0;

                    int blockSizeBytes = rijndael.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;
                    using (FileStream inFs = new FileStream(inFile, FileMode.Open))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        } while (count > 0);
                        inFs.Close();
                    }
                    outStreamEncrypted.FlushFinalBlock();
                    outStreamEncrypted.Close();
                }
                outFs.Close();
            }
        


        }
        //Salaustoiminto
        private void DecryptFileToiminto()
        {
            //Tarkistetaan onko avainta luotu
            if (rsa == null)
                MessageBox.Show("Avainta ei asetettu.");
            else
            {
                //Näytä dialogiboxi josta valitaan salattu tiedosto
                openFileDialog2.InitialDirectory = EncrFolder;
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    string fName = openFileDialog2.FileName;
                    if (fName != null)
                    {
                        FileInfo fi = new FileInfo(fName);
                        string name = fi.Name;
                        DecryptFile(name);
                    }
                }
            }
        }
        //Salausfunktio
        private void DecryptFile(string inFile)
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.KeySize = 256;
            rijndael.BlockSize = 256;
            rijndael.Mode = CipherMode.CBC;

            byte[] PitK = new byte[4];
            byte[] PitIV = new byte[4];

            string outFile = DecrFolder + inFile.Substring(0, inFile.LastIndexOf(".")) + ".csv"; //tallennetaan csv tiedostoon
            using (FileStream inFs = new FileStream(EncrFolder + inFile, FileMode.Open))
            {
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(PitK, 0, 3);
                inFs.Seek(4, SeekOrigin.Begin);
                inFs.Read(PitIV, 0, 3);

                int pitK = BitConverter.ToInt32(PitK, 0);
                int pitIV = BitConverter.ToInt32(PitIV, 0);

                int startC = pitK + pitIV + 8;
                int lenC = (int)inFs.Length - startC;

                byte[] KeyEncrypted = new byte[pitK];
                byte[] IV = new byte[pitIV];

                //Haetaan avain ja IV aloitetaan indeksistä 8
                inFs.Seek(8, SeekOrigin.Begin);
                inFs.Read(KeyEncrypted, 0, pitK);
                inFs.Seek(8 + pitK, SeekOrigin.Begin);
                inFs.Read(IV, 0, pitIV);
                Directory.CreateDirectory(DecrFolder);

                // käytetään RSA CryptoserviceProvideria decryptaamiseen
                byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);
                // avataan avain
                ICryptoTransform transform = rijndael.CreateDecryptor(KeyDecrypted, IV);
                // Decryptataan Filestriimistä
                using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                {
                    int count = 0;
                    int offset = 0;

                    int blockSizeBytes = rijndael.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];

                    inFs.Seek(startC, SeekOrigin.Begin);
                    using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamDecrypted.Write(data, 0, count);
                        } while (count > 0);
                        outStreamDecrypted.FlushFinalBlock();
                        outStreamDecrypted.Close();

                    } outFs.Close();
                }
                inFs.Close();

            }
        }
        //Luodaan salattu tiedosto
        private void luoSalattuTiedostoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EncryptFileToiminto();
        }
        // Luodaan tavallinen csv tiedosto
        private void luoTavallinenTiedostoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            using (SaveFileDialog sfdTallenna = new SaveFileDialog() { Filter = "CSV|*.csv", ValidateNames = true })
            {
                if (sfdTallenna.ShowDialog() == DialogResult.OK)
                {
                    using (var sw = new StreamWriter(sfdTallenna.FileName, false, Encoding.UTF8))
                    {
                        var writer = new CsvWriter(sw);
                        writer.WriteHeader(typeof(Hlotiedot));
                        writer.NextRecord();
                        foreach (Hlotiedot h in Henkilotiedot) 
                        {
                            writer.WriteRecord(h);
                            writer.NextRecord();
                        }
                        sw.Close();
                    }
                    MessageBox.Show("Tiedot tallennettu CSV tiedostoon.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
           
        }
        //Kun salattu tiedosto avataan
        private void avaaSalattuTiedostoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\temp\Encrypt\rsaPublicKey.txt"))
            {
                //Tarkistetaan onko avainta olemassa, jos on niin kutsutaan purku funktio
                DecryptFileToiminto();
            }
            else
            {
                MessageBox.Show("Avainta ei asetettu");
            }
        }
        //Kun tavallinen tiedosto avataan
        private void avaaTavallinenTiedostoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            using (OpenFileDialog ofpAvaa = new OpenFileDialog() { Filter = "CSV|*.csv", ValidateNames = true })
            {

                if (ofpAvaa.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sourceFile = ofpAvaa.FileName;
                        using (TextReader fileReader = File.OpenText(sourceFile))
                        {
                            var csv = new CsvReader(fileReader);
                            csv.Configuration.HasHeaderRecord = true;
                            csv.Configuration.Encoding = Encoding.UTF8;
                            Henkilotiedot = csv.GetRecords<Hlotiedot>().ToList<Hlotiedot>();

                            fileReader.Close();

                        }
                        dtaHlo.DataSource = null;
                        Bindaus.DataSource = Henkilotiedot;
                        dtaHlo.DataSource = Bindaus;
                                                
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        
                    }

                   
                }
                
              
            }

        }
        
      
       // Luodaan avain nappia painamalla
        private void btnSalaa_Click(object sender, EventArgs e)
        {
            LuoAvain();
        }
        // Tuplaklikkauksella siirrä valitut tiedot listasta labeleihin
        private void dtaHlo_RowHeaderMouseDoubleClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            lblEt.Text = dtaHlo[0, e.RowIndex].Value.ToString();
            lblKut.Text = dtaHlo[2, e.RowIndex].Value.ToString();
            lblSn.Text = dtaHlo[1, e.RowIndex].Value.ToString();
            lblOs.Text = dtaHlo[3, e.RowIndex].Value.ToString();
            lblPost.Text = dtaHlo[4, e.RowIndex].Value.ToString();
            lblPnr.Text = dtaHlo[5, e.RowIndex].Value.ToString();
            lblHetu.Text = dtaHlo[6, e.RowIndex].Value.ToString();
            lblYks.Text = dtaHlo[8, e.RowIndex].Value.ToString();
            lblNim.Text = dtaHlo[7, e.RowIndex].Value.ToString();
            lblAlk.Text = dtaHlo[9, e.RowIndex].Value.ToString();
            lblLop.Text = dtaHlo[10, e.RowIndex].Value.ToString();
        }
        //Poista valittu tieto
        private void btnPoista_Click(object sender, EventArgs e)
        {
            
            int rowIndex = dtaHlo.CurrentCell.RowIndex;
            DialogResult jes = MessageBox.Show("Haluatko varmasti poistaa tiedot?", "Varmistus", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (rowIndex >= 0 && jes == DialogResult.OK)
            {
                Logi2();
                dtaHlo.ClearSelection();
                
                Bindaus.RemoveAt(rowIndex);
                dtaHlo.DataSource = null;
                dtaHlo.DataSource = Bindaus;
                PoistaLogi++;
                uusi = false;
                
                
            }
            else if (rowIndex <= 0)
            {
                MessageBox.Show("Mitään ei ole valittu");
            }
            
        }
        
        private void Form1_Resize(object sender, EventArgs e)
        {
            dtaHlo.AutoResizeRows();
            dtaHlo.AutoResizeColumns();
        }
      //Postintoimipaikan ennakoija
        private void txbPostinr_TextChanged(object sender, EventArgs e)
        {  
            int index = source2.IndexOf(txbPostinr.Text);
            if (index >= 0)
            {
                txbToimi.Text = source[index].ToString();
            }
           
            
        }
        
      
        //Kun saraketta klikataan
        private void dtaHlo_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            SortOrder so = SortOrder.None;
            if (grid.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.None ||
                grid.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
            {
                so = SortOrder.Descending;
            }
            else
            {
                so = SortOrder.Ascending;
            }
            //Aseta SortGlyphDirection 
            Sort(grid.Columns[e.ColumnIndex].Index, so);
            grid.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = so;
        }
        //Järjestää sarakkeet a-z
        private void Sort(int column, SortOrder sortOrder)
        {
            switch (column)
            {
                case 0:
                    {
                        if (sortOrder == SortOrder.Ascending)
                        {
                            dtaHlo.DataSource = Henkilotiedot.OrderBy(x => x.Nimet).ToList();
                        }
                        else
                        {
                            dtaHlo.DataSource = Henkilotiedot.OrderByDescending(x => x.Nimet).ToList();
                        }
                        break;
                    }
                case 1:
                    {
                        if (sortOrder == SortOrder.Ascending)
                        {
                            dtaHlo.DataSource = Henkilotiedot.OrderBy(x => x.Sukunimi).ToList();
                        }
                        else
                        {
                            dtaHlo.DataSource = Henkilotiedot.OrderByDescending(x => x.Sukunimi).ToList();
                        }
                        break;
                    }
                case 7:
                    {
                        if (sortOrder == SortOrder.Ascending)
                        {
                            dtaHlo.DataSource = Henkilotiedot.OrderBy(x => x.Nimike).ToList();
                        }
                        else
                        {
                            dtaHlo.DataSource = Henkilotiedot.OrderByDescending(x => x.Nimike).ToList();
                        }
                        break;
                    }
            }
        }
        
        //Logi kirjoittaja
        public void Logi()
        {

            using (StreamWriter sw = new StreamWriter(@"C:\temp\logi.txt", true))
            {
                sw.WriteLine($"Kirjautumisaika: {loginTime.ToString()}\tKäyttäjänimi: {Nimi} \tUloskirjautumisaika: {logoutTime.ToString()}");
                sw.WriteLine($"Koneen Nimi: {Environment.MachineName}\t Käyttöjärjestelmä: {Environment.OSVersion.ToString()}\t ");
                sw.WriteLine("Lisättyjä rivejä: {0}\t Muokattuja rivejä: {1}\t Poistettuja rivejä: {2} ", Lisaalogi, MuokkaaLogi, PoistaLogi);
                sw.Close();
            }
        }
        // Tallenna avain tiedostoon
        private void btnVieAvain_Click(object sender, EventArgs e)
        {
          
            Directory.CreateDirectory(EncrFolder);
            StreamWriter sw = new StreamWriter(PubKeyFile, false);
            sw.Write(rsa.ToXmlString(false));
            sw.Close();
        }
        //Tuo tallennettu avain
        private void btnTuoAvain_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(PubKeyFile);
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            string keytxt = sr.ReadToEnd();
            rsa.FromXmlString(keytxt);
            rsa.PersistKeyInCsp = true;
            if (rsa.PublicOnly == true)
                tlsStatus1.Text = "Key: " + cspp.KeyContainerName + " - Public Only";
            else
                tlsStatus1.Text = "Key: " + cspp.KeyContainerName + " - Full Key Pair";
            sr.Close();
        }
        //Luo yksityinen avain
        private void btnYksAvain_Click(object sender, EventArgs e)
        {
            cspp.KeyContainerName = keyName;

            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            if (rsa.PublicOnly == true)
                tlsStatus1.Text = "Key: " + cspp.KeyContainerName + " - Public Only";
            else
                tlsStatus1.Text = "Key: " + cspp.KeyContainerName + " - Full Key Pair";
        }

        public void Logi2()
        {
            logoutTime = DateTime.Now;
            using (StreamWriter sw = new StreamWriter(@"C:\temp\logi.txt", true))
            {
                if (uusi == true)
                {
                    sw.WriteLine("Lisätty rivi: {0}, {1}", txbEtunimi.Text, logoutTime.ToString());
                }
                else if (muutettu == true)
                {
                    sw.WriteLine("Muutettu rivi: {0}, {1}", txbEtunimi.Text, logoutTime.ToString());
                }
                else
                {
                    sw.WriteLine("Poistettu rivi: {0}, {1}", dtaHlo.SelectedCells[0].Value.ToString(), logoutTime.ToString());
                }
                sw.Close();
            }
        }





    }
}
