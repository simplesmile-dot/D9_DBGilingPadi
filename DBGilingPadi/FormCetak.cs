using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBGilingPadi
{
    public partial class FormCetak : Form
    {
        // Menggunakan Nama Server Asli + Autentikasi SQL Server (sa)
        string connectionString = @"Data Source=DESKTOP-GU8JFSR\ZIDANE; Initial Catalog=DBGilingPadi; User ID=sa; Password=apasaja;";

        SqlConnection conn;
        SqlDataAdapter da;
        DataTable dtPenggilingan;

        List<ListPenggilingan> listDataGiling = new List<ListPenggilingan>();

        string wilayahFilter;
        string statusFilter;

        public FormCetak(string Wilayah, string Status)
        {
            InitializeComponent();
            wilayahFilter = Wilayah;
            statusFilter = Status;

            // Inisialisasi koneksi di dalam constructor agar tidak merah
            conn = new SqlConnection(connectionString);
        }

        private void FormCetak_Load(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_ReportPenggilingan", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parameter filter aktif kembali
                cmd.Parameters.AddWithValue("@inWilayah", wilayahFilter);
                cmd.Parameters.AddWithValue("@inStatus", statusFilter);

                da = new SqlDataAdapter(cmd);
                dtPenggilingan = new DataTable();
                da.Fill(dtPenggilingan);
                conn.Close();

                listDataGiling.Clear();

                foreach (DataRow row in dtPenggilingan.Rows)
                {
                    listDataGiling.Add(new ListPenggilingan
                    {
                        ID_Giling = row["ID_Giling"].ToString(),
                        NamaTempat = row["NamaTempat"].ToString(),
                        Alamat = row["Alamat"].ToString(),
                        Wilayah = row["Wilayah"].ToString(),
                        StatusOperasional = row["StatusOperasional"].ToString()
                    });
                }

                ReportDocument rpt = new ReportDocument();
                string rptPath = @"E:\PABD\UCP 1\DBGilingPadi\DBGilingPadi\ReportPenggilingan.rpt";

                if (File.Exists(rptPath))
                {
                    rpt.Load(rptPath);
                }
                else
                {
                    rpt.Load(Path.Combine(Environment.CurrentDirectory, "ReportPenggilingan.rpt"));
                }

                rpt.SetDataSource(listDataGiling);
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal Load data laporan: " + ex.Message);
            }
        }
    }
}