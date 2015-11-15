using MonitorU.Database;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorU
{
    class DatabaseAccess : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/MonitorU.sdf";

        // Pass the connection string to the base class.
        public DatabaseAccess(string connectionString)
            : base(connectionString)
        { }

        // Specify a single table for the to-do items.
        public Table<ScreenObscurtionEvent> ScreenObscurtionEvents;
    }
}
