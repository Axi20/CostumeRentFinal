using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.IO;

namespace leltar
{

    public partial class Form1 : Form
    {
        #region Adatok az adatbázisból
        string uname = "";
        string passw = "";
        string jelmezMeret = "";
        //string jelmezFromBox;
        List<string> jelmeznevek = new List<string>();
        List<string> jelmeznevek2 = new List<string>();
        List<string> ugyfelEmailList = new List<string>();
        List<int> jelmezIdList = new List<int>();
        List<int> ugyfelIdList = new List<int>();
        List<int> kolcsonzesIdList = new List<int>();
        int id = 0;
        int napi_ar = 0;
        string date = DateTime.Now.ToString();
        #endregion;

        #region Számla - adatok tárolása
        //Számla adatok tárolása
        //------------------------------------------------
        string idopont = "";
        int napokSzama = 0;
        int fizetendo = 0;
        string keszpenz = "";
        int ugyfelFK = 0;
        int jelmezFK = 0;
        string keresztnev = "";
        string vezeteknev = "";
        string iranyitoszam = "";
        string varos = "";
        string utca = "";
        string hazszam = "";
        string adoszam = "";
        string email = "";
        string meret = "";
        //------------------------------------------------
        #endregion;

        #region Számla - jelmez adatok
        //Számla adatok
        //------------------------------------------------
        string szamlaJelmezNev;
        string szamlaJelmezMeret;
        string szamlaJelmezNapiAr;
        //------------------------------------------------
        #endregion;

        public Form1()
        {
            InitializeComponent();
            DateTime now = DateTime.Now;
            label10.Text = now.Year.ToString();
            switch (now.Month)
            {
                case 1: if (now.Month == 1) { label11.Text = "Január"; } break;
                case 2: if (now.Month == 2) { label11.Text = "Február"; } break;
                case 3: if (now.Month == 3) { label11.Text = "Március"; } break;
                case 4: if (now.Month == 4) { label11.Text = "Április"; } break;
                case 5: if (now.Month == 5) { label11.Text = "Május"; } break;
                case 6: if (now.Month == 6) { label11.Text = "Június"; } break;
                case 7: if (now.Month == 7) { label11.Text = "Július"; } break;
                case 8: if (now.Month == 8) { label11.Text = "Augusztus"; } break;
                case 9: if (now.Month == 9) { label11.Text = "Szeptember"; } break;
                case 10: if (now.Month == 10) { label11.Text = "Október"; } break;
                case 11: if (now.Month == 11) { label11.Text = "November"; } break;
                case 12: if (now.Month == 12) { label11.Text = "December"; } break;
            }
            label12.Text = now.Day.ToString();
            label13.Text = now.DayOfWeek.ToString();
            #region HetekNapjai;
            if (now.DayOfWeek.ToString() == "Monday") { label13.Text = "Hétfő"; }
            if (now.DayOfWeek.ToString() == "Tuesday") { label13.Text = "Kedd"; }
            if (now.DayOfWeek.ToString() == "Wednesday") { label13.Text = "Szerda"; }
            if (now.DayOfWeek.ToString() == "Thursday") { label13.Text = "Csütörtök"; }
            if (now.DayOfWeek.ToString() == "Friday") { label13.Text = "Péntek"; }
            if (now.DayOfWeek.ToString() == "Saturday") { label13.Text = "Szombat"; }
            if (now.DayOfWeek.ToString() == "Sunday") { label13.Text = "Vasárnap"; }
            #endregion;
            GetJelmezNevek();
            for (int i = 0; i < jelmeznevek2.Count; i++) { comboBox5.Items.Add(jelmeznevek2[i]); }
            tabPage1.Show();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            SetTextBoxColor();
            SetTextBoxText();
            DisableButtons();
            GetJelmezID2();
            GetUgyfelID();
            GetKolcsonzesID();
            GetUgyfelEmail();
        }

