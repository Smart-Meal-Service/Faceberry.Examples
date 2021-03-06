using Faceberry.Grpc.AI;
using Faceberry.Grpc.AI.Events;
using Faceberry.Grpc.AI.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Faceberry.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly GrpcChannel _channel;
        private readonly RequestService.RequestServiceClient _client;

        private readonly NotificationServiceImplementation _grpc;
        private readonly Server _server;

        private readonly DispatcherTimer _dispatcherTimer;
        private int _fps;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize main window.
        /// </summary>
        public MainWindow()
        {
            _channel = GrpcExtensions.CreateClientChannel(5070);
            _client = new RequestService.RequestServiceClient(_channel);

            _grpc = new NotificationServiceImplementation();
            _server = GrpcExtensions.CreateServer(_grpc, 5080);

            _dispatcherTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1),
            };
            _dispatcherTimer.Tick += (s, e) =>
            {
                InvokeInUiThread(() => Title = $"Fps: {_fps}");
                _fps = 0;
            };

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Main window loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();

            await _client.EnableRecognitionAsync(new BoolValue { Value = true });

            _grpc.OnReceivedNotification += OnReceivedNotification;
            _server.Start();
        }

        /// <summary>
        /// Main window closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _dispatcherTimer.Stop();

            await _client.EnableRecognitionAsync(new BoolValue { Value = false });
            _channel.Dispose();

            _grpc.OnReceivedNotification -= OnReceivedNotification;
            await _server.ShutdownAsync();
        }

        /// <summary>
        /// On received notification event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceivedNotification(object sender, NotificationEventsArgs e)
        {
            InvokeDrawing(e.FrameBytes, e.RecognitionUnitList); _fps++;
        }

        #endregion

        #region UI methods

        /// <summary>
        /// Invoke drawing task.
        /// </summary>
        /// <param name="frameBytes">Frame bytes</param>
        /// <param name="recognitionUnitList">Recognition unit list</param>
        /// <returns></returns>
        private void InvokeDrawing(byte[] frameBytes, RecognitionUnitList recognitionUnitList)
        {
            try
            {
                using var ms = new MemoryStream(frameBytes);
                using var frame = new Bitmap(ms);
                using var pen = new Pen(Color.Silver, 2);
                var identificationUnits = recognitionUnitList.RecognitionUnit;
                var length = identificationUnits.Count;

                for (var i = 0; i < length; i++)
                {
                    var unit = identificationUnits[i];
                    var region = unit.Region;
                    var boolean = unit.Boolean;

                    var rectangle = new Rectangle
                    {
                        X = region.X,
                        Y = region.Y,
                        Width = region.Width,
                        Height = region.Height
                    };

                    using var graphics = Graphics.FromImage(frame);
                    pen.Color = boolean ? Color.Gold : Color.Silver;
                    graphics.DrawRectangle(pen, rectangle);
                }

                InvokeInUiThread(() =>
                {
                    videoPlayer.Source = ToBitmapSource(frame);
                });
            }
            catch
            {
                // log here
            }
        }

        /// <summary>
        /// Converts Bitmap to BitmapSource.
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>BitmapSource</returns>
        private static BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                System.Windows.Media.PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            bitmapSource.Freeze();

            return bitmapSource;
        }

        /// <summary>
        /// Invoke <see cref="Action"/> in default UI thread.
        /// </summary>
        /// <param name="action"></param>
        private static void InvokeInUiThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    action();
                }
                catch
                {
                    // log here
                }
            }, DispatcherPriority.Normal);
        }

        #endregion
    }
}
