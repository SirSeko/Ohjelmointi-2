using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Henkilotiedot
{
    public partial class Form2 : Form
    {
        public bool nappi = false;

        public Form2()
        {
            InitializeComponent();

        }

        public void Form2_Load(object sender, EventArgs e)
        {

        }
        
        public BindingList<Nimike> Blista;  //Bindinglista johon tallennetaan Nimike ja Yksikkö
        public void InitializeNimike()
        {
            Blista = new BindingList<Nimike>(); // esitellään bindinglist
            Blista.AddingNew += new AddingNewEventHandler(Blista_lisays); //Lisäys eventti
            Blista.AllowNew = true;
            Blista.AllowEdit = true;
            Blista.AllowRemove = true;
            Blista.RaiseListChangedEvents = true;
            Blista.Add(new Nimike("Kokki", "Keittiö")); //Lisätään nämä listaan
            Blista.Add(new Nimike("Siivooja", "Huolto"));
            Blista.Add(new Nimike("Esimies", "Päällystö"));
            Blista.Add(new Nimike("Asiakaspalvelija", "Respa"));
           
        }
        //Lisää uuden Nimikkeen ja Yksikön listaan
        void Blista_lisays(object sender, AddingNewEventArgs e) //Lisää tiedot listaan
        {
            e.NewObject = new Nimike(txbUusNimike.Text, txbUusiYksikko.Text);
            
        }

        public bool tarkistanum; //bool tarkistaa onko Nimike-kentässä numeroita
        public void btnF2Tallenna_Click(object sender, EventArgs e)
        {
           
            tarkistanum = txbUusNimike.Text.Any(Char.IsDigit); // Tarkistetaan onko kentässä numeroita
            Nimike uusiNimike = Blista.AddNew();
            if (tarkistanum == true)
            {
                //Jos kentässä numeroita, poistetaan se listasta.
                Blista.CancelNew(Blista.IndexOf(uusiNimike));
                MessageBox.Show("Nimikkeessä ei voi olla numeroita!");
                txbUusNimike.Clear();
                ActiveControl = txbUusNimike; // Aseta kontrolli tekstiboksiin.
            }
            else
            {
                //Kun tiedot lisätään listaan
                MessageBox.Show("Tiedot lisätty!");
                Close();
            }

        }
        // Suljetaan lomake
        private void btnF2Peruuta_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
