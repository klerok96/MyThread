using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MyThread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _countThreadQueue = 0;
        CancellationTokenSource[] _tokenSourceArr;
        ManualResetEvent[] _mreArr;

        public MainWindow()
        {
            _tokenSourceArr = new CancellationTokenSource[4];

            _mreArr = new ManualResetEvent[]
            {                
                new ManualResetEvent(true),
                new ManualResetEvent(true),
                new ManualResetEvent(true),
                new ManualResetEvent(true)
            };

            InitializeComponent();
        }

        private void CreateNewTreadButton_Click(object sender, RoutedEventArgs e)
        {
            var progressBar = ControlHelpers.FindEmptyPogressBar(this);

            if (progressBar != null)
            {                
                var numberSelectedPB = progressBar.GetNumberSelectedPB();

                var worker = new Worker(numberSelectedPB);
                worker.ProcessChanged += worker_ProcessChanged;

                CreateNewTreadAsync(progressBar, numberSelectedPB, worker);
            }
            else
            {
                InQueueLabel.Content = $"В очереди: {++_countThreadQueue}";
            }
        }

        async void CreateNewTreadAsync(ProgressBar progressBar,int numberSelectedPB,Worker worker)
        {
            NumberThreadCB.Items.Add(numberSelectedPB.ToString());

            try
            {
                _tokenSourceArr[numberSelectedPB - 1] = new CancellationTokenSource();

                await Task.Factory.StartNew(worker.Work, new
                {
                    Delay = 100,                   
                    Mre = _mreArr[numberSelectedPB - 1],
                    Token = _tokenSourceArr[numberSelectedPB - 1].Token
                }, _tokenSourceArr[numberSelectedPB - 1].Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            { }

            if (_countThreadQueue != 0)
            {
                InQueueLabel.Content = $"В очереди: {--_countThreadQueue}";
                CreateNewTreadAsync(progressBar, numberSelectedPB, worker);
            }

            NumberThreadCB.Items.Remove(numberSelectedPB.ToString());       
        }

        private void worker_ProcessChanged(int progress, int workId)
        {
            ProgressBar progressBar;

            Dispatcher.Invoke(() => {
                progressBar = (ProgressBar)FindName($"ProgressBar{workId}");

                if (progressBar != null)
                    progressBar.Value = progress;
            });
        }

        private void CompleteThreadButtom_Click(object sender, RoutedEventArgs e)
        {
            if (NumberThreadCB.SelectedItem != null)
                _tokenSourceArr[Convert.ToInt32(NumberThreadCB.SelectedValue) - 1].Cancel();
        }

        private void StopThreadButton_Click(object sender, RoutedEventArgs e)
        {
            if (NumberThreadCB.SelectedItem != null)
                _mreArr[Convert.ToInt32(NumberThreadCB.SelectedValue) - 1].Reset();
        }

        private void СontinueThreadButton_Click(object sender, RoutedEventArgs e)
        {
            if (NumberThreadCB.SelectedItem != null)
                _mreArr[Convert.ToInt32(NumberThreadCB.SelectedValue) - 1].Set();
        }
    }
}
