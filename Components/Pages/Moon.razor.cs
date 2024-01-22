using Microsoft.AspNetCore.Components;
using SunriseSunsetApp.Helpers;
using SunriseSunsetApp.Data;
using CosineKitty;

namespace SunriseSunsetApp.Components.Pages
{
    public class MoonBase : ComponentBase
    {
        protected SunAndMoonData dataYzerFontein;
        protected SunAndMoonData dataCapeTown;
        protected SunAndMoonData dataHeverlee;

        protected double phase;
        protected AstroTime? time;
        protected int timeZone = 2;
        protected DateTime localTime;
        protected string? quarterNameFine;
        protected IllumInfo illum;
        protected double phasePercentage;
        protected IEnumerable<MoonQuarterInfo>? mqs;
        protected IEnumerable<MoonQuarterInfo>? mqsCapeTown;
        protected IEnumerable<MoonQuarterInfo>? mqsHeverlee;

        public struct mqStruct
        {
            public MoonQuarterInfo MoonQuarter;
            public int TimeZone;
            public DateTime LocalTime;

            public mqStruct(MoonQuarterInfo MoonQuarter, int TimeZone)
            {
                this.MoonQuarter = MoonQuarter;
                this.TimeZone = TimeZone;
                LocalTime = MoonQuarter.time.ToUtcDateTime();
                LocalTime = LocalTime.AddHours(TimeZone);
            }
        }

        protected IEnumerable<mqStruct>? mqsStructYzerfontein;
        protected IEnumerable<mqStruct>? mqsStructCapeTown;
        protected IEnumerable<mqStruct>? mqsStructHeverlee;

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(500);

            time = new AstroTime(DateTime.Now);

            phase = Astronomy.MoonPhase(time);
            quarterNameFine = QuarterNameFine(phase);
            mqs = Astronomy.MoonQuartersAfter(time).Take(10);

            //Calculate the percentage of the Moon's disc that is illuminated from the Earth's point of view.
            illum = Astronomy.Illumination(Body.Moon, time);
            phasePercentage = illum.phase_fraction * 100.0;

            //Yzerfontein
            dataYzerFontein = MoonHelper.GetData(DateTime.Now, -33.35035690634893, 18.14970686674384, timeZone);
            localTime = time.ToUtcDateTime();
            localTime = localTime.AddHours(timeZone);
            mqsStructYzerfontein = mqs.Select(mq => new mqStruct(mq, timeZone));

            //Cape Town 
            dataCapeTown = MoonHelper.GetData(DateTime.Now, -33.93408625718356, 18.47711460013822, timeZone);
            mqsStructCapeTown = mqs.Select(mq => new mqStruct(mq, timeZone));


            //Heverlee
            timeZone = TimeZones.GetBelgiumTimeZone(DateTime.Now);
            dataHeverlee = MoonHelper.GetData(DateTime.Now, 50.853520152481615, 4.693098052258068, timeZone);
            localTime = time.ToUtcDateTime();
            localTime = localTime.AddHours(timeZone);
            mqsStructHeverlee = mqs.Select(mq => new mqStruct(mq, timeZone));
        }

        public static string QuarterName(int quarter)
        {
            switch (quarter)
            {
                case 0: return "New Moon";
                case 1: return "First Quarter";
                case 2: return "Full Moon";
                case 3: return "Third Quarter";
                default: return "INVALID QUARTER";
            }
        }

        static string QuarterNameFine(double degrees)
        {
            // Normalize the degrees to be within the range [0, 360)
            degrees = (degrees % 360 + 360) % 360;

            string[] moonPhases = { "New Moon",
                                "Waxing Crescent",
                                "First Quarter",
                                "Waxing Gibbous",
                                "Full Moon",
                                "Waning Gibbous",
                                "Last Quarter",
                                "Waning Crescent" };

            double[] phaseRanges = { 0, 45, 90, 135, 180, 225, 270, 315 };

            int phaseIndex = Array.FindIndex(phaseRanges, range => degrees < range);

            // If the degree is greater than or equal to 315, it corresponds to the New Moon phase
            if (degrees >= 315)
                phaseIndex = 0;

            return moonPhases[phaseIndex];
        }

        static int MoonQuarter(AstroTime time)
        {
            double q = Astronomy.MoonPhase(time) / 90.0;
            return (int)q;
        }
    }
}
