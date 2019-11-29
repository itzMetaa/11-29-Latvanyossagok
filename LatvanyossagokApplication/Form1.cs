using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LatvanyossagokApplication
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;

        public Form1()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost;Port=3307;Database=latvanyossagokdb;Uid=root;Pwd=;");
            conn.Open();
            VarosListazas();
        }

        void VarosListazas()
        {
            listBoxVarosok.Items.Clear();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, nev, lakossag FROM varosok ORDER BY nev";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("id");
                    var nev = reader.GetString("nev");
                    var lakossag = reader.GetInt32("lakossag");
                    var varos = new Varos(id, nev, lakossag);

                    listBoxVarosok.Items.Add(varos);
                }
            }
        }

        private void buttonVarosFelvetel_Click(object sender, EventArgs e)
        {                                                  
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO varosok (nev,lakossag) VALUES(@nev, @lakossag)";
            cmd.Parameters.AddWithValue("@nev", textBoxVarosNev.Text);
            cmd.Parameters.AddWithValue("@lakossag", numericUpDownVarosLakossag.Value);
            cmd.ExecuteNonQuery();

            textBoxVarosNev.Text = "";
            VarosListazas();
        }

        private void buttonLatvanyossagFelvetel_Click(object sender, EventArgs e)
        {
            var varos = (Varos)(listBoxVarosok.SelectedItem);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO latvanyossagok (cim,kiadas,hossz,rendezo_id) VALUES(@cim, @kiadas, @hossz, @rendezo_id)";
            cmd.Parameters.AddWithValue("@nev", textBoxLatvanyossagNev.Text);
            cmd.Parameters.AddWithValue("@leiras", textBoxLatvanyossagLeiras.Text);
            cmd.Parameters.AddWithValue("@ar", numericUpDownLatvanyossagAr.Value);
            cmd.Parameters.AddWithValue("@varos_id", varos.Id);
            cmd.ExecuteNonQuery();

            textBoxLatvanyossagNev.Text = "";
            textBoxLatvanyossagLeiras.Text = "";
            numericUpDownLatvanyossagAr.Value = 1000;

            LatvanyossagListazas();
        }

        private void buttonVarosTorles_Click(object sender, EventArgs e)
        {
            var varos = (Varos)(listBoxVarosok.SelectedItem);
            if (listBoxVarosok.SelectedIndex == -1)
            {
                MessageBox.Show("o szia eHelo nemjio nincs kivlaszltva:D:)");
                return;
            }
            int varos_id = listBoxVarosok.SelectedIndex;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM varosok WHERE id = @varos_id";
            cmd.Parameters.AddWithValue("@varos_id", varos.Id);
            cmd.ExecuteNonQuery();
            VarosListazas();
        }

        void LatvanyossagListazas()
        {
            listBoxLatvanyossagok.Items.Clear();
            var varos = (Varos)(listBoxVarosok.SelectedItem);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nev, leiras, ar, varos_id 
                                FROM latvanyossagok
                                WHERE varos_id like @varos_id
                                ORDER BY cim";
            cmd.Parameters.AddWithValue("@varos_id", varos.Id);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("id");
                    var nev = reader.GetString("nev");
                    var leiras = reader.GetString("leiras");
                    var ar = reader.GetInt32("ar");
                    var varos_id = reader.GetInt32("varos_id");
                    var latvanyossag = new Latvanyossag(id, nev, leiras, ar, varos_id);

                    listBoxLatvanyossagok.Items.Add(latvanyossag);
                }
            }
        }
    }

}
