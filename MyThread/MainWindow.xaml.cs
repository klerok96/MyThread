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
        CancellationTokenSource[] _tokenSourceArr = new CancellationTokenSource[4];
        ManualResetEvent _mre;
        ManualResetEvent[] _mreArr = new ManualResetEvent[4];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateNewTreadButton_Click(object sender, RoutedEventArgs e)
        {
            temp();
        }

        async void temp()
        {
            Worker worker;

            var progressBar = ControlHelpers.FindEmptyPogressBar(this);

            if (progressBar != null)
            {
                var tokenSource = new CancellationTokenSource();
                _tokenSourceArr[progressBar.GetIdProgressBar() - 1] = tokenSource;
                var token = tokenSource.Token;
                ///есть ли смысл создавать новый каждый раз?
                _mre = new ManualResetEvent(true);
                _mreArr[progressBar.GetIdProgressBar() - 1] = _mre;

                NumberThreadCB.Items.Add(progressBar.GetIdProgressBar().ToString());

                worker = new Worker(progressBar.GetIdProgressBar());

                worker.ProcessChanged += worker_ProcessChanged;

                try
                {
                    do
                    {
                        if (_countThreadQueue != 0)
                            InQueueLabel.Content = $"В очереди: {--_countThreadQueue}"; ;

                        await Task.Factory.StartNew(worker.Work, new
                        {
                            Delay = 100,
                            Token = token,
                            Mre = _mreArr[progressBar.GetIdProgressBar() - 1]
                        }, token);
                    }
                    while (_countThreadQueue != 0);
                }
                catch (OperationCanceledException)
                {
                    progressBar.Value = 0;
                    if (_countThreadQueue != 0)
                    {
                        temp();
                    }
                }
                catch (Exception ex)
                { }
                NumberThreadCB.Items.Remove(progressBar.GetIdProgressBar().ToString());
            }
            else
            {
                InQueueLabel.Content = $"В очереди: {++_countThreadQueue}";
            }
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
            _tokenSourceArr[Convert.ToInt32(NumberThreadCB.SelectedValue) - 1].Cancel();
        }

        private void StopThreadButton_Click(object sender, RoutedEventArgs e)
        {
            _mreArr[Convert.ToInt32(NumberThreadCB.SelectedValue) - 1].Reset();
        }

        private void СontinueThreadButton_Click(object sender, RoutedEventArgs e)
        {
            _mreArr[Convert.ToInt32(NumberThreadCB.SelectedValue) - 1].Set();
        }
    }
}
