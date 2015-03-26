namespace LightZDesktop
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Forms;

    using LightZDesktop.View;

    using Application = System.Windows.Application;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {

        #region Properties

        private NotifyIcon WindowNotifyIcon { get; set; }

        private ContextMenuStrip NotifyIconContextMenuStrip { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            this.InitializeComponent();

            this.NotifyIconContextMenuStrip = new ContextMenuStrip();
            this.NotifyIconContextMenuStrip.Items.Add("Paramètres", null, this.ContextMenuStripOpenClick);
            this.NotifyIconContextMenuStrip.Items.Add(new ToolStripSeparator());
            this.NotifyIconContextMenuStrip.Items.Add("Quitter", null, this.ContextMenuStripQuitClick);

            this.WindowNotifyIcon = new NotifyIcon();
            this.ChangeNotifyIcon("icon.ico");
            this.WindowNotifyIcon.ContextMenuStrip = this.NotifyIconContextMenuStrip;
            this.WindowNotifyIcon.Text = "LightZ";
            this.WindowNotifyIcon.Visible = true;
            this.WindowNotifyIcon.Click += this.WindowNotifyIcon_Click;
            this.WindowNotifyIcon.DoubleClick += this.ContextMenuStripOpenClick;
        }

        #endregion

        #region Handed Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden; // important!   
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.WindowNotifyIcon != null)
                this.WindowNotifyIcon.Visible = false;
        }

        private void WindowNotifyIcon_Click(object sender, EventArgs e)
        {
            MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(this.WindowNotifyIcon, null);
        }

        private void ContextMenuStripOpenClick(object sender, EventArgs e)
        {
            if (!this.WindowNotifyIcon.Visible)
                return;

            this.WindowNotifyIcon.Visible = false;
            var managerWindow = new SettingsWindow();
            managerWindow.ShowDialog();
            this.WindowNotifyIcon.Visible = true;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void ContextMenuStripQuitClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        #endregion

        #region Methods

        private void ChangeNotifyIcon(string file)
        {
            var streamResourceInfo = Application.GetResourceStream(new Uri(string.Format("pack://application:,,,/LightZDesktop;component/Resources/{0}", file)));
            if (streamResourceInfo == null)
                return;

            var iconStream = streamResourceInfo.Stream;
            this.WindowNotifyIcon.Icon = new Icon(iconStream);
            iconStream.Dispose();
        }

        #endregion
    }
}
