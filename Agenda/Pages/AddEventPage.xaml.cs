using Agenda.SQL_Classes;
using Microsoft.Maui.Controls.Internals;
using SQLitePCL;

namespace Agenda;

public partial class AddEventPage : ContentPage
{
	private string routeID = "";
	public AddEventPage()
	{
		InitializeComponent();
		Date.MinimumDate = DateTime.Now; //Makes sure the Date picker will only allow user to select dates after and including current date
		StartTime.Time = DateTime.Now.TimeOfDay; //Sets the default time (of the start and end times) to the user's current time
		EndTime.Time = DateTime.Now.TimeOfDay;
		RecurringDropDown.SelectedIndex = 0;//selected index on xaml side is bugged. This sorts it out. 

	}

	private void Cancel_Clicked(object sender, EventArgs e)
	{
		Shell.Current.SendBackButtonPressed(); //Will Go back to schedule page if delete is pressed
	}

	private async void SendtoRoutePage(object sender, EventArgs e)
	{
		if(ValidateTimes() == true)//ensures start times are valid
		{
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            await Navigation.PushAsync(new Pages.RoutePage(StartTime.Time.Ticks, Date.Date.Ticks, tcs));//going to routepage, passing in the date, start time and tcs
			routeID = await tcs.Task;
        }
	}

    private void ValidateEntryData(object sender, EventArgs e)
    {
		if(Title.Text == null || Title.Text == "") //Makes sure Title entry field is not empty and will prompt user if so
		{
			DisplayAlert("Error", "Title Field is Empty","Ok");
			return;
		}
		else if (Title.Text.Length > 32) //Sanitises the Title to make sure it is not longer than 32 characters
		{
			Title.Text = Title.Text.Substring(0, 32);
		}
		if((Description.Text != null && Description.Text != "") && Description.Text.Length > 128) //if a description is present sanitises to to make sure it is not longer than 128 characters
		{
			Description.Text = Description.Text.Substring(0, 128);
		}

		if(ValidateTimes() == true) //will got to validate times
		{
            SaveEvent(); //Calls the saving of the event
        }
    }
	private void SaveEvent()
	{
		SQLiteTransfer link = new SQLiteTransfer(); //initialises link the to the table
		EventObject NewEvent = new EventObject(); // creates an empty event which is populated with all the data from the Add Event Page
		NewEvent.Title = Title.Text;
		NewEvent.Description = Description.Text;
		NewEvent.Date = Date.Date.Ticks; //Date is now stored as number of ticks
		NewEvent.StartTime = StartTime.Time.Ticks;//Start and end times are now also stored as ticks
		NewEvent.EndTime = EndTime.Time.Ticks;
		NewEvent.Recurring = RecurringDropDown.SelectedIndex;
		NewEvent.IsAlarm = isAlarm.IsToggled;
		if(routeID != "")
		{
			NewEvent.IsVariable = true; //if routeID has a value then it will be a variable event
        }
		else
		{
			NewEvent.IsVariable = false; //if not, it will be a regular event
        }
		NewEvent.RouteId = routeID;
		link.AddRecord(NewEvent);
        Shell.Current.SendBackButtonPressed(); 
    }

	private bool ValidateTimes()
	{
        if (StartTime.Time.CompareTo(EndTime.Time) == 1) //Makes sure that the start time is not later than the end time
        {
            DisplayAlert("Error", "Start Time Cannot Be Later Than End Time", "Ok");
            return false;
        }
        if (StartTime.Time.CompareTo(DateTime.Now.TimeOfDay) == -1 && Date.Date == DateTime.Now.Date) //Makes sure that the start time cannot be earlier than the current time
                                                                                                      //not needed for end time 
        {
            DisplayAlert("Error", "Start Time Cannot Be Earlier than Current Time", "Ok");
			return false;
        }
		return true;
    }
}