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
    public partial class FormLogin : Form
    {
        string connectionString =
            @"Data Source=DESKTOP-GU8JFSR\ZIDANE;
            Initial Catalog=DBGilingPadi;
            Integrated Security=True";


        public FormLogin()
        {
            InitializeComponent();

            txtPassword.UseSystemPasswordChar = true;
            this.AcceptButton = this.btnLogin;
        }

        //private void btnLogin_Click(object sender, EventArgs e)
        //{
        //if (txtUsername.Text == "" ||
        //txtPassword.Text == "")
        //{
        //MessageBox.Show(
        //"Username dan Password wajib diisi!");

        //return;
        //}

        //try
        //{
        //using (SqlConnection conn =
        //new SqlConnection(connectionString))
        //{
        //conn.Open();

        //string query =
        //@"SELECT RoleUser
        //FROM Users
        //WHERE Username=@username
        //AND Password=@password";

        //SqlCommand cmd =
        //new SqlCommand(query, conn);

        //cmd.Parameters.Add("@username",
        //SqlDbType.VarChar).Value =
        //txtUsername.Text;

        //cmd.Parameters.Add("@password",
        //SqlDbType.VarChar).Value =
        //txtPassword.Text;

        //object result = cmd.ExecuteScalar();

        //if (result != null)
        //{
        //string role = result.ToString();

        //MessageBox.Show(
        //"Login berhasil!");

        // LOGIN ADMIN--
        //if (role == "Admin")
        //{
        //FormAdmin admin =
        //new FormAdmin();

        //admin.Show();

        //this.Hide();
        //}

        // LOGIN USER--
        //else if (role == "User")
        //{
        //FormUser user =
        //new FormUser();

        //user.Show();

        //this.Hide();
        //}
        //}
        //else
        //{
        //MessageBox.Show(
        //"Username atau Password salah!");
        //}
        //}
        //}
        //catch (Exception ex)
        //{
        //MessageBox.Show(ex.Message);
        //}
        //}

        //private void btnLogin_Click(object sender, EventArgs e)
        //{
            //try
            //{
                //using (SqlConnection conn = new SqlConnection(connectionString))
                //{
                    //conn.Open();

                    // KODE PALING RENTAN: Menggunakan COUNT agar jika ada 1 baris yang TRUE, login lolos --
                    //string query = "SELECT COUNT(*) FROM Users WHERE Username = '" + txtUsername.Text + "' AND Password = '" + txtPassword.Text + "'";

                    //SqlCommand cmd = new SqlCommand(query, conn);
                    //int totalBaris = (int)cmd.ExecuteScalar();

                    //if (totalBaris > 0)
                    //{
                        //MessageBox.Show("Login Berhasil! (SQL Injection Sukses)");

                        // Untuk sementara, langsung buka FormUser atau FormAdmin
                        //FormUser userForm = new FormUser();
                        //userForm.Show();
                        //this.Hide();
                    //}
                    //else
                    //{
                        //MessageBox.Show("Username atau Password salah!");
                    //}
                //}
            //}
            //catch (Exception ex)
            //{
                //MessageBox.Show("Error: " + ex.Message);
            //}
        //}

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    // RENTAN SQL INJECTION
                    string query =
                        "SELECT TOP 1 RoleUser FROM Users " +
                        "WHERE Username='" + txtUsername.Text +
                        "' AND Password='" + txtPassword.Text + "'";

                    SqlCommand cmd =
                        new SqlCommand(query, conn);

                    object result =
                        cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string role =
                            result.ToString();

                        MessageBox.Show(
                            "Login Berhasil!");

                        // ROLE ADMIN
                        if (role == "Admin")
                        {
                            FormAdmin admin =
                                new FormAdmin();

                            admin.Show();
                            this.Hide();
                        }

                        // ROLE USER
                        else if (role == "User")
                        {
                            FormUser user =
                                new FormUser();

                            user.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Username atau Password salah!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message);
            }
        }

    }
}