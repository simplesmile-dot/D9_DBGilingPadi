using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace DBGilingPadi
{
    public class DAL
    {
        // 1. Deklarasi string koneksi ke database SQL Server milikmu
        private string connectionString = @"Data Source=192.168.18.9,1433; Initial Catalog=DBGilingPadi; User ID=sa; Password=apasaja;";

        public SqlConnection conn;
        public SqlDataAdapter da;

        // Constructor kelas DAL
        public DAL()
        {
            string ipServer = GetLocalIPAddress();
            connectionString = $"Data Source={ipServer},1433; Initial Catalog=DBGilingPadi; User ID=sa; Password=apasaja;";
            conn = new SqlConnection(connectionString);
        }
        // Fungsi untuk mengambil connection string
        public string GetConnectionString()
        {
            return connectionString;
        }

        // 2. Fungsi Utama untuk Memasukkan Data Tabel Excel ke Database (Sesuai Poin D.6 Modul)
        public void ImportExcelToDatabase(DataTable dtExcel)
        {
            // Menggunakan SqlConnection untuk menyambung ke database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Melakukan perulangan (looping) menyisir baris demi baris data dari Excel
                    foreach (DataRow row in dtExcel.Rows)
                    {
                        // Memanggil Stored Procedure insert data penggilingan padi milikmu
                        using (SqlCommand cmd = new SqlCommand("sp_InsertPenggilingan", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Memetakan isi kolom Excel ke dalam parameter Stored Procedure
                            // CATATAN: Pastikan nama di dalam row["..."] sama persis dengan judul kolom di file Excel kamu nanti
                            cmd.Parameters.AddWithValue("@nama", row["NamaTempat"].ToString());
                            cmd.Parameters.AddWithValue("@alamat", row["Alamat"].ToString());
                            cmd.Parameters.AddWithValue("@wilayah", row["Wilayah"].ToString());
                            cmd.Parameters.AddWithValue("@status", row["StatusOperasional"].ToString());

                            // Karena data Excel tidak memuat foto biner, kita beri nilai default DBNull (Kosong)
                            cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = DBNull.Value;

                            // Eksekusi penyimpanan data per baris ke database SQL Server
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Hebat! Semua data dari file Excel berhasil di-import ke database.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan saat menyimpan data Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }


        public string GetLocalIPAddress()
        {
            // Mencari list IP Address yang ada di komputer server ini
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // Pastikan hanya mengambil IP Address versi 4 (IPv4)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString(); // Mengembalikan IP seperti "192.168.1.X"
                }
            }
            // Jika tidak terhubung ke jaringan apa pun, gunakan ip lokal standar
            return "127.0.0.1";
        }
    }

}