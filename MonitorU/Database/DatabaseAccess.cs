using MonitorU.Database;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MonitorU
{
    public class DatabaseAccess : INotifyPropertyChanged
    {
        // Data context for the local database
        private MonitorUDataContext eventsDB;

        // Class constructor, create the data context object.
        public DatabaseAccess(string toDoDBConnectionString)
        {
            eventsDB = new MonitorUDataContext(toDoDBConnectionString);
        }

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

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            eventsDB.SubmitChanges();
        }

        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {
            // Define the query to gather all of the events.
            var eventsInDB = from ScreenObscurtionEvent screenObscurtionEvent in eventsDB.ScreenObscurtionEvents
                             select screenObscurtionEvent;

            // Execute the query and place the results into a collection.
            Events = new ObservableCollection<ScreenObscurtionEvent>(eventsInDB);

            foreach (ScreenObscurtionEvent screenEvent in Events)
            {
                System.Diagnostics.Debug.WriteLine(screenEvent);
            }
        }

        // Add an event to the database and collections.
        public void AddScreenObscurtionEvent(ScreenObscurtionEvent newEvent)
        {
            // Add an event to the data context.
            eventsDB.ScreenObscurtionEvents.InsertOnSubmit(newEvent);

            // Save changes to the database.
            eventsDB.SubmitChanges();

            // Add an event to the observable collection.
            Events.Add(newEvent);
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
    }
}
