using Agenda.SQL_Classes;

namespace Agenda;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private void ClearTableAndNotifications(object sender, EventArgs e)
    {
        SQLiteTransfer delete = new SQLiteTransfer();
        delete.clearTable(); //deletes all records in the Table
    }
}