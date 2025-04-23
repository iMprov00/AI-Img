using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI_Img
{
    public partial class Main_img2img : Form
    {

        private HttpClient httpClient = new HttpClient();
        private bool isGenerating = false;
        private const string API_KEY = "a66422bf-b296-4e3e-8f91-da56f4404154";
        public Main_img2img()
        {
          //  this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            httpClient.Timeout = TimeSpan.FromMinutes(5);

        }

        private void Main_img2img_Load(object sender, EventArgs e)
        {
            picResult.Image = null;
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuPictureBox2_Click(object sender, EventArgs e)
        {

        }

        private async void bunifuButton24_Click(object sender, EventArgs e)
        {
            if (isGenerating) return;

            if (string.IsNullOrWhiteSpace(txtPrompt.Text))
            {
                lblStatus.Text = "Введите текстовый запрос (prompt)";
                return;
            }
            lblStatus.Visible = true;

            isGenerating = true;
            btnGenerate.Enabled = false;
            lblStatus.Text = "Генерация...";
            picResult.Image = null;

            try
            {
                // Параметры для txt2img (без init_image)
                var payload = new
                {
                    token = API_KEY,
                    model = "HUBGSexFlux.1-Alpha", // Или другая модель для txt2img
                    prompt = txtPrompt.Text,
                    negative_prompt = "low quality, blurry, bad anatomy",
                    width = 1920, // Стандартный размер
                    height = 1080,
                    steps = 30,
                    sampler = "Euler",
                    cfg_scale = 7,
                    seed = -1,
                    stream = false,
                    response_type = "url",
                    nsfw_filter = true,
                    send_to_gallery = false
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

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {

        }

        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async Task LoadResultImage(string url)
        {
            try
            {
                var imageBytes = await httpClient.GetByteArrayAsync(url);
                using (var ms = new MemoryStream(imageBytes))
                {
                    picResult.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка загрузки изображения: {ex.Message}";
            }
        }

        // Класс для десериализации ответа
        private class GenerationResult
        {
            public string image_url { get; set; }
            public string image { get; set; } // Для response_type="base64"
        }

        // Очистка PictureBox при загрузке формы
        private void Form1_Load(object sender, EventArgs e)
        {
            picResult.Image = null;
        }
    }
}
