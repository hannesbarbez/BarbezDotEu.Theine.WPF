// Copyright (c) Hannes Barbez. All rights reserved.
// Licensed under the GNU General Public License v3.0

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Theine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        #region Fields
        private bool disposedValue = false;
        private System.Threading.Timer keepAwakeTimer;
        #endregion

        #region Constants
        private readonly Key KEY = Key.F14;
        private const int KEEPAWAKEINTERVAL = 29 * 1000; // 29 seconds
        public const string MSSTORERATINGURL = @"ms-windows-store://review/?productid=9NTDTC99PP06";
        #endregion

        #region Methods: Keep Awake
        /// <summary>
        /// Keeps the computer from falling asleep
        /// </summary>
        /// <param name="state">(Ignored)</param>
        private void KeepAwakeTimer_Tick(object state)
        {
            try
            {
                // Different types of sending a key stroke for different kinds of devices?
                SendKeys.SendWait("{" + KEY.ToString() + "}"); // First type of sending a Key
                SimulateKeyboardInput(KEY); // Second type of sending a Key
                AnimateLogo();
            }
            catch { }
        }

        /// <summary>
        /// Simulates a specified key to this application
        /// </summary>
        /// <param name="key">Keyboard key to simulate</param>
        private void SimulateKeyboardInput(Key key)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (Keyboard.PrimaryDevice != null)
                    {
                        if (Keyboard.PrimaryDevice.ActiveSource != null)
                        {
                            var e = new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
                            {
                                RoutedEvent = Keyboard.KeyDownEvent
                            };
                            InputManager.Current.ProcessInput(e);
                        }
                    }
                });
            }
            catch { }
        }
        #endregion

        #region Startup
        /// <summary>
        /// Constructs the Main Window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeOthers();
        }

        /// <summary>
        /// Initializes other fields and properties needed by the application.
        /// </summary>
        private void InitializeOthers()
        {
            this.keepAwakeTimer = new System.Threading.Timer(new TimerCallback(KeepAwakeTimer_Tick), null, 0, KEEPAWAKEINTERVAL);
        }
        #endregion

        #region IDisposable Support
        /// <summary>
        /// Disposes of the timer.
        /// </summary>
        /// <param name="disposing">Set to true or false, depending on if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this.keepAwakeTimer.Dispose();
                    }
                    catch { }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Calls dispose methode whilst is disposing with the disposing argument set to true.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Methods: GUI
        /// <summary>
        /// Entertains the user by playing around with the logo's opacity.
        /// </summary>
        private void AnimateLogo()
        {
            this.Dispatcher.Invoke(() =>
            {
                iLogo.Opacity = Math.Round(.5 + (new Random().NextDouble() / 2), 1);
            });
        }

        /// <summary>
        /// Occurs whenever left mouse button is clicked.
        /// </summary>
        /// <param name="sender">(Ignored)</param>
        /// <param name="e">(Ignored)</param>
        private void LblAbout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(MSSTORERATINGURL);
        }

        /// <summary>
        /// Minimizes the <see cref="WindowState"/>.
        /// </summary>
        /// <param name="sender">(Ignored)</param>
        /// <param name="e">(Ignored)</param>
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
    }
}
