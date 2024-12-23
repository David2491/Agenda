using Agenda.SQL_Classes;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using PolylineEncoder.Net.Utility;

namespace Agenda.Pages;

public partial class RoutePage : ContentPage
{
	private int arrivalTime;
	private string RouteID = "";
    private TaskCompletionSource<string> Task;
	public RoutePage(long StartTime, long Date, TaskCompletionSource<string> output)
	{
		InitializeComponent();
		TransitMode.SelectedIndex = 0;
		arrivalTime = (int)((StartTime + Date - DateTime.UnixEpoch.Ticks) / 10000000); //converts ticks to seconds since Unix Epoch
        Task = output;
    }

	private void Cancel_Clicked(object sender, EventArgs e)
	{
        Task.SetResult("");
		Shell.Current.SendBackButtonPressed(); //Will Navigate back to add event page
	}
    private void ChooseRouteClicked(object sender, EventArgs e) //called when choose route button at the end of the page is pressed
    {
        if (RouteID != "") //if there is a route chosen
        {
            Task.SetResult(RouteID); //return the routeID
            Shell.Current.SendBackButtonPressed();
        }
        else //display an error if there is no route chosen
        {
            DisplayAlert("Error", "No Route Has been Chosen", "Close");
        }
    }

    private async void Submit(object sender, EventArgs e)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("Error", "There is No Network Connection", "Cancel"); //if there is no internet, a message is displayed and no request will be sent.
            return;
        }
        if (StartLocation.Text != "" && StartLocation.Text != null && EndLocation.Text != "" && EndLocation.Text != null)//if the start location and end location are not blank
		{
			string[][] routes = await MapsHelper.GetRoutes(StartLocation.Text, EndLocation.Text, arrivalTime, TransitMode.SelectedItem.ToString().ToLower());
			StackLayout.Clear(); //clear the stack layout
			foreach(string[] route in routes)
			{
                Button routeRepresentation = new Button
                {
                    HorizontalOptions = LayoutOptions.Center,
					Margin = 5,
                    Text = route[5] //button title will be the duration in text
                };
				routeRepresentation.Clicked += (sender , args) => ButtonProcedure(route[4], route[3], route[0]);
				StackLayout.Add(routeRepresentation);
			}
        }
    }

	private void ButtonProcedure(string steps, string lineData, string duration) //displays instructions to the user and the route line on the map
	{
        steps = steps.Remove(steps.Length - 1); //removes the final |
		string[] AllSteps = steps.Split('|');

        PlaceInstructions(AllSteps); //Puts instructions into the instructions box on the page. 

		PolylineDrawer(lineData); //draws the line on the map

        RouteID = StartLocation.Text + "|" + EndLocation.Text + "|" + TransitMode.SelectedItem.ToString().ToLower() + "|" + lineData.HashString() +"|"+ duration; //duration is in seconds. 
	}

	private void PolylineDrawer(string lineData)//Responsible for drawing the polyline on the map
	{
        Map.MapElements.Clear();
        Polyline line = new Polyline
        {
            StrokeWidth = 18,
            StrokeColor = Color.FromArgb("355C7D")
        };
        var utility = new PolylineUtility();
        Tuple<double, double>[] points = utility.DecodeAsTuples(lineData).ToArray();//turns the string line data into an array of tuples
        foreach (Tuple<double, double> coords in points)
        {
            line.Geopath.Add(new Location(coords.Item1, coords.Item2));
        }
        Map.MapElements.Add(line);//the line is added to the map. 
    }

	private void PlaceInstructions(string[] instructions) //responsible for placing instructions in the instructions box
	{
        InstructionsBox.Clear();
        for (int i = 0; i < instructions.Length; i++) 
        {
            Label step = new Label //create a new label
            {
                Text = (i + 1).ToString() + ": " + instructions[i]
            };
            InstructionsBox.Add(step);//add the label to the stack layout
        }
    }
}