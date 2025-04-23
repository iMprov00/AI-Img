using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI_Img
{
    public partial class Form1 : Form
    {

        private HttpClient httpClient = new HttpClient();
        private bool isGenerating = false;
        private const string API_KEY = "a66422bf-b296-4e3e-8f91-da56f4404154"; // Замените на ваш ключ
        private Bitmap sourceImage;
        public Form1()
        {
            InitializeComponent();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void picSource_DoubleClick(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        sourceImage = new Bitmap(openDialog.FileName);
                        picSource.Image = sourceImage;
                        lblStatus.Text = "Image loaded. Ready to generate.";
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = $"Error: {ex.Message}";
                    }
                }
            }
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (isGenerating) return;

            if (string.IsNullOrWhiteSpace(txtPrompt.Text))
            {
                lblStatus.Text = "Please enter a prompt";
                return;
            }

            if (picSource.Image == null)
            {
                lblStatus.Text = "Please load an image first (double click)";
                return;
            }

            isGenerating = true;
            btnGenerate.Enabled = false;
            lblStatus.Text = "Generating...";
            picResult.Image = null;

            try
            {
                // Жестко заданные параметры
                var payload = new
                {
                    token = API_KEY,
                    model = "NovaAnimeXL-ILv5.5",
                    prompt = txtPrompt.Text,
                    negative_prompt = "low quality, blurry, bad anatomy",
                    init_image = ImageToBase64(sourceImage),
                    denoising_strength = 0.75, // Средняя сила изменения
                    width = sourceImage.Width,
                    height = sourceImage.Height,
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
                    lblStatus.Text = "Done!";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                isGenerating = false;
                btnGenerate.Enabled = true;
            }
        }

        private string ImageToBase64(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
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
                    picResult.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading result: {ex.Message}";
            }
        }

        // Класс для десериализации ответа
        private class GenerationResult
        {
            public string image_url { get; set; }
            public string image { get; set; }
        }
    }
}
