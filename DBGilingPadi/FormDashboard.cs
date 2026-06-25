using ExcelDataReader;
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
using System.Windows.Forms.DataVisualization.Charting;

namespace DBGilingPadi
{
    public partial class FormDashboard : Form
    {
        // 1. String koneksi database SQL Server milikmu
        string connectionString = @"Data Source=DESKTOP-GU8JFSR\ZIDANE; Initial Catalog=DBGilingPadi; User ID=sa; Password=apasaja;";

        // 2. Deklarasi objek Chart secara global lewat kode
        private Chart chartRekap;

        public FormDashboard()
        {
            InitializeComponent();

            // 3. Panggil fungsi untuk menggambar kotak grafik kosong saat form dibuat
            InisialisasiChartManual();
        }

        private void InisialisasiChartManual()
        {
            chartRekap = new Chart();
            chartRekap.Location = new Point(40, 100);
            chartRekap.Size = new Size(500, 300);

            ChartArea chartArea = new ChartArea("MainArea");
            chartRekap.ChartAreas.Add(chartArea);

            // ============================================================
            // TAMBAHKAN 2 BARIS INI UNTUK MEMUNCUKKAN KOTAK PETUNJUK WARNA
            // ============================================================
            Legend legend = new Legend("MainLegend");
            chartRekap.Legends.Add(legend);
            // ============================================================

            Series series = new Series("Total Tempat Gilingan");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.DodgerBlue;
            chartRekap.Series.Add(series);

            this.Controls.Add(chartRekap);
        }

        private void btnKeFormAdmin_Click(object sender, EventArgs e)
        {
            FormAdmin formAdmin = new FormAdmin();
            formAdmin.Show();
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx; *.xls)|*.xlsx; *.xls";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 2. Membaca file Excel menggunakan library ExcelDataReader yang di-install kemarin
                    using (var stream = System.IO.File.Open(openFileDialog.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                        {
                            // 3. Mengonversi hasil pembacaan Excel menjadi objek DataSet C#
                            var result = reader.AsDataSet(new ExcelDataReader.ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataReader.ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true // Menggunakan baris pertama Excel sebagai judul kolom
                                }
                            });

                            // 4. Mengambil tabel pertama dari file Excel
                            DataTable dtExcel = result.Tables[0];

                            // 5. Memanggil Class DAL yang sudah kita buat sebelumnya untuk melempar data ke SQL Server
                            DAL dataAccess = new DAL();
                            dataAccess.ImportExcelToDatabase(dtExcel);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal membaca file Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            // 1. Bersihkan data lama pada grafik
            chartRekap.Series["Total Tempat Gilingan"].Points.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    // KONDISI 1: Jika user memilih wilayah tertentu di ComboBox
                    if (comboBox1.SelectedIndex != -1)
                    {
                        // Ubah bentuk grafik menjadi PIE (Diagram Lingkaran) agar terlihat estetik
                        chartRekap.Series["Total Tempat Gilingan"].ChartType = SeriesChartType.Pie;

                        // Menampilkan label persentase dan nama di dalam diagram lingkaran
                        chartRekap.Series["Total Tempat Gilingan"].IsValueShownAsLabel = true;

                        // Mengatur agar label di dalam kue menampilkan "Nama (Jumlah)" -> Contoh: Sleman (5)
                        chartRekap.Series["Total Tempat Gilingan"].Label = "#AXISLABEL (#VAL)";

                        // Query mengambil data wilayah terpilih VS wilayah lainnya sebagai pembanding
                        query = @"SELECT CASE WHEN Wilayah = @wilayah THEN Wilayah ELSE 'Wilayah Lainnya' END AS WilayahGroup, 
                                 COUNT(*) as Total 
                          FROM Penggilingan 
                          GROUP BY CASE WHEN Wilayah = @wilayah THEN Wilayah ELSE 'Wilayah Lainnya' END";

                        cmd.Parameters.AddWithValue("@wilayah", comboBox1.SelectedItem.ToString());
                    }
                    // KONDISI 2: Jika ComboBox kosong / Reset (Menampilkan Semua Wilayah berdampingan)
                    else
                    {
                        // Kembalikan bentuk grafik menjadi COLUMN (Diagram Batang)
                        chartRekap.Series["Total Tempat Gilingan"].ChartType = SeriesChartType.Column;
                        chartRekap.Series["Total Tempat Gilingan"].IsValueShownAsLabel = true;

                        query = "SELECT Wilayah, COUNT(*) as Total FROM Penggilingan GROUP BY Wilayah";
                    }

                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string namaWilayah = reader[0].ToString();
                            int jumlahGilingan = Convert.ToInt32(reader[1]);

                            // Tambahkan data ke grafik
                            chartRekap.Series["Total Tempat Gilingan"].Points.AddXY(namaWilayah, jumlahGilingan);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tidak ada data di database untuk direkap.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat grafik: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            chartRekap.Series["Total Tempat Gilingan"].Points.Clear();
        }

        private void FormDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
