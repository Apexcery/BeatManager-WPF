﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BeatManager_WPF_.Enums;
using BeatManager_WPF_.Interfaces;
using BeatManager_WPF_.Models;
using BeatManager_WPF_.UserControls;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace BeatManager_WPF_
{
    public partial class MainWindow : Window
    {
        private readonly Config _config;
        private readonly IBeatSaverAPI _beatSaverApi;

        public MainWindow(Config config, IBeatSaverAPI beatSaverApi)
        {
            _config = config;
            _beatSaverApi = beatSaverApi;

            InitializeComponent();

            InitializeStartupPage();
        }

        private void InitializeStartupPage()
        {
            var startupPage = _config.StartupPage;
            var parseSuccessfully = Enum.TryParse(startupPage, true, out Config.Page startupPageEnum);

            if (!parseSuccessfully)
                return;

            switch (startupPageEnum)
            {
                case Config.Page.Songs:
                    BtnSongs.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                    {
                        RoutedEvent = Mouse.MouseUpEvent,
                        Source = this,
                    });
                    break;
            }
        }

        public MainWindow(Config config, IBeatSaverAPI beatSaverApi, string notifMessage, NotificationSeverityEnum severity) : this(config, beatSaverApi)
        {
            if (!string.IsNullOrEmpty(notifMessage))
            {
                this.Loaded += (o, args) => ShowNotification(o, args, notifMessage, severity);
            }
        }

        private void ShowNotification(object sender, RoutedEventArgs e, string message, NotificationSeverityEnum severity)
        {
            var notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    Application.Current.MainWindow,
                    Corner.BottomRight,
                    10,
                    10);
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            notifier.ShowSuccess(message, new MessageOptions { ShowCloseButton = true });
        }

        private void BtnOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            BtnCloseMenu.Visibility = Visibility.Visible;
            BtnOpenMenu.Visibility = Visibility.Collapsed;
            
        }

        private void BtnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            if (BtnCloseMenu.Visibility != Visibility.Collapsed && BtnOpenMenu.Visibility != Visibility.Visible)
            {
                BtnCloseMenu.Visibility = Visibility.Collapsed;
                BtnOpenMenu.Visibility = Visibility.Visible;
            }
        }

        private void WindowContent_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (BtnCloseMenu.Visibility != Visibility.Collapsed && BtnOpenMenu.Visibility != Visibility.Visible)
            {
                BtnCloseMenu.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void BtnPopUpExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #region Nav Buttons

        private void AddBorderToButton(ListViewItem btn)
        {
            var listOfButtons = NavButtonList;
            foreach (var item in listOfButtons.Items)
            {
                if (item.GetType() == typeof(ListViewItem) && ((ListViewItem)item).Name.StartsWith("Btn"))
                {
                    var button = (ListViewItem)item;
                    button.BorderThickness = new Thickness(0, 0, 0, 0);
                }
            }

            btn.BorderThickness = new Thickness(0, 0, 2, 0);
        }

        private void BtnSongs_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            AddBorderToButton((ListViewItem) sender);
            var songsControl = new Songs(_config, _beatSaverApi)
            {
                Width = this.WindowContent.Width,
                Height = this.WindowContent.Height
            };
            WindowContent.Children.Add(songsControl);
        }

        private void BtnPlaylists_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnAvatars_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnSabers_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnNotes_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnPlatforms_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnMods_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
