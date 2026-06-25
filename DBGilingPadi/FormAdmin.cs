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

        string connectionString = @"Data Source=DESKTOP-GU8JFSR\ZIDANE; Initial Catalog=DBGilingPadi; User ID=sa; Password=apasaja;";
        SqlConnection conn;

        BindingSource bs =
          new BindingSource();

        public FormAdmin()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void SimpanLog(string pesan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO LogError (pesan_error) VALUES (@pesan)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", pesan);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void FormAdmin_Load(object sender, EventArgs e)
        {
            // Mengisi pilihan di ComboBox Status saat aplikasi dibuka.
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

        // Event handler untuk tombol Connect
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
            // ADAPTASI MODUL: Try-Catch Bertingkat pada Load Data
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Gagal Memuat Data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
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

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_InsertPenggilingan", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@wilayah", txtWilayah.Text);
                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                byte[] fotoBiner = ConvertImageToByteArray(pbFoto.Image); // Menggunakan pbFoto sesuai nama PictureBox-mu
                if (fotoBiner != null)
                {
                    cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = fotoBiner;
                }
                else
                {
                    cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = DBNull.Value;
                }

                cmd.ExecuteNonQuery();

                SqlCommand cmdLog = new SqlCommand(
                    "INSERT INTO LogAktivitasSalah (aktivitas, waktu) VALUES (@aktivitas, GETDATE())",
                    conn,
                    trans
                );

                cmdLog.Parameters.AddWithValue("@aktivitas", "TAMBAH PENGGILINGAN: " + txtNama.Text);
                cmdLog.ExecuteNonQuery();

                trans.Commit();

                MessageBox.Show("Data Berhasil Disimpan!");
                ClearForm();
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                SimpanLog("BATAL TAMBAH DATA: " + ex.Message);
                MessageBox.Show("SQL Error Simpan: " + ex.Message, "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("BATAL TAMBAH DATA: " + ex.Message);
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

                    byte[] fotoBiner = ConvertImageToByteArray(pbFoto.Image);
                    if (fotoBiner != null)
                    {
                        cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = fotoBiner;
                    }
                    else
                    {
                        cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = DBNull.Value;
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Berhasil Diperbarui!");
                    ClearForm();
                }
                else { MessageBox.Show("Pilih baris di tabel dulu!"); }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message); // Simpan ke tabel LogError jika kena trigger proteksi massal
                MessageBox.Show("SQL Error Update: " + ex.Message, "Keamanan / Database Terganggu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
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

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data Terhapus!");
                        ClearForm();
                    }
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message); // Simpan ke tabel LogError
                MessageBox.Show("SQL Error Hapus: " + ex.Message, "Kesalahan Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
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

        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Penggilingan SET NamaTempat = '" + txtNama.Text +
                                   "' WHERE Wilayah = '" + txtWilayah.Text + "'";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Update berhasil");
                }
            }
            catch (SqlException ex) // Menangkap pesan RAISERROR dari database
            {
                SimpanLog(ex.Message); // <-- Kunci utamanya di sini, merekam ke tabel LogError!
                MessageBox.Show("SQL Error: " + ex.Message); // Memunculkan pesan peringatan di layar
            }
            catch (Exception ex) // Menangkap error umum lainnya
            {
                SimpanLog(ex.Message); //
                MessageBox.Show("General Error: " + ex.Message); //
            }
        }

        private void btnKeHalamanRekap_Click(object sender, EventArgs e)
        {
            // Membuka FormRekap yang sudah kita buat sesuai modul
            FormRekap frmRekap = new FormRekap();
            frmRekap.Show();

            // Menyembunyikan Dashboard Admin ini sementara waktu
            this.Hide();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // 1. Membuat objek OpenFileDialog untuk membuka file browser
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 2. Mengatur filter format file agar user hanya bisa memilih gambar
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.gif; *.png)|*.jpg; *.jpeg; *.gif; *.png";

            // 3. Menampilkan jendela dialog file browser
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 4. Memasukkan gambar yang dipilih ke dalam PictureBox (pbFoto)
                pbFoto.Image = new Bitmap(openFileDialog.FileName);

                // 5. Memastikan gambar menyesuaikan ukuran kotak PictureBox secara otomatis
                pbFoto.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        // Fungsi bantuan untuk mengubah Image menjadi Array of Bytes (Biner)
        private byte[] ConvertImageToByteArray(Image img)
        {
            // 1. Jika di PictureBox ternyata tidak ada gambarnya, kembalikan nilai null (kosong)
            if (img == null)
                return null;

            // 2. Menyediakan wadah memori sementara untuk menampung aliran data gambar
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                // 3. Menyimpan objek gambar ke dalam MemoryStream dengan format Jpeg
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                // 4. Mengembalikan hasil konversi dalam bentuk array byte yang siap dikirim ke database
                return ms.ToArray();
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            FormDashboard dashboard = new FormDashboard();
            dashboard.Show();
            this.Close();
        }

        private void txtNama_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya mengizinkan huruf, angka, spasi, dan tombol backspace (untuk menghapus)
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Tolak input jika berupa simbol
                MessageBox.Show("Nama tidak boleh mengandung simbol!", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtWilayah_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya mengizinkan huruf, spasi, dan tombol backspace (angka dan simbol dilarang)
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Tolak input jika berupa angka atau simbol
                MessageBox.Show("Wilayah hanya boleh berisi huruf dan spasi!", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

