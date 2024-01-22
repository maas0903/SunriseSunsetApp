using CosineKitty;
using Microsoft.AspNetCore.Components;
using SunriseSunsetApp.Data;

namespace SunriseSunsetApp.Components.Pages
{
    public class SeasonsBase : ComponentBase
    {
        SeasonsInfo seasons = Astronomy.Seasons(DateTime.Now.Year);
        int timeZone = 2;

        protected struct SeasonsLocal
        {
            public DateTime MarchEquinox;
            public DateTime JuneSolstice;
            public DateTime SeptemberEquinox;
            public DateTime DecemberSolstice;
        }

        protected SeasonsLocal seasonsLocalSouthAfrica;
        protected SeasonsLocal seasonsLocalBelgium;


        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);

            seasonsLocalSouthAfrica.MarchEquinox = seasons.mar_equinox.ToUtcDateTime();
            seasonsLocalSouthAfrica.MarchEquinox = seasonsLocalSouthAfrica.MarchEquinox.AddHours(timeZone);
            seasonsLocalSouthAfrica.JuneSolstice = seasons.jun_solstice.ToUtcDateTime();
            seasonsLocalSouthAfrica.JuneSolstice = seasonsLocalSouthAfrica.JuneSolstice.AddHours(timeZone);
            seasonsLocalSouthAfrica.SeptemberEquinox = seasons.sep_equinox.ToUtcDateTime();
            seasonsLocalSouthAfrica.SeptemberEquinox = seasonsLocalSouthAfrica.SeptemberEquinox.AddHours(timeZone);
            seasonsLocalSouthAfrica.DecemberSolstice = seasons.dec_solstice.ToUtcDateTime();
            seasonsLocalSouthAfrica.DecemberSolstice = seasonsLocalSouthAfrica.DecemberSolstice.AddHours(timeZone);

            timeZone = TimeZones.GetBelgiumTimeZone(seasonsLocalBelgium.MarchEquinox);
            seasonsLocalBelgium.MarchEquinox = seasons.mar_equinox.ToUtcDateTime();
            seasonsLocalBelgium.MarchEquinox = seasonsLocalBelgium.MarchEquinox.AddHours(timeZone);
            seasonsLocalBelgium.JuneSolstice = seasons.jun_solstice.ToUtcDateTime();
            seasonsLocalBelgium.JuneSolstice = seasonsLocalBelgium.JuneSolstice.AddHours(timeZone);
            seasonsLocalBelgium.SeptemberEquinox = seasons.sep_equinox.ToUtcDateTime();
            seasonsLocalBelgium.SeptemberEquinox = seasonsLocalBelgium.SeptemberEquinox.AddHours(timeZone);
            seasonsLocalBelgium.DecemberSolstice = seasons.dec_solstice.ToUtcDateTime();
            seasonsLocalBelgium.DecemberSolstice = seasonsLocalBelgium.DecemberSolstice.AddHours(timeZone);
        }
    }
}
