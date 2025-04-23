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
using System.IO;

namespace AI_Img
{
    public partial class Gallery : Form
    {
        private const string ConnectionString = "Data Source=img.db;Version=3;";
        private List<Image> userImages = new List<Image>();
        public Gallery()
        {
            InitializeComponent();
        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            Main_img2img main = new Main_img2img();
            main.Show();
            this.Hide();
        }

        private void Gallery_Load(object sender, EventArgs e)
        {
            label_user.Text = $"{UserData.Name}";
            LoadUserImages();
        }

        private void LoadUserImages()
        {
            try
            {
                flowLayoutPanel1.Controls.Clear(); // Очищаем панель перед загрузкой
                userImages.Clear();

                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT image FROM images WHERE user_id = @user_id ORDER BY upload_date DESC";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", UserData.UserId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] imageData = (byte[])reader["image"];
                                using (var ms = new MemoryStream(imageData))
                                {
                                    Image img = Image.FromStream(ms);
                                    userImages.Add(img);

                                    // Создаем PictureBox для каждого изображения
                                    var pictureBox = new PictureBox
                                    {
                                        Width = 200,
                                        Height = 200,
                                        SizeMode = PictureBoxSizeMode.Zoom,
                                        Image = img,
                                        Margin = new Padding(10)
                                    };

                                    // Добавляем обработчик клика
                                    pictureBox.Click += PictureBox_Click;

                                    flowLayoutPanel1.Controls.Add(pictureBox);
                                }
                            }
                        }
                    }
                }

                if (flowLayoutPanel1.Controls.Count == 0)
                {
                    Label emptyLabel = new Label()
                    {
                        Text = "В вашей галерее пока нет изображений",
                        AutoSize = true,
                        Font = new Font("Arial", 12),
                        ForeColor = Color.Gray
                    };
                    flowLayoutPanel1.Controls.Add(emptyLabel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}");
            }
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                // Создаем форму для просмотра изображения в полном размере
                var viewForm = new Form()
                {
                    Text = "Просмотр изображения",
                    StartPosition = FormStartPosition.CenterScreen,
                    Size = new Size(800, 600),
                    MinimizeBox = false,
                    MaximizeBox = true,
                    FormBorderStyle = FormBorderStyle.Sizable
                };

                var fullSizePictureBox = new PictureBox()
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = pictureBox.Image
                };

                // Создаем панель для кнопок
                var panel = new Panel()
                {
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    BackColor = Color.LightGray
                };

                // Кнопка сохранения
                var btnSave = new Button()
                {
                    Text = "Сохранить",
                    Width = 100,
                    Height = 30,
                    Top = 5,
                    Left = 10,
                    Anchor = AnchorStyles.Left
                };
                btnSave.Click += (s, args) => SaveImageToFile(pictureBox.Image);

                // Кнопка закрытия
                var btnClose = new Button()
                {
                    Text = "Закрыть",
                    Width = 100,
                    Height = 30,
                    Top = 5,
                    Left = viewForm.Width - 110,
                    Anchor = AnchorStyles.Right
                };
                btnClose.Click += (s, args) => viewForm.Close();

                panel.Controls.Add(btnSave);
                panel.Controls.Add(btnClose);

                viewForm.Controls.Add(panel);
                viewForm.Controls.Add(fullSizePictureBox);

                // Обработчик изменения размера формы
                viewForm.Resize += (s, args) =>
                {
                    btnClose.Left = viewForm.Width - 110;
                };

                viewForm.ShowDialog();
            }
        }

        private void SaveImageToFile(Image image)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp";
                saveFileDialog.Title = "Сохранить изображение";
                saveFileDialog.FileName = $"image_{DateTime.Now:yyyyMMddHHmmss}"; // Автоматическое имя файла

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Определяем формат на основе выбранного расширения
                        System.Drawing.Imaging.ImageFormat format;
                        switch (Path.GetExtension(saveFileDialog.FileName).ToLower())
                        {
                            case ".jpg":
                            case ".jpeg":
                                format = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case ".png":
                                format = System.Drawing.Imaging.ImageFormat.Png;
                                break;
                            case ".bmp":
                                format = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                            default:
                                format = System.Drawing.Imaging.ImageFormat.Png;
                                break;
                        }

                        image.Save(saveFileDialog.FileName, format);
                        MessageBox.Show("Изображение успешно сохранено!", "Успех",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUserImages();
        }
    }
}
