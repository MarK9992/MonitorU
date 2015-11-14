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
using System.Collections.ObjectModel;
using MonitorU.Database;

namespace MonitorU
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        // Data context for the local database
        private DatabaseAccess eventsDB;

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<ScreenObscurtionEvent> _events;
        public ObservableCollection<ScreenObscurtionEvent> Events
        {
            get
            {
                return _events;
            }
            set
            {
                if (_events != value)
                {
                    _events = value;
                    NotifyPropertyChanged("Events");
                }
            }
        }

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            
            // Connect to the database and instantiate data context.
            eventsDB = new DatabaseAccess(DatabaseAccess.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;
            
            // Exemple de code pour la localisation d'ApplicationBar
            //BuildLocalizedApplicationBar();

            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            PhoneApplicationFrame rootFrame = App.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
            {
                rootFrame.Obscured += OnObscured;
                rootFrame.Unobscured += Unobscured;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PhoneApplicationFrame rootFrame = App.Current.RootVisual as PhoneApplicationFrame;

            if (rootFrame != null)
            {
                rootFrame.Obscured += OnObscured;
                rootFrame.Unobscured += Unobscured;
            }
            
            // Define the query to gather all of the to-do items.
            var eventsInDB = from ScreenObscurtionEvent screenObscurtionEvent in eventsDB.ScreenObscurtionEvents
                                select screenObscurtionEvent;

            // Execute the query and place the results into a collection.
            Events = new ObservableCollection<ScreenObscurtionEvent>(eventsInDB);

            foreach (ScreenObscurtionEvent screenEvent in Events)
            {
                System.Diagnostics.Debug.WriteLine(screenEvent);
            }
            
            // Call the base method.
            base.OnNavigatedTo(e);
        }

        void OnObscured(Object sender, ObscuredEventArgs e)
        {
            ScreenObscurtionEvent screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Obscurtion, Date = DateTime.Now };

            Events.Add(screenEvent);
            eventsDB.ScreenObscurtionEvents.InsertOnSubmit(screenEvent);
            eventsDB.SubmitChanges();
            txtObs.Text = "Obscured at " + DateTime.Now.ToString();
        }

        void Unobscured(Object sender, EventArgs e)
        {
            ScreenObscurtionEvent screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Unobscurtion, Date = DateTime.Now };

            Events.Add(screenEvent);
            eventsDB.ScreenObscurtionEvents.InsertOnSubmit(screenEvent);
            eventsDB.SubmitChanges();
            txtUnobs.Text = "Unobscured at " + DateTime.Now.ToString();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

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