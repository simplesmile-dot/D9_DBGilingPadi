using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBGilingPadi
{
    public partial class FormUser : Form
    {
        // ==============================
        // CONNECTION STRING
        // ==============================
        string connectionString =
            @"Data Source=DESKTOP-GU8JFSR\ZIDANE;
            Initial Catalog=DBGilingPadi;
            Integrated Security=True";

        // ==============================
        // CONSTRUCTOR
        // ==============================
        public FormUser()
        {
            InitializeComponent();
        }

        // ==============================
        // FORM LOAD
        // ==============================
        private void FormUser_Load(object sender, EventArgs e)
        {

            dgvGiling.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvGiling.MultiSelect = false;

            dgvGiling.ReadOnly = true;

            dgvGiling.AllowUserToAddRows = false;

            // Isi ComboBox Filter
            cmbFilterStatus.Items.Clear();

            cmbFilterStatus.Items.Add("Semua");
            cmbFilterStatus.Items.Add("Buka");
            cmbFilterStatus.Items.Add("Tutup");

            cmbFilterStatus.SelectedIndex = 0;

            // Load Data
            LoadData();
        }

        // ==============================
        // LOAD DATA
        // ==============================
        private void LoadData()
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query =
                        "SELECT * FROM vw_Penggilingan";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, conn);

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvGiling.DataSource = dt;

                    HitungJumlahBuka(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Gagal memuat data : " + ex.Message);
            }
        }

        // ==============================
        // HITUNG STATUS BUKA
        // ==============================
        private void HitungJumlahBuka(DataTable dt)
        {
            int jumlah = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["StatusOperasional"]
                    .ToString() == "Buka")
                {
                    jumlah++;
                }
            }

            lblJumlahBuka.Text =
                "Jumlah Penggilingan Buka : " +
                jumlah.ToString();
        }

        // ==============================
        // SEARCH DATA
        // ==============================
        private void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlDataAdapter da =
                    new SqlDataAdapter(
                     "sp_SearchPenggilingan",
                      conn);

                    da.SelectCommand.CommandType =
                        CommandType.StoredProcedure;

                    da.SelectCommand.Parameters
                        .AddWithValue(
                        "@wilayah",
                        txtCari.Text);

                    da.SelectCommand.Parameters
                        .AddWithValue(
                        "@status",
                        cmbFilterStatus.Text);


                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvGiling.DataSource = dt;

                    HitungJumlahBuka(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "Data tidak ditemukan!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Pencarian gagal : " + ex.Message);
            }
        }

        // ==============================
        // REFRESH DATA
        // ==============================
        private void btnRefresh_Click(
            object sender,
            EventArgs e)
        {
            txtCari.Clear();

            cmbFilterStatus.SelectedIndex = 0;

            LoadData();
        }

        // ==============================
        // LOGOUT.
        // ==============================
        private void btnLogout_Click(
            object sender,
            EventArgs e)
        {
            DialogResult hasil =
                MessageBox.Show(
                    "Yakin ingin logout?",
                    "Konfirmasi Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (hasil == DialogResult.Yes)
            {
                FormLogin login =
                    new FormLogin();

                login.Show();

                this.Hide();
            }
        }

        // ==============================
        // FILTER STATUS LANGSUNG
        // ==============================
        private void cmbFilterStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            btnSearch.PerformClick();
        }
    }
}
