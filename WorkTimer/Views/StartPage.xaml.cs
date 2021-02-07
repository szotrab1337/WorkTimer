using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkTimer.Models;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkTimer.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();

            Messenger.Default.Register<NotificationMessage<LocalNotification>>(this, LocalNotificationMessage);
        }

        public void ShowLocalNotification(int Duration, string Content)
        {
            LocalNotification.Show(Content, Duration);
        }

        public void LocalNotificationMessage(NotificationMessage<LocalNotification> message)
        {
            if(message.Notification == "NewLocalNotification")
                ShowLocalNotification(message.Content.Duration, message.Content.Content);
        }
    }
}
