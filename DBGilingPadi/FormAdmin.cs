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
    public partial class FormAdmin : Form
    {

        string connectionString = @"Data Source=DESKTOP-GU8JFSR\ZIDANE; Initial Catalog=DBGilingPadi; Integrated Security=True";
        SqlConnection conn;

        BindingSource bs =
          new BindingSource();

        public FormAdmin()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void FormAdmin_Load(object sender, EventArgs e)
        {
            // Mengisi pilihan di ComboBox Status saat aplikasi dibuka
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Buka");
            cmbStatus.Items.Add("Tutup");
            cmbStatus.SelectedIndex = 0;

            dgvGiling.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvGiling.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvGiling.MultiSelect = false;

            dgvGiling.ReadOnly = true;

            dgvGiling.AllowUserToAddRows = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    MessageBox.Show("Koneksi ke Database Berhasil!", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi Gagal: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM vw_Penggilingan";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();

                da.Fill(dt);

                // BINDING
                bs.DataSource = dt;

                dgvGiling.DataSource = bs;

                // BINDING NAVIGATOR
                bindingNavigator1.BindingSource = bs;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Memuat Data: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (txtNama.Text == "" ||
            txtWilayah.Text == "" ||
            txtAlamat.Text == "")
            {
                MessageBox.Show(
                    "Semua data wajib diisi!");

                return;
            }

            try
            {

                conn.Open();
                SqlCommand cmd =
                new SqlCommand(
                "sp_InsertPenggilingan",
                conn);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@wilayah", txtWilayah.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Data Berhasil Disimpan!");
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Simpan: " + ex.Message);
            }
            finally
            {
                conn.Close();
                btnLoad_Click(null, null); // Refresh tabel otomatis
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtNama.Text == "" ||
            txtWilayah.Text == "" ||
            txtAlamat.Text == "")
            {
                MessageBox.Show(
                    "Semua data wajib diisi!");

                return;
            }

            try
            {
                if (dgvGiling.SelectedRows.Count > 0)
                {
                    conn.Open();
                    string id = dgvGiling.SelectedRows[0].Cells["ID_Giling"].Value.ToString();
                    SqlCommand cmd =
                    new SqlCommand(
                    "sp_UpdatePenggilingan",
                    conn);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                    cmd.Parameters.AddWithValue("@wilayah", txtWilayah.Text);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Berhasil Diperbarui!");
                    ClearForm();
                }
                else { MessageBox.Show("Pilih baris di tabel dulu!"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Update: " + ex.Message);
            }
            finally
            {
                conn.Close();
                btnLoad_Click(null, null);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGiling.SelectedRows.Count > 0)
                {
                    string id = dgvGiling.SelectedRows[0].Cells["ID_Giling"].Value.ToString();
                    if (MessageBox.Show("Hapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        conn.Open();
                        SqlCommand cmd =
                        new SqlCommand(
                        "sp_DeletePenggilingan",
                        conn);

                        cmd.CommandType =
                            CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue(
                            "@id",
                            id);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data Terhapus!");
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Hapus: " + ex.Message);
            }
            finally
            {
                conn.Close();
                btnLoad_Click(null, null);
            }
        }

        private void gg(object sender, EventArgs e)
        {

        }

        private void dgvGiling_CellClick(
        object sender,
        DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row =
                    dgvGiling.Rows[e.RowIndex];

                txtNama.Text =
                    row.Cells["NamaTempat"].Value.ToString();

                txtWilayah.Text =
                    row.Cells["Wilayah"].Value.ToString();

                txtAlamat.Text =
                    row.Cells["Alamat"].Value.ToString();

                cmbStatus.Text =
                    row.Cells["StatusOperasional"]
                    .Value.ToString();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            FormLogin login =
            new FormLogin();

            login.Show();

            this.Hide();
        }

        private void ClearForm()
        {
            txtNama.Clear();
            txtWilayah.Clear();
            txtAlamat.Clear();
            cmbStatus.SelectedIndex = 0;
        }

        private void bindingNavigatorPositionItem_Click(object sender, EventArgs e)
        {

        }
    }
}

