using Microsoft.AspNetCore.Components;

namespace SunriseSunsetApp.Components.Pages
{
    public class AboutBase : ComponentBase
    {
        public static int year;
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);
            year = DateTime.Now.Year;

        }
    }
}
