using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource[] cts = new[] { new CancellationTokenSource(), new CancellationTokenSource(), new CancellationTokenSource(), new CancellationTokenSource() };

        public MainWindow()
        {
            InitializeComponent();
        }

        private int clickCount = 0;

        private async void BtnDownloadClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(MyTextBox.Text))
            {
                MessageBox.Show("It must e not null");
                throw new NullReferenceException();
            }
            ProgressBar[] pBarArray = new ProgressBar[] { ProgressBar1, ProgressBar2, ProgressBar3, ProgressBar4 };
            Button[] btnArray = new[] { BtnCancel1, BtnCancel2, BtnCancel3, BtnCancel4 };
            var link = MyTextBox.Text;

            try
            {
                await taskDownloadAsync(link, pBarArray[clickCount], btnArray[clickCount], cts[clickCount++].Token);

            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Operation was cancelled");
            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        async Task taskDownloadAsync(string url, ProgressBar pBar, Button btn, CancellationToken token)
        {
            if (String.IsNullOrEmpty(url))
                throw new NullReferenceException();
            pBar.Visibility = Visibility.Visible;
            btn.Visibility = Visibility.Visible;
            string path = @"C:\Users\Gleb\GSviridov\HW_18\HW_18\";
            var randName = new Random().Next().ToString() + ".html";
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url, token))
            using (FileStream fsDestination = new FileStream(path + randName, FileMode.OpenOrCreate))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var size = stream.Length;
                byte[] buff = new byte[size / 10];
                var readCount = stream.Read(buff, 0, buff.Length);
                while (readCount > 0)
                {

                    fsDestination.Write(buff, 0, readCount);
                    readCount = stream.Read(buff, 0, buff.Length);
                    if (token.IsCancellationRequested)
                    {
                        pBar.Visibility = Visibility.Hidden;
                        btn.Visibility = Visibility.Hidden;
                        fsDestination.Dispose();
                        token.ThrowIfCancellationRequested();
                    }
                    ProgressReport(pBar);
                    await Task.Delay(200);
                }

                pBar.Value = 100;
                btn.Visibility = Visibility.Hidden;
            }

        }

        private void ProgressReport(ProgressBar pBar)
        {
            if (pBar.Value < 91)
            {
                pBar.Value += 10;
            }
        }

        private void BtnClickCancel1(object sender, RoutedEventArgs e)
        {
            cts[0].Cancel();

        }

        private void BtnCancel2_OnClick(object sender, RoutedEventArgs e)
        {
            cts[1].Cancel();
        }

        private void BtnCancel3_OnClick(object sender, RoutedEventArgs e)
        {
            cts[2].Cancel();
        }

        private void BtnCancel4_OnClickkCancel4(object sender, RoutedEventArgs e)
        {
            cts[3].Cancel();
        }
    }
}
