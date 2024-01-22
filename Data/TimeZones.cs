using Google.TimeZoneApi;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net;

namespace SunriseSunsetApp.Data
{

    //test this class

    public static class TimeZones
    {
        public static int GetBelgiumTimeZone(DateTime InputDate)
        {
            DateTime lastSundayOfMarch = TimeZones.GetLastSundayOfMarch();
            DateTime lastSundayOfOctober = TimeZones.GetLastSundayOfOctober();
            int timezone;

            if (InputDate.Date == lastSundayOfMarch.Date || InputDate.Date == lastSundayOfOctober.Date)
            {
                // //Is it 2 o'clock in the morning?
                // if (InputDate.Hour == 2)
                // {
                //     //Add or subtract an hour
                //     timezone = timezone == 2 ? 1 : 2;
                // }
                timezone = TimeZones.GetGoogleTimeZone(50.853520152481615, 4.693098052258068);
            }
            else
            {
                DateTime marchChangeDate = new DateTime(lastSundayOfMarch.Year, lastSundayOfMarch.Month, lastSundayOfMarch.Day, 2, 0, 0);
                DateTime octoberChangeDate = new DateTime(lastSundayOfOctober.Year, lastSundayOfOctober.Month, lastSundayOfOctober.Day, 2, 0, 0);

                if (InputDate < marchChangeDate || InputDate > octoberChangeDate)
                {
                    timezone = 1;
                }
                else
                {
                    timezone = 2;
                }

                ////Does not work as expected
                //if (InputDate.IsDaylightSavingTime())
                //{
                //    timezone = 2;
                //}
                //else
                //{
                //    timezone = 1;
                //}
            }
            return timezone;
        }

        // ********************************************************************************************************************
        // https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.local
        // https://en.wikipedia.org/wiki/List_of_tz_database_time_zones

        // Use the List table in https://en.wikipedia.org/wiki/List_of_tz_database_time_zones to build a table with TZ identifier and DST ans STD UTC Offset
        // Use the following code to get the list of time zones
        public static void GetTimeZones()
        {
            ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo zone in zones)
            {
                Console.WriteLine(zone.Id);
                Console.WriteLine(zone.BaseUtcOffset.Hours);

            }
        }

        public static bool IsDaylightSavingActive(string timeZoneId)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            if (cstZone.SupportsDaylightSavingTime && !cstZone.IsDaylightSavingTime(DateTime.UtcNow))
            {
                //DateTime dateTime = ;
            }
            else
            {
                ;
            }
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);
            return cstZone.IsDaylightSavingTime(cstTime);
        }
        // ********************************************************************************************************************

        public static DateTime GetLastSundayOfMarch()
        {
            DateTime marchFirst = new DateTime(DateTime.Now.Year, 3, 1);
            DateTime lastDayOfMarch = marchFirst.AddMonths(1).AddDays(-1);
            int daysUntilLastSunday = (int)lastDayOfMarch.DayOfWeek;

            return lastDayOfMarch.AddDays(-daysUntilLastSunday);
        }

        public static DateTime GetLastSundayOfOctober()
        {
            DateTime octoberFirst = new DateTime(DateTime.Now.Year, 10, 1);
            DateTime lastDayOfOctober = octoberFirst.AddMonths(1).AddDays(-1);
            int daysUntilLastSunday = (int)lastDayOfOctober.DayOfWeek;

            return lastDayOfOctober.AddDays(-daysUntilLastSunday);
        }


        public static int GetGoogleTimeZone(double latitude, double longitude)
        {
            string url = "https://maps.googleapis.com/maps/api/timezone/json?location=" + latitude + "," + longitude + "&timestamp=" + GetCurrentUnixTime() + "&key=" + "AIzaSyC1uccC-JTxEMdy_VT7eTbm6fZQiNsHO6I";
            string json = new WebClient().DownloadString(url);
            GoogleTimeZoneResult googleTimeZoneResult = JsonConvert.DeserializeObject<GoogleTimeZoneResult>(json);
            TimeZoneInfo.TryFindSystemTimeZoneById(googleTimeZoneResult.TimeZoneName, out TimeZoneInfo timeZoneInfo);
            return timeZoneInfo.BaseUtcOffset.Hours;
        }

        public static async Task<int> GetGoogleTimeZoneWithClientAsync(double latitude, double longitude)
        {

            //var request = new HttpRequestMessage(HttpMethod.Get,
            //    "https://maps.googleapis.com/maps/api/timezone/json?location=" + latitude + "," + longitude + "&timestamp=" + GetCurrentUnixTime() + "&key=" + "AIzaSyC1uccC-JTxEMdy_VT7eTbm6fZQiNsHO6I");
            //request.Headers.Add("Accept", "application/vnd.github.v3+json");
            //request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            //var client = ClientFactory.CreateClient();

            HttpClient httpClient = new HttpClient();

            try
            {

                //    using HttpResponseMessage response = await httpClient.GetAsync("https://maps.googleapis.com/maps/api/timezone/json?location=" + latitude + "," + longitude + "&timestamp=" + GetCurrentUnixTime() + "&key=" + "AIzaSyC1uccC-JTxEMdy_VT7eTbm6fZQiNsHO6I");
                //response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                string uri = "https://maps.googleapis.com/maps/api/timezone/json?location=" + latitude + "," + longitude + "&timestamp=" + GetCurrentUnixTime() + "&key=" + "AIzaSyC1uccC-JTxEMdy_VT7eTbm6fZQiNsHO6I";

                string responseBody = await httpClient.GetStringAsync(uri);

                Console.WriteLine(responseBody);

                GoogleTimeZoneResult googleTimeZoneResult = JsonConvert.DeserializeObject<GoogleTimeZoneResult>(responseBody);
                TimeZoneInfo.TryFindSystemTimeZoneById(googleTimeZoneResult.TimeZoneName, out TimeZoneInfo timeZoneInfo);
                return timeZoneInfo.BaseUtcOffset.Hours;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return 0;
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        static long GetCurrentUnixTime()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return now.ToUnixTimeSeconds();
        }
    }
}
