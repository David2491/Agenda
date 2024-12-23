using System.Diagnostics;
using System.Text.Json;

namespace Agenda.SQL_Classes
{
    public static class MapsHelper
    {
        private static HttpClient client = new HttpClient();
        private const string url = "https://maps.googleapis.com/maps/api/directions/json";
        private const string apiKey = "GOOGLE_DIRECTIONS_API_KEY";

        public static async Task<string[][]> GetRoutes(string origin, string destination, int TimeOfArrival, string TransitMode)
        {
            //the appropriate request is made to the google maps directions api
            var apiUrl = $"{url}?origin={origin}&destination={destination}&key={apiKey}&alternatives=true&arrival_time={TimeOfArrival}&mode={TransitMode}";
            var response = await client.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();

            var toReturn = new List<string[]>(); // a list which holds string arrays
            JsonDocument json = JsonDocument.Parse(content); 
            JsonElement routes = json.RootElement.GetProperty("routes");
            foreach(JsonElement route in routes.EnumerateArray())//for each route given in routes
            {
                string[] routeData = new string[6]; //an empty string array which will hold 5 values:
                JsonElement mainData = route.GetProperty("legs")[0];
                routeData[0] = mainData.GetProperty("duration").GetProperty("value").GetInt32().ToString(); //duration of certain route in seconds

                JsonElement arrivalTime;
                if(mainData.TryGetProperty("arrival_time",out arrivalTime))//tries to get arrival time if it exists (does not exist for driving transport mode)
                {
                    routeData[1] = arrivalTime.GetProperty("text").GetString(); //if it exists, it is put in the array
                }
                else {
                    routeData[1] = ""; //if it doesn't, an empty string is stored
                }

                routeData[2] = mainData.GetProperty("distance").GetProperty("text").GetString(); //distance of route
                routeData[3] = route.GetProperty("overview_polyline").GetProperty("points").GetString();//get the entire line that represents the route
                foreach(JsonElement step in mainData.GetProperty("steps").EnumerateArray())
                {
                    routeData[4] += HTMLRemover(step.GetProperty("html_instructions").GetString()) + "|"; // | will be a character we split the string based on 
                }
                routeData[5] = mainData.GetProperty("duration").GetProperty("text").GetString(); //text representation of duration
                toReturn.Add(routeData);//the string array is appended to the list
            }
            return toReturn.ToArray(); //the final list containing string arrays is converted to a 2d string array
        }

        public static async Task<int> GetRouteTime(EventObject evnt)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                return -1; // if there is no internet, return -1
            }
            string[] RouteData = evnt.RouteId.Split("|");

            int arrivalTime = (int)((evnt.StartTime + evnt.Date - DateTime.UnixEpoch.Ticks) / 10000000);//calculate arrival time in seconds since UNIX Epoch

            var apiUrl = $"{url}?origin={RouteData[0]}&destination={RouteData[1]}&key={apiKey}&alternatives=true&arrival_time={arrivalTime}&mode={RouteData[2]}";
            var response = await client.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync(); //sends and recieves request

            JsonDocument json = JsonDocument.Parse(content);
            JsonElement routes = json.RootElement.GetProperty("routes");
            foreach (JsonElement route in routes.EnumerateArray())
            {
                JsonElement mainData = route.GetProperty("legs")[0];
                if(route.GetProperty("overview_polyline").GetProperty("points").GetString().HashString() == RouteData[3]) //checks the hashes of the polylines. 
                {
                    return mainData.GetProperty("duration").GetProperty("value").GetInt32(); //Returns the ascociated route's estimated journey time
                }
            }
            return -1; // if this point is reached, the route has not been found 
        }

        private static string HTMLRemover(string text) //steps are given with HTML tags which should be removed
        {
            string returnString = "";
            int substringIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    returnString += text.Substring(substringIndex, i - substringIndex);
                }
                else if (text[i] == '>')
                {
                    substringIndex = i + 1;
                }
            }
            returnString += text.Substring(substringIndex, text.Length - substringIndex);
            return returnString;
        }
    }
}
