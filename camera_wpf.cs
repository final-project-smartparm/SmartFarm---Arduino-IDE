using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Threading;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private WebClient _webClient;
        private bool _isStreaming;
        private const string StreamUrl = "http://192.168.5.5/mjpeg/1"; // 여기에 ESP32-CAM의 MJPEG 스트림 URL을 입력

        public MainWindow()
        {
            InitializeComponent();
            StartMjpegStream();
        }

        private void StartMjpegStream()
        {
            _webClient = new WebClient();
            _webClient.Headers[HttpRequestHeader.Accept] = "multipart/x-mixed-replace";
            _isStreaming = true;

            Task.Run(() => StreamMjpeg());
        }

        private async void StreamMjpeg()
        {
            try
            {
                using (Stream stream = await _webClient.OpenReadTaskAsync(new Uri(StreamUrl)))
                {
                    byte[] buffer = new byte[4096];
                    MemoryStream imageStream = new MemoryStream();
                    bool imageStarted = false;

                    while (_isStreaming)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;

                        for (int i = 0; i < bytesRead; i++)
                        {
                            if (!imageStarted)
                            {
                                // MJPEG 프레임의 시작을 찾음
                                if (buffer[i] == 0xFF && buffer[i + 1] == 0xD8)
                                {
                                    imageStarted = true;
                                    imageStream = new MemoryStream();
                                }
                            }

                            if (imageStarted)
                            {
                                imageStream.WriteByte(buffer[i]);

                                // MJPEG 프레임의 끝을 찾음
                                if (buffer[i] == 0xFF && buffer[i + 1] == 0xD9)
                                {
                                    imageStarted = false;
                                    imageStream.Seek(0, SeekOrigin.Begin);
                                    Dispatcher.Invoke(() => UpdateImage(imageStream));
                                    imageStream.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error streaming MJPEG: " + ex.Message);
            }
        }

        private void UpdateImage(MemoryStream imageStream)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = imageStream;
            bitmap.EndInit();
            streamImage.Source = bitmap;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isStreaming = false;
            _webClient?.Dispose();
        }
    }
}