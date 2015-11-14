using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MonitorU.Database
{
    enum Type { Obscurtion, Unobscurtion }

    [Table]
    public class ScreenObscurtionEvent : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private long _eventId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "LONG NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long EventId
        {
            get
            {
                return _eventId;
            }
            set
            {
                if (_eventId != value)
                {
                    NotifyPropertyChanging("EventId");
                    _eventId = value;
                    NotifyPropertyChanged("EventId");
                }
            }
        }

        private Type _type;

        [Column]
        public Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    NotifyPropertyChanging("Type");
                    _type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        private DateTime _date;

        [Column]
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (!_date.Equals(value))
                {
                    NotifyPropertyChanging("Date");
                    _date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
