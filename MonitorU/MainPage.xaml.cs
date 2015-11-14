using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MonitorU.Resources;
using System.ComponentModel;
using MonitorU.Database;

namespace MonitorU
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructeur
        public MainPage()
        {
            InitializeComponent();

            // Set the page DataContext property to the DabaseAccess.
            this.DataContext = App.DatabaseAccess;

            // Exemple de code pour la localisation d'ApplicationBar
            //BuildLocalizedApplicationBar();

            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PhoneApplicationFrame rootFrame = App.Current.RootVisual as PhoneApplicationFrame;

            if (rootFrame != null)
            {
                rootFrame.Obscured += OnObscured;
                rootFrame.Unobscured += Unobscured;
            }
            
            // Call the base method.
            base.OnNavigatedTo(e);
        }

        void OnObscured(Object sender, ObscuredEventArgs e)
        {
            ScreenObscurtionEvent screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Obscurtion, Date = DateTime.Now };

            App.DatabaseAccess.AddScreenObscurtionEvent(screenEvent);
            txtObs.Text = "Obscured at " + DateTime.Now.ToString();
        }

        void Unobscured(Object sender, EventArgs e)
        {
            ScreenObscurtionEvent screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Unobscurtion, Date = DateTime.Now };

            App.DatabaseAccess.AddScreenObscurtionEvent(screenEvent);
            txtUnobs.Text = "Unobscured at " + DateTime.Now.ToString();
        }

        // Exemple de code pour la conception d'une ApplicationBar localisée
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Définit l'ApplicationBar de la page sur une nouvelle instance d'ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crée un bouton et définit la valeur du texte sur la chaîne localisée issue d'AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crée un nouvel élément de menu avec la chaîne localisée d'AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}