using SQLite;
using System.Globalization;
using System.Linq;

namespace Agenda.SQL_Classes
{
    class SQLiteTransfer
    {
        SQLiteConnection connection;
        
        public SQLiteTransfer()
        {
            connection = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "EventsData.db"));//creates a new SQLite connection to the file EventsData.db
            connection.CreateTable<EventObject>(); //creates the table if it does not exist already
        }
        public void AddRecord(EventObject Event) //Simply inserts a record into the Table
        {
            connection.Insert(Event);
        }
        public void DeleteRecord(EventObject Evnt) //Deletes a record from the table based on its Primary Key
        {
            connection.Delete(Evnt);
        }

        public void clearTable() //temporary testing code
        {
            connection.DeleteAll<EventObject>();
        }
        public EventObject GetEvent(int EventId)
        {
            var Event = from r in connection.Table<EventObject>() where r.Id == EventId select r;
            return Event.First();
        }

        public void IncrementEventDate(int days, int id)
        {
            EventObject evnt = GetEvent(id); //gets the event
            evnt.Date += Math.BigMul(days * 60 * 60, 10000000 * 24); // adds a number of days to the date of the event
            UpdateEvent(evnt);

        }

        public void UpdateEvent(EventObject evnt) //updates an event in the table that has the same Primary key as the event it is being updated with
        {
            connection.Update(evnt);
        }

        public List<EventObject> GetRecords()//Temporary code for testing 
        {
            List<EventObject> records = new List<EventObject>();
            records = connection.Table<EventObject>().ToList();//Returns the entire table
            return records;
        }
        public List<EventObject> EventsInTimePeriod(int rangeOfDays) //This function can be used for all time periods
        {
            long maxDayInTicks = DateTime.Now.Ticks + Math.BigMul(rangeOfDays * 60 * 60, 10000000 * 24); //A time period which is an certain number of days away
            // Big Mul must be used as the numbers are long (64-bits) and not ints (32-bits) 
            var EventsToday = from r in connection.Table<EventObject>() where r.Date < maxDayInTicks orderby r.Date select r;
            return EventsToday.ToList<EventObject>();
        }

        public EventObject GetMostRecentEvent()
        {
            if(connection.Table<EventObject>().Count() == 0)
            {
                return null;//If the table is empty, no event will be returned
            }

            EventObject minimum = new EventObject();
            minimum.Date = DateTime.MaxValue.Ticks; //assuming the minimum start date is the maximum possible value means
            //as long as the table is not empty, an event will be found. 
            foreach(EventObject e in connection.Table<EventObject>())
            {
                if(e.Date+e.StartTime < minimum.Date + minimum.StartTime)
                {
                    minimum = e;
                }
            }
            return minimum;
        }
    }
}
