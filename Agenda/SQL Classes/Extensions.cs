using Android.Media;
using System.Security.Cryptography;

namespace Agenda
{
    public static class Extensions //Now methods can be added which can be accessed across the entire program
    {
        public static string DateTicksToString(this long time)
        {
            DateTime date = new DateTime(time);
            return date.ToString("dd-MM-yyyy");
        }
        public static string TimeTicksToString(this long time)
        {
            TimeSpan timeOfDay = new TimeSpan(time);
            return timeOfDay.ToString(@"hh\:mm");//retuns time in format hour:minute
        }

        public static string HashString(this string str) //hashes a string and returns it
        {
            SHA256 sha = SHA256.Create();
            byte[] ByteArray = System.Text.Encoding.Default.GetBytes(str);
            byte[] Hashed = sha.ComputeHash(ByteArray);
            return Convert.ToBase64String(Hashed);
        }
    }
}
