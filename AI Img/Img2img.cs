using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI_Img
{
    public partial class Img2img : Form
    {
        private HttpClient httpClient = new HttpClient();
        private bool isGenerating = false;
        private const string API_KEY = "a66422bf-b296-4e3e-8f91-da56f4404154";
        private Image uploadedImage = null;
        public Img2img()
        {
            InitializeComponent();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            UpdateUI();
        }

        private void Img2img_Load(object sender, EventArgs e)
        {
            bunifuLabel2.Text = $"{UserData.Name}";
            picResult.Image = null;
            picUpload.Image = null;
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (isGenerating) return;

            if (string.IsNullOrWhiteSpace(txtPrompt.Text))
            {
                lblStatus.Text = "Введите текстовый запрос (prompt)";
                return;
            }
            if (string.IsNullOrWhiteSpace(bunifuDropdown1.Text))
            {
                lblStatus.Text = "Выберите модель генерации";
                return;
            }

            isGenerating = true;
            btnGenerate.Enabled = false;
            lblStatus.Text = "Генерация...";
            picResult.Image = null;

            try
            {
                // Подготовка параметров запроса
                var payload = new
                {
                    token = API_KEY,
                    model = bunifuDropdown1.Text,
                    prompt = txtPrompt.Text,
                    negative_prompt = string.IsNullOrWhiteSpace(neg_text.Text) ?
                                      "low quality, blurry, bad anatomy" : neg_text.Text,
                    width = 1024,
                    height = 1024,
                    steps = 30,
                    sampler = "Euler",
                    cfg_scale = 7,
                    seed = -1,
                    stream = false,
                    response_type = "url",
                    nsfw_filter = true,
                    send_to_gallery = false,
                    // Параметры для img2img
                    init_image = uploadedImage != null ? ImageToBase64(uploadedImage) : null,
                    denoising_strength = uploadedImage != null ? 0.7 : (double?)null
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://neuroimg.art/api/v1/generate", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GenerationResult>(responseContent);

                if (!string.IsNullOrEmpty(result.image_url))
                {
                    await LoadResultImage(result.image_url);
                    lblStatus.Text = "Готово!";

                    if (bunifuCheckBox1.Checked)
                    {
                        lblStatus.Text += " Изображение сохранено.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка: {ex.Message}";
            }
            finally
            {
                isGenerating = false;
                btnGenerate.Enabled = true;
            }
        }


        private class GenerationResult
        {
            public string image_url { get; set; }
            public string image { get; set; } // Для response_type="base64"
        }
        private string ImageToBase64(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private async Task LoadResultImage(string url)
        {
            try
            {
                var imageBytes = await httpClient.GetByteArrayAsync(url);
                using (var ms = new MemoryStream(imageBytes))
                {
                    var image = Image.FromStream(ms);
                    picResult.Image = image;
                    SaveImageToDatabase(image);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка загрузки изображения: {ex.Message}";
            }
        }

        private void SaveImageToDatabase(Image image)
        {
            if (!bunifuCheckBox1.Checked) return;

            try
            {
                byte[] imageData;
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageData = ms.ToArray();
                }

                using (var connection = new SQLiteConnection("Data Source=img.db;Version=3;"))
                {
                    connection.Open();
                    string query = "INSERT INTO images (user_id, image) VALUES (@user_id, @image)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", UserData.UserId);
                        command.Parameters.AddWithValue("@image", imageData);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Выберите изображение для обработки";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        uploadedImage = Image.FromFile(openFileDialog.FileName);
                        picUpload.Image = uploadedImage;
                        lblUploadedImage.Text = Path.GetFileName(openFileDialog.FileName);
                        UpdateUI();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateUI()
        {
            bool hasImage = uploadedImage != null;
            btnClearUpload.Enabled = hasImage;
            lblUploadedImage.Text = hasImage ? lblUploadedImage.Text : "Изображение не загружено";

            // Можно добавить дополнительные элементы UI, которые зависят от наличия изображения
        }

        private void picResult_Click(object sender, EventArgs e)
        {
            if (picResult.Image != null)
            {
                var viewForm = new Form()
                {
                    Text = "Просмотр результата",
                    StartPosition = FormStartPosition.CenterScreen,
                    Size = new Size(800, 600)
                };

                var fullSizePictureBox = new PictureBox()
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = picResult.Image
                };

                var panel = new Panel()
                {
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    BackColor = Color.LightGray
                };

                var btnSave = new Button()
                {
                    Text = "Сохранить",
                    Width = 100,
                    Height = 30,
                    Top = 5,
                    Left = 10
                };
                btnSave.Click += (s, args) => SaveImageToFile(picResult.Image);

                var btnClose = new Button()
                {
                    Text = "Закрыть",
                    Width = 100,
                    Height = 30,
                    Top = 5,
                    Left = viewForm.Width - 110
                };
                btnClose.Click += (s, args) => viewForm.Close();

                panel.Controls.Add(btnSave);
                panel.Controls.Add(btnClose);
                viewForm.Controls.Add(panel);
                viewForm.Controls.Add(fullSizePictureBox);

                viewForm.ShowDialog();
            }
        }

        private void SaveImageToFile(Image image)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp";
                saveFileDialog.Title = "Сохранить изображение";
                saveFileDialog.FileName = $"result_{DateTime.Now:yyyyMMddHHmmss}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                        switch (Path.GetExtension(saveFileDialog.FileName).ToLower())
                        {
                            case ".jpg": format = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                            case ".bmp": format = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        }

                        image.Save(saveFileDialog.FileName, format);
                        MessageBox.Show("Изображение успешно сохранено!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                    }
                }
            }
        }
    }
}
