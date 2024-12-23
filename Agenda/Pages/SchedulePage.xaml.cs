using Agenda.SQL_Classes;

namespace Agenda;

public partial class SchedulePage : ContentPage
{

	public SchedulePage()
	{
		InitializeComponent();
        TimeIntervalDropDown.SelectedIndex = 1;
    }

    private void AddEventButtonPress(object sender, EventArgs e) //this navigates from the schedule page to add event page
	{
		Shell.Current.GoToAsync("AddEventPageRoute");
	}

    private void Reload(object sender, EventArgs e) // will select records based on given time interval
    {
		List<EventObject> events = new List<EventObject>();
		SQLiteTransfer read = new SQLiteTransfer();

		switch (TimeIntervalDropDown.SelectedIndex)
		{
			case 0:
				events = read.EventsInTimePeriod(1);
				break;
			case 1:
				events = read.EventsInTimePeriod(7);
				break;
			case 2:
				events = read.EventsInTimePeriod(30);
				break;
			case 3:
				events = read.EventsInTimePeriod(365);
                break;
		}
		EventStackLayout.Clear(); // clears the stack layout
		foreach (EventObject record in events) // calls display event for each event in the list
		{
			DisplayEvent(record);
		}

    }
	private void DisplayEvent(EventObject e) //formats events into vertical stack layout 
	{
		Grid grid = new Grid
		{
			RowDefinitions = {
				new RowDefinition { Height = new GridLength(100) },
				new RowDefinition { Height = new GridLength(50) }
			}
		};

        grid.Add(new Label { Text = e.Title, FontAttributes = FontAttributes.Bold, FontSize = 14, HorizontalOptions = LayoutOptions.Start},0,0);
		grid.Add(new Label { Text = e.Date.DateTicksToString(), HorizontalOptions = LayoutOptions.Start},0,1);
		grid.Add(new Label { Text = e.StartTime.TimeTicksToString(), HorizontalOptions = LayoutOptions.End }, 0, 1);

		Frame frame = new Frame
		{
			CornerRadius = 5,
			Content = grid
		};

		EventStackLayout.Add(frame);
	}
}

