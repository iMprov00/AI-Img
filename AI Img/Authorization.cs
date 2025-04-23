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


namespace AI_Img
{
    public partial class Authorization : Form
    {
        private const string ConnectionString = "Data Source=img.db;Version=3;";
        public Authorization()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string login = text_login.Text.Trim();
            string password = text_pass.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT user_id, name, login FROM users WHERE login = @login AND pass = @password";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Сохраняем данные пользователя
                                UserData.UserId = Convert.ToInt32(reader["user_id"]);
                                UserData.Name = reader["name"].ToString();
                                UserData.Login = reader["login"].ToString();

                                // Открываем главную форму
                                Main_img2img main = new Main_img2img();
                                main.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }


        }

        private void text_pass_TextChanged(object sender, EventArgs e)
        {

        }

        private void text_login_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
