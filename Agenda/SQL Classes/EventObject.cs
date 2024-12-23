using SQLite;

namespace Agenda.SQL_Classes
{
    public class EventObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } //Primary key field which will increment when a new event is added. 

        [MaxLength(32)]
        public string Title { get; set; } //Title field 

        [MaxLength(128)]
        public string Description { get; set; } //Description Field

        public long Date { get; set; } //Date in ticks field

        public long StartTime { get; set; } //Start time field

        public long EndTime { get; set;} //End time Field

        public int Recurring { get; set; } //An integer representing if the event repeats at all and if so how frequently

        public bool IsAlarm { get; set; } //If false, the event uses a notification, if true, the event uses an alarm

        public bool IsVariable { get; set; } //indicates when an event uses the variable time mode

        public string RouteId { get; set; } //this may need to be changed if route Ids do not exist. An id which represesnts which route the user has chosen. 
    }
}
