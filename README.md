# DBGilingPadi

Form Koneksi <img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/c3d2d9d0-8f1d-434d-ac4a-5bef096bf699" />
Form Input Data <img width="1366" height="768" alt="Screenshot (1172)" src="https://github.com/user-attachments/assets/d1f0da42-4121-44c3-8d67-864432e24a41" />

Form Tampilan Data Pada Dashboard User<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/9383447a-5fcd-4bbc-952d-79e3e9d4b830" />
Form Tampilan Data Pada Dashboard Admin<img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/29ec8733-a664-49f4-9b06-dbe135ac0b5d" />
Bukti Insert <img width="1366" height="768" alt="Screenshot (1170)" src="https://github.com/user-attachments/assets/f2673c6a-fa8a-46cb-8556-46f52cc3db08" />
Bukti Update <img width="1366" height="768" alt="Screenshot (1171)" src="https://github.com/user-attachments/assets/4cb9b938-2d4c-434d-b383-374d083a5de0" />
Bukti Delete <img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/094957b0-bcd2-43be-95a0-1f7c23a026ba" />

Bukti Search <img width="1366" height="768" alt="Screenshot (1173)" src="https://github.com/user-attachments/assets/fe42c953-5aed-47fe-8ac1-c959da1fb40c" />

SQL Injection pada form login <img width="1366" height="768" alt="image" src="https://github.com/user-attachments/assets/ee3a4dca-fdae-4746-9d60-9f5926aa12f3" />

SKENARIO SQL INJECTION PADA FORM LOGIN
string query =
    "SELECT TOP 1 RoleUser FROM Users " +
    "WHERE Username='" + txtUsername.Text +
    "' AND Password='" + txtPassword.Text + "'";
Query di atas rentan terhadap SQL Injection karena input user langsung digabung ke query tanpa parameter.

| Username | Password | RoleUser |
| -------- | -------- | -------- |
| admin    | admin123 | Admin    |
| petani   | user123  | User     |

Username : admin' --
Password : bebas

--Query Yang Terbentuk-----------|
SELECT TOP 1 RoleUser FROM Users |
WHERE Username='admin' --'       |
AND Password='bebas'             |
----------------------------------

-- digunakan untuk membuat komentar di SQL
akibatnya bagian AND Password='bebas' diabaikan oleh SQL Server --> maka query berubah menjadi 

SELECT TOP 1 RoleUser FROM Users
WHERE Username='admin'

Karena user admin ada di database, login berhasil tanpa mengetahui password asli.

HASIL
- Sistem berhasil ditembus tanpa password asli
- User langsung masuk sebagai Admin
- Ini membuktikan adanya celah SQL Injection

Penyebab
- query dibuat dengan penggabungan string
- input user tidak divalidasi
- tidak menggunakan parameter query

  Solusi Pencegahan
Gunakan parameter query : string query =
                          "SELECT RoleUser FROM Users
                           WHERE Username=@username
                          AND Password=@password";
  Dengan parameter : cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                     cmd.Parameters.AddWithValue("@password", txtPassword.Text);