        #region Adatbázis csatlakozás;
        //ADATBÁZIS CSATLAKOZÁS
        //------------------------------------------------

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();

        private SQLiteConnection sql_con2;
        private SQLiteCommand sql_cmd2;
        private SQLiteDataAdapter DB2;
        private DataSet DS2 = new DataSet();
        private DataTable DT2 = new DataTable();

        private SQLiteConnection sql_con3;
        private SQLiteCommand sql_cmd3;
        private SQLiteDataAdapter DB3;
        private DataSet DS3 = new DataSet();
        private DataTable DT3 = new DataTable();

        private void SetConnection()
        {
            sql_con = new SQLiteConnection("Data Source=leltar.db;Version=3;New=False;Compress=True;");
        }
        private void Connect()
        {
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
        }
        private void ExecuteQuery(string txtQuery)
        {
            Connect();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }
        //------------------------------------------------
        #endregion;

        #region Menü gombok;
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            string textQuery = "select * from jelmez";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        } //Leltár button
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        } //Hozzáadás button
        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage4;
        } //Módosítás button
        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage5;
            GetJelmezNevek();
            //for (int i = 0; i < jelmeznevek2.Count; i++) { comboBox5.Items.Add(jelmeznevek2[i]); }
        } //Számla button
        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage6;
        } //Törlés button
        private void button12_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage7;
        } //Adatok button
        private void button6_Click(object sender, EventArgs e)
        {
            //Párbeszéd ablak - igen válasz esetén kilép
            if (MessageBox.Show("Biztos, hogy ki akarsz lépni?", "Kilépés", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }  //Kilépés button
        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage8;
            /*for (int i = 1; i < ugyfelEmailList.Count; i++)
            {
                if ((string)comboBox6.Items[i] != ugyfelEmailList[i])
                {
                    comboBox6.Items.Add(ugyfelEmailList[i].ToString());
                }
            }*/

            foreach (var item in ugyfelEmailList)
            {
                comboBox6.Items.Add(item);
            }

        } //Kapcsolat button

        #endregion;

        #region Login
        private void button9_Click(object sender, EventArgs e)
        {
            LoginUname();
            LoginPass();
            string textQuery = "select * from jelmez";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);

            if (textBox1.Text == uname && textBox2.Text == passw) { tabControl1.SelectedTab = tabPage2; EnableButtons(); }
            else { MessageBox.Show("Hibás bejelentkezési adatok!", "Hiba", MessageBoxButtons.OK); }
        }//Login OK button
        private void LoginUname()
        {
            string username = "select username from Login where ID=1";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = username;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { uname = dr.GetValue(0).ToString(); }
            dr.Close();
            cn.Close();
        }  //Login username query
        private void LoginPass()
        {
            string password = "select password from Login where ID=1";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = password;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { passw = dr.GetValue(0).ToString(); }
            dr.Close();
            cn.Close();
        }  //Login username query

        #endregion;

        #region Leltár - jelmezek szűrése
        private void button17_Click(object sender, EventArgs e)
        {
            string textQuery = "SELECT ID, Nev, Meret, NapiAr, Elerheto FROM jelmez";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        }//Reset button
        private void button10_Click(object sender, EventArgs e)
        {
            Connect();
            string CommandText = "SELECT Nev, Meret, NapiAr FROM jelmez WHERE Elerheto != 'false'";
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView1.DataSource = DT;
            sql_con.Close();
        }//Kölcsönözhető button
        private void button13_Click(object sender, EventArgs e)
        {
            Connect();
            string textQuery = "SELECT Nev, Meret, NapiAr FROM jelmez WHERE Meret='XS'";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        }//XS
        private void button14_Click(object sender, EventArgs e)
        {
            string textQuery = "SELECT Nev, Meret, NapiAr FROM jelmez WHERE Meret='S'";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        }//S
        private void button15_Click(object sender, EventArgs e)
        {
            string textQuery = "SELECT Nev, Meret, NapiAr FROM jelmez WHERE Meret='M'";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        }//M
        private void button16_Click(object sender, EventArgs e)
        {
            string textQuery = "SELECT Nev, Meret, NapiAr FROM jelmez WHERE Meret='L'";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        }//L
        private void button11_Click(object sender, EventArgs e)
        {
            string textQuery = $"SELECT Nev, Meret, NapiAr FROM jelmez WHERE Elerheto!='true'";
            ExecuteQuery(textQuery);
            JelmezQuery(textQuery);
        }//Foglalt jelmezek

        #endregion;

        #region Click event button18-28
        private void button21_Click(object sender, EventArgs e)
        {
            string elerheto = "";
            if (radioButton1.Checked == true) { elerheto = "true"; }
            if (radioButton2.Checked == true) { elerheto = "false"; }
            string textQuery = $"INSERT INTO jelmez (Nev, Meret, NapiAr, Elerheto) VALUES ('{textBox6.Text}', '{comboBox1.SelectedItem.ToString()}', '{int.Parse(textBox7.Text)}', '{elerheto}');";
            ExecuteQuery(textQuery);
            MessageBox.Show("Adat hozzáadva!", "Hozzáad", MessageBoxButtons.OK);
            comboBox1.ResetText();
            textBox6.Text = ""; textBox7.Text = ""; radioButton1.Checked = false; radioButton2.Checked = false; comboBox1.SelectedItem = " ";
        }//Új jelmez hozzáadása button
        private void button19_Click(object sender, EventArgs e)
        {
            if ((string)comboBox3.SelectedItem == "Név")
            {
                string textQuery = $"UPDATE jelmez SET Nev='{textBox12.Text}' WHERE ID='{textBox13.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < jelmezIdList.Count; i++)
                {

                    if (jelmezIdList.Contains(Convert.ToInt32(textBox13.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox12.Text = ""; textBox13.Text = ""; comboBox3.ResetText();
            }
            else if ((string)comboBox3.SelectedItem == "Méret")
            {
                string textQuery = $"UPDATE jelmez SET Meret='{textBox12.Text}' WHERE ID='{textBox13.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < jelmezIdList.Count; i++)
                {

                    if (jelmezIdList.Contains(Convert.ToInt32(textBox13.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox12.Text = ""; textBox13.Text = ""; comboBox3.ResetText();
            }
            else if ((string)comboBox3.SelectedItem == "Napi ár")
            {
                string textQuery = $"UPDATE jelmez SET NapiAr='{textBox12.Text}' WHERE ID='{textBox13.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < jelmezIdList.Count; i++)
                {

                    if (jelmezIdList.Contains(Convert.ToInt32(textBox13.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox12.Text = ""; textBox13.Text = ""; comboBox3.ResetText();
            }
            else if ((string)comboBox3.SelectedItem == "Elérhető")
            {
                string textQuery = $"UPDATE jelmez SET Elerheto='{textBox12.Text}' WHERE ID='{textBox13.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < jelmezIdList.Count; i++)
                {
                    if (jelmezIdList.Contains(Convert.ToInt32(textBox13.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox12.Text = ""; textBox13.Text = ""; comboBox3.ResetText();
            }
        }//Jelmez adatok módosítása
        private void button20_Click(object sender, EventArgs e)
        {
            if ((string)comboBox2.SelectedItem == "Vezetéknév")
            {
                string textQuery = $"UPDATE ugyfel SET Vezeteknev='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {

                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Keresztnév")
            {
                string textQuery = $"UPDATE ugyfel SET Keresztnev='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {

                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Irányítószám")
            {
                string textQuery = $"UPDATE ugyfel SET Iranyitoszam='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {
                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Város")
            {
                string textQuery = $"UPDATE ugyfel SET Varos='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {
                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Utca")
            {
                string textQuery = $"UPDATE ugyfel SET Utca='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {
                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Házszám")
            {
                string textQuery = $"UPDATE ugyfel SET Hazszam='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {
                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Adószám")
            {
                string textQuery = $"UPDATE ugyfel SET Adoszam='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {
                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
            else if ((string)comboBox2.SelectedItem == "Email")
            {
                string textQuery = $"UPDATE ugyfel SET Email='{textBox9.Text}' WHERE ID='{textBox8.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < ugyfelIdList.Count; i++)
                {
                    if (ugyfelIdList.Contains(Convert.ToInt32(textBox8.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox8.Text = ""; textBox9.Text = ""; comboBox2.ResetText();
            }
        }//Ügyfél adatok módosítása
        private void button18_Click(object sender, EventArgs e)
        {
            if ((string)comboBox4.SelectedItem == "Időpont")
            {
                string textQuery = $"UPDATE kolcsonzes SET Idopont='{textBox11.Text}' WHERE ID='{textBox10.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < kolcsonzesIdList.Count; i++)
                {
                    if (kolcsonzesIdList.Contains(Convert.ToInt32(textBox10.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox11.Text = ""; textBox10.Text = ""; comboBox4.ResetText();
            }
            else if ((string)comboBox4.SelectedItem == "Napok száma")
            {
                string textQuery = $"UPDATE kolcsonzes SET NapokSzama='{textBox11.Text}' WHERE ID='{textBox10.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < kolcsonzesIdList.Count; i++)
                {
                    if (kolcsonzesIdList.Contains(Convert.ToInt32(textBox10.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox11.Text = ""; textBox10.Text = ""; comboBox4.ResetText();
            }
            else if ((string)comboBox4.SelectedItem == "Fizetendő")
            {
                string textQuery = $"UPDATE kolcsonzes SET Fizetendo='{textBox11.Text}' WHERE ID='{textBox10.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < kolcsonzesIdList.Count; i++)
                {
                    if (kolcsonzesIdList.Contains(Convert.ToInt32(textBox10.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox11.Text = ""; textBox10.Text = ""; comboBox4.ResetText();
            }
            else if ((string)comboBox4.SelectedItem == "Készpénz")
            {
                string textQuery = $"UPDATE kolcsonzes SET Keszpenz='{textBox11.Text}' WHERE ID='{textBox10.Text}';";
                ExecuteQuery(textQuery);
                for (int i = 1; i < kolcsonzesIdList.Count; i++)
                {
                    if (kolcsonzesIdList.Contains(Convert.ToInt32(textBox10.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                    else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
                }
                textBox11.Text = ""; textBox10.Text = ""; comboBox4.ResetText();
            }
        }//Kölcsönzés adatok módosítása
        private void button22_Click(object sender, EventArgs e)
        {
            string textQuery = $"DELETE FROM jelmez WHERE ID='{textBox16.Text}';";
            ExecuteQuery(textQuery);
            for (int i = 1; i < jelmezIdList.Count; i++)
            {
                if (jelmezIdList.Contains(Convert.ToInt32(textBox16.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
            }
            textBox16.Text = "";
        }//Jelmez törlése
        private void button23_Click(object sender, EventArgs e)
        {
            string textQuery = $"DELETE FROM ugyfel WHERE ID='{textBox15.Text}';";
            string textQuery2 = $"DELETE FROM kolcsonzes WHERE ugyfel = '{textBox15.Text}';";
            ExecuteQuery(textQuery);
            ExecuteQuery(textQuery2);
            for (int i = 1; i < ugyfelIdList.Count; i++)
            {
                if (ugyfelIdList.Contains(Convert.ToInt32(textBox15.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
            }
            textBox15.Text = "";
        }//Ügyfél törlése
        private void button24_Click(object sender, EventArgs e)
        {
            string textQuery = $"DELETE FROM kolcsonzes WHERE ID='{textBox14.Text}';";
            ExecuteQuery(textQuery);
            for (int i = 1; i < kolcsonzesIdList.Count; i++)
            {
                if (kolcsonzesIdList.Contains(Convert.ToInt32(textBox14.Text)) == false) { MessageBox.Show("Ilyen azonosítóval rendelkező adat nem létezik az adatbázisban!", "Hiba", MessageBoxButtons.OK); break; }
                else { MessageBox.Show("Adat módosítva!", "Módosítás", MessageBoxButtons.OK); break; }
            }
            textBox14.Text = "";
        }//Kölcsönzés törlése
        private void button25_Click(object sender, EventArgs e)
        {
            string textQuery = "select * from Ugyfel";
            ExecuteQuery(textQuery);
            GetUgyfelAdatok(textQuery);
        }//Ügyfél adatok
        private void button26_Click(object sender, EventArgs e)
        {
            string textQuery = "select * from Kolcsonzes";
            ExecuteQuery(textQuery);
            GetKolcsonzesAdatok(textQuery);
        }//Kölcsönzés adatok
        private void button27_Click(object sender, EventArgs e)
        {
            string textQuery = $"INSERT INTO ugyfel (Vezeteknev, Keresztnev, Iranyitoszam, Varos, Utca, Hazszam, Adoszam, Email) VALUES ('{textBox18.Text}', '{textBox19.Text}', '{textBox20.Text}', '{textBox21.Text}', '{textBox22.Text}', '{textBox23.Text}', '{textBox24.Text}', '{textBox25.Text}');";
            ExecuteQuery(textQuery);
            comboBox5.ResetText();
            textBox18.Text = ""; textBox19.Text = ""; textBox20.Text = ""; textBox21.Text = ""; textBox22.Text = ""; textBox23.Text = ""; textBox24.Text = ""; textBox25.Text = ""; textBox26.Text = ""; textBox17.Text = "";
            napokSzama = (int)numericUpDown1.Value;
            fizetendo = napokSzama * napi_ar;
            DateTime date = DateTime.Now;
            string ev = date.Year.ToString();
            string honap = date.Month.ToString();
            string nap = date.Day.ToString();
            idopont = $"{ev}.{honap}.{nap}";
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int elteltIdo = DateTime.Now.Year * 365 - year * 365 + DateTime.Now.Month * 30 - month * 30 + DateTime.Now.Day - day;
            int idoHatra = Math.Abs(elteltIdo - napokSzama);

            if (radioButton4.Checked == true) { keszpenz = "true"; }
            if (radioButton3.Checked == true) { keszpenz = "false"; }
            GetUgyfelFKey();
            GetJelmezFKey();
            string textQuery2 = $" INSERT INTO kolcsonzes (Ugyfel, Jelmez, Idopont, NapokSzama, Fizetendo, Keszpenz, HatralevoNapok) VALUES ('{ugyfelFK}', '{jelmezFK}', '{idopont}', '{napokSzama}', '{fizetendo}', '{keszpenz}', '{idoHatra}');";
            ExecuteQuery(textQuery2);

            string textQuery3 = $"UPDATE jelmez SET Elerheto='false' WHERE ID='{jelmezFK}';";
            ExecuteQuery(textQuery3);
            MessageBox.Show("Számla elkészült!", "Számla", MessageBoxButtons.OK);
            SzamlaTxt();

        }//Számla készítése
        private void button28_Click(object sender, EventArgs e)
        {
            //-------------- lwwdlgiffhfysbzx --------------------

            string subject = textBox27.Text;
            string message = richTextBox1.Text;
            string recipient = comboBox6.SelectedItem.ToString();
            string from = "costome.rent22@gmail.com";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("costume.rent22@gmail.com", "lwwdlgiffhfysbzx"),
                EnableSsl = true
            };
            client.EnableSsl = true;
            client.Send(from, recipient, subject, message);

            MessageBox.Show("Üzenet elküldve!", "Email küldés", MessageBoxButtons.OK);
            comboBox6.ResetText();
            richTextBox1.ResetText();
            textBox27.ResetText();

        } //Email küldés
        private void button8_Click(object sender, EventArgs e)
        {
            vezeteknev = textBox18.Text;
            keresztnev = textBox19.Text;
            iranyitoszam = textBox20.Text;
            varos = textBox21.Text;
            utca = textBox22.Text;
            hazszam = textBox23.Text;
            adoszam = textBox24.Text;
            email = textBox25.Text;
            meret = textBox26.Text;
            MessageBox.Show("Adatok elmentve!", "Mentés", MessageBoxButtons.OK);
        } //Adatok mentése button

        #endregion;

        #region Metódusok
        private void JelmezQuery(string textQuery)
        {
            Connect();
            string CommandText = textQuery;
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            dataGridView1.DataSource = DT;
            sql_con.Close();
        }
        private void GetUgyfelAdatok(string textQuery)
        {
            sql_con2 = new SQLiteConnection("Data Source=leltar.db;Version=3;New=False;Compress=True;");
            sql_con2.Open();
            sql_cmd2 = sql_con2.CreateCommand();
            string CommandText = textQuery;
            DB2 = new SQLiteDataAdapter(CommandText, sql_con2);
            DS2.Reset();
            DB2.Fill(DS2);
            DT2 = DS2.Tables[0];
            dataGridView3.DataSource = DT2;
            sql_con2.Close();
        }
        private void GetKolcsonzesAdatok(string textQuery)
        {
            sql_con3 = new SQLiteConnection("Data Source=leltar.db;Version=3;New=False;Compress=True;");
            sql_con3.Open();
            sql_cmd3 = sql_con3.CreateCommand();
            string CommandText = textQuery;
            DB3 = new SQLiteDataAdapter(CommandText, sql_con3);
            DS3.Reset();
            DB3.Fill(DS3);
            DT3 = DS3.Tables[0];
            dataGridView4.DataSource = DT3;
            sql_con3.Close();
        }
        private void GetJelmezNevek()
        {
            string jelmeznevekQuery = "select Nev from jelmez where Elerheto = 'true'";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = jelmeznevekQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { jelmeznevek2.Add(dr.GetValue(0).ToString()); }
            dr.Close();
            cn.Close();
        }
        private void GetJelmezID()
        {
            string jelmez = comboBox5.SelectedItem.ToString();
            string jelmezmeretQuery = $"select ID from jelmez where Nev = '{jelmez}'";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = jelmezmeretQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { id = Convert.ToInt32(dr.GetValue(0)); }
            dr.Close();
            cn.Close();
        }
        private void GetJelmezMeretFromID()
        {
            string jelmezMeretQuery = $"select Meret from jelmez where ID = '{id}'";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = jelmezMeretQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { jelmezMeret = dr.GetValue(0).ToString(); }
            dr.Close();
            cn.Close();
        }
        private void GetJelmezArFromID()
        {
            string jelmezArQuery = $"select NapiAr from jelmez where ID = '{id}'";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = jelmezArQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { napi_ar = Convert.ToInt32(dr.GetValue(0)); }
            dr.Close();
            cn.Close();
        }
        private void GetUgyfelFKey()
        {
            string textQuery = $"select max(ID) from ugyfel";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = textQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { ugyfelFK = Convert.ToInt32(dr.GetValue(0)); }
            dr.Close();
            cn.Close();
        }
        private void GetJelmezFKey()
        {
            string textQuery = $"select max(ID) from jelmez where Nev = '{comboBox5.SelectedItem}'";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = textQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { jelmezFK = Convert.ToInt32(dr.GetValue(0)); }
            dr.Close();
            cn.Close();
        }
        private void GetUgyfelEmail()
        {
            string ugyfelEmailQuery = "select Email from ugyfel";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = ugyfelEmailQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { ugyfelEmailList.Add(dr.GetValue(0).ToString()); }
            dr.Close();
            cn.Close();
        }
        private void SetTextBoxColor()
        {
            textBox18.ForeColor = Color.Gray; textBox19.ForeColor = Color.Gray; textBox20.ForeColor = Color.Gray;
            textBox21.ForeColor = Color.Gray; textBox22.ForeColor = Color.Gray; textBox23.ForeColor = Color.Gray;
            textBox24.ForeColor = Color.Gray; textBox25.ForeColor = Color.Gray;
        }
        private void SetTextBoxText()
        {
            textBox18.Text = "Vezetéknév"; textBox19.Text = "Keresztnév"; textBox20.Text = "Irányítószám";
            textBox21.Text = "Város"; textBox22.Text = "Utca"; textBox23.Text = "Ház szám";
            textBox24.Text = "00000000-0-00"; textBox25.Text = "pelda@gmail.com";
        }
        private void DisableButtons()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button12.Enabled = false;
        }
        private void EnableButtons()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button12.Enabled = true;
        }
        private void GetJelmezID2()
        {
            string jelmezIDQuery = "select ID from jelmez";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = jelmezIDQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { jelmezIdList.Add(Convert.ToInt32(dr.GetValue(0))); }
            dr.Close();
            cn.Close();
        }
        private void GetUgyfelID()
        {
            string ugyfelIDQuery = "select ID from ugyfel";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = ugyfelIDQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { ugyfelIdList.Add(Convert.ToInt32(dr.GetValue(0))); }
            dr.Close();
            cn.Close();
        }
        private void GetKolcsonzesID()
        {
            string kolcsonzesIDQuery = "SELECT ID FROM kolcsonzes";
            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            cn.ConnectionString = "Data Source=leltar.db;Version=3;New=False;Compress=True;";
            cmd.Connection = cn;
            cmd.Connection.Open();
            cmd.CommandText = kolcsonzesIDQuery;
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) { kolcsonzesIdList.Add(Convert.ToInt32(dr.GetValue(0))); }
            dr.Close();
            cn.Close();
        }
        private void SzamlaTxt()
        {
            string fizetesiMod = "";
            if (radioButton4.Checked == true) { fizetesiMod = "Készpénz"; }
            if (radioButton3.Checked == true) { fizetesiMod = "Bankkártya"; }
            StreamWriter sw = new StreamWriter("Szamla.txt", true);
            sw.WriteLine("---------------------------------------");
            sw.WriteLine("                 SZÁMLA                ");
            sw.WriteLine("---------------------------------------");
            sw.WriteLine();
            sw.WriteLine("Vásárló adatai");
            sw.WriteLine();
            sw.WriteLine($"Név: {vezeteknev} {keresztnev}");
            sw.WriteLine($"Cím: {iranyitoszam} {varos} {utca} {hazszam}");
            sw.WriteLine($"Adószám: {adoszam}");
            sw.WriteLine($"Email: {email}");
            sw.WriteLine();
            sw.WriteLine("Jelmez adatai");
            sw.WriteLine();
            sw.WriteLine($"Név: {comboBox5.SelectedItem.ToString()}");
            sw.WriteLine($"Méret: {meret}");
            sw.WriteLine($"Napi ár: {napi_ar}");
            sw.WriteLine();
            sw.WriteLine("Kölcsönzés adatai");
            sw.WriteLine();
            sw.WriteLine($"Időpont: {idopont}");
            sw.WriteLine($"Napok száma: {napokSzama}");
            sw.WriteLine($"Fizetendő: {fizetendo}");
            sw.WriteLine($"Fizetési mód: {fizetesiMod}");
            sw.WriteLine();
            sw.WriteLine("---------------------------------------");
            sw.WriteLine($"Számla kelte: {date}");
            sw.WriteLine("---------------------------------------");
            sw.WriteLine(); sw.WriteLine();
            sw.Flush();
            sw.Close();

        }
        #endregion;

        #region Timer, combobox5 event
        private void timer1_Tick(object sender, EventArgs e)
        {
            label22.Text = DateTime.Now.ToString("hh:mm:ss");
        }
        private void comboBox5_SelectedValueChanged(object sender, EventArgs e)
        {
            GetJelmezID();
            GetJelmezMeretFromID();
            GetJelmezArFromID();
            textBox26.Text = jelmezMeret;
            textBox17.Text = napi_ar.ToString();
            szamlaJelmezNev = comboBox5.SelectedItem.ToString();
            szamlaJelmezMeret = textBox26.Text;
            szamlaJelmezNapiAr = textBox17.Text;
        }//Combobox5
        #endregion;

        #region TextBox Eventek 18-25
        private void textBox18_Enter(object sender, EventArgs e)
        {
            if (textBox18.Text == "Vezetéknév") { textBox18.Text = ""; }
            textBox18.ForeColor = Color.Black;
        }
        private void textBox18_Leave(object sender, EventArgs e)
        {
            if (textBox18.Text == "") { textBox18.Text = "Vezetéknév"; textBox18.ForeColor = Color.Gray; }
        }
        private void textBox19_Enter(object sender, EventArgs e)
        {
            if (textBox19.Text == "Keresztnév") { textBox19.Text = ""; }
            textBox19.ForeColor = Color.Black;
        }
        private void textBox19_Leave(object sender, EventArgs e)
        {
            if (textBox19.Text == "") { textBox19.Text = "Keresztnév"; textBox19.ForeColor = Color.Gray; }
        }
        private void textBox20_Enter(object sender, EventArgs e)
        {
            if (textBox20.Text == "Irányítószám") { textBox20.Text = ""; }
            textBox20.ForeColor = Color.Black;
        }
        private void textBox20_Leave(object sender, EventArgs e)
        {
            if (textBox20.Text == "") { textBox20.Text = "Irányítószám"; textBox20.ForeColor = Color.Gray; }
        }
        private void textBox21_Enter(object sender, EventArgs e)
        {
            if (textBox21.Text == "Város") { textBox21.Text = ""; }
            textBox21.ForeColor = Color.Black;
        }
        private void textBox21_Leave(object sender, EventArgs e)
        {
            if (textBox21.Text == "") { textBox21.Text = "Város"; textBox21.ForeColor = Color.Gray; }
        }
        private void textBox22_Enter(object sender, EventArgs e)
        {
            if (textBox22.Text == "Utca") { textBox22.Text = ""; }
            textBox22.ForeColor = Color.Black;
        }
        private void textBox22_Leave(object sender, EventArgs e)
        {
            if (textBox22.Text == "") { textBox22.Text = "Utca"; textBox22.ForeColor = Color.Gray; }
        }
        private void textBox23_Enter(object sender, EventArgs e)
        {
            if (textBox23.Text == "Házszám") { textBox23.Text = ""; }
            textBox23.ForeColor = Color.Black;
        }
        private void textBox23_Leave(object sender, EventArgs e)
        {
            if (textBox23.Text == "") { textBox23.Text = "Házszám"; textBox23.ForeColor = Color.Gray; }
        }
        private void textBox24_Enter(object sender, EventArgs e)
        {
            if (textBox24.Text == "00000000-0-00") { textBox24.Text = ""; }
            textBox24.ForeColor = Color.Black;
        }
        private void textBox24_Leave(object sender, EventArgs e)
        {
            if (textBox24.Text == "") { textBox24.Text = "00000000-0-00"; textBox24.ForeColor = Color.Gray; }
        }
        private void textBox25_Enter(object sender, EventArgs e)
        {
            if (textBox25.Text == "pelda@gmail.com") { textBox25.Text = ""; }
            textBox25.ForeColor = Color.Black;
        }
        private void textBox25_Leave(object sender, EventArgs e)
        {
            if (textBox25.Text == "") { textBox25.Text = "pelda@gmail.com"; textBox25.ForeColor = Color.Gray; }
        }
        private void textBox23_Click(object sender, EventArgs e)
        {
            textBox23.Text = "";
        }

        #endregion;
    }
}
