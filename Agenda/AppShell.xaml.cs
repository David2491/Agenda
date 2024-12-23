using Agenda.Pages;

namespace Agenda;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("AddEventPageRoute", typeof(AddEventPage));//creates a new route to allow navigation to the add event page
		Routing.RegisterRoute("RoutePage", typeof(RoutePage)); //creates a route to the route choosing page. 
	}
}
