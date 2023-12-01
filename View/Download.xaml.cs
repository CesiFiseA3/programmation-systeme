using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PROGRAMMATION_SYST_ME.View
{
    public partial class Download : Window
    {
        private CancellationTokenSource cancellationTokenSource1;
        private ProgressBar downloadProgressBar1;
        private TextBlock Status1;
        private TextBlock Status3;
        private TextBlock Status4;
        private CancellationTokenSource cancellationTokenSource2;
        private ProgressBar downloadProgressBar2;
        private TextBlock Status2;
        private CancellationTokenSource cancellationTokenSource3;
        private CancellationTokenSource cancellationTokenSource4;

        public Download()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource1 = new CancellationTokenSource();
            Status1.Text = "On going";

            try
            {
                await DownloadFileAsync(downloadProgressBar1, cancellationTokenSource1.Token);
                Status1.Text = "Finish";
            }
            catch (OperationCanceledException)
            {
                Status1.Text = "Cancel";
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource1?.Cancel();
            Status1.Text = "Pending";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource1?.Cancel();
            Status1.Text = "Cancel";
        }

        private async void Start_Click1(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource2 = new CancellationTokenSource();
            Status2.Text = "On going";

            try
            {
                await DownloadFileAsync(downloadProgressBar2, cancellationTokenSource2.Token);
                Status2.Text = "Finish";
            }
            catch (OperationCanceledException)
            {
                Status2.Text = "Cancel";
            }
        }

        private void Pause_Click1(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource2?.Cancel();
            Status2.Text = "Pending";
        }

        private void Cancel_Click1(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource2?.Cancel();
            Status2.Text = "Cancel";
        }
        private async void Start_Click2(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource3 = new CancellationTokenSource();
            Status3.Text = "On going";

            try
            {
                await DownloadFileAsync(downloadProgressBar2, cancellationTokenSource2.Token);
                Status3.Text = "Finish";
            }
            catch (OperationCanceledException)
            {
                Status3.Text = "Cancel";
            }
        }

        private void Pause_Click2(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource3?.Cancel();
            Status3.Text = "Pending";
        }

        private void Cancel_Click2(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource3?.Cancel();
            Status3.Text = "Cancel";
        }
        private async void Start_Click3(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource4 = new CancellationTokenSource();
            Status4.Text = "On going";

            try
            {
                await DownloadFileAsync(downloadProgressBar2, cancellationTokenSource2.Token);
                Status4.Text = "Finish";
            }
            catch (OperationCanceledException)
            {
                Status4.Text = "Cancel";
            }
        }

        private void Pause_Click3(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource4?.Cancel();
            Status4.Text = "Pending";
        }

        private void Cancel_Click3(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource4?.Cancel();
            Status4.Text = "Cancel";
        }


        private async Task DownloadFileAsync(ProgressBar progressBar, CancellationToken cancellationToken)
        {
            for (int i = 0; i <= 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Dispatcher.InvokeAsync(() => progressBar.Value = i, DispatcherPriority.Background);
                await Task.Delay(100);
            }
        }
    }
}
