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
    public partial class FormRekap : Form
    {
        // 5. Deklarasi objek database (Murni Modul Poin 5)
        static string connectionString = @"Data Source=DESKTOP-GU8JFSR\ZIDANE; Initial Catalog=DBGilingPadi; Integrated Security=True";
        SqlConnection conn = new SqlConnection(connectionString); // Pastikan ini ada new-nya
        SqlDataAdapter da;
        DataTable dtPenggilingan;

        public FormRekap()
        {
            InitializeComponent();
        }

        // 6. Event Form Load (Murni Modul Poin 6)
        private void FormRekap_Load(object sender, EventArgs e)
        {
            btnCetak.Enabled = false; // Tombol cetak mati sebelum data di-load
        }

        // 7. Event Tombol Load Click (Murni Modul Poin 7)
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_ReportPenggilingan", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Mengirim parameter ke Stored Procedure sesuai input form
                cmd.Parameters.Add("@inWilayah", SqlDbType.VarChar, 50).Value = txtWilayah.Text;
                cmd.Parameters.Add("@inStatus", SqlDbType.VarChar, 20).Value = cmbStatus.Text;

                da = new SqlDataAdapter(cmd);
                dtPenggilingan = new DataTable();
                da.Fill(dtPenggilingan);

                dataGridView1.DataSource = dtPenggilingan;

                if (dtPenggilingan.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load data: " + ex.Message);
            }
        }

        // 14. Event Tombol Cetak Click (Murni Modul Poin 14)
        private void btnCetak_Click(object sender, EventArgs e)
        {
            // Membuka Form Cetak Report sambil melempar parameter data inputan form (Modul Poin 14)
            FormCetak frmCetak = new FormCetak(txtWilayah.Text, cmbStatus.Text);
            frmCetak.Show();
            this.Hide();
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            // Menampilkan kembali FormAdmin
            FormAdmin frmAdmin = new FormAdmin();
            frmAdmin.Show();

            // Menutup FormRekap ini
            this.Close();
        }
    }
}
