using System.Windows;

namespace Faceberry.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Lifecycle methods

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            base.OnStartup(e);
        }

        #endregion
    }
}
