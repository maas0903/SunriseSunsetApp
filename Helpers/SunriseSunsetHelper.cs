namespace SunriseSunsetApp.Helpers
{
    public class SunriseSunsetHelper
    {
        private const double SunriseSunsetAltitude = -35d / 60d;
        private const double CivilTwilightAltitude = -6d;
        private const double NauticalTwilightAltitude = -12d;
        private const double AstronomicalTwilightAltitude = -18d;

        public static void SunriseSunset(DateTime Date, double Latitude, double Longitude, int TimeZone, out double SunriseHours, out double SunsetHours)
        {
            SunriseSunset(Date.Year, Date.Month, Date.Day, Longitude, Latitude, TimeZone, SunriseSunsetAltitude, true, out SunriseHours, out SunsetHours);
        }

        public static void CivilTwilight(DateTime Date, double Latitude, double Longitude, int TimeZone, out double SunriseHours, out double SunsetHours)
        {
            SunriseSunset(Date.Year, Date.Month, Date.Day, Longitude, Latitude, TimeZone, CivilTwilightAltitude, false, out SunriseHours, out SunsetHours);
        }

        public static void NauticalTwilight(DateTime Date, double Latitude, double Longitude, int TimeZone, out double SunriseHours, out double SunsetHours)
        {
            SunriseSunset(Date.Year, Date.Month, Date.Day, Longitude, Latitude, TimeZone, NauticalTwilightAltitude, false, out SunriseHours, out SunsetHours);
        }

        public static void AstronomicalTwilight(DateTime Date, double Latitude, double Longitude, int TimeZone, out double SunriseHours, out double SunsetHours)
        {
            SunriseSunset(Date.Year, Date.Month, Date.Day, Longitude, Latitude, TimeZone, AstronomicalTwilightAltitude, false, out SunriseHours, out SunsetHours);
        }


        /// A function to compute the number of days elapsed since 2000 Jan 0.0 
        /// (which is equal to 1999 Dec 31, 0h UT)  
        private static long DaysSince2000Jan0(int Y, int M, int D)
        {
            return 367L * Y - 7 * (Y + (M + 9) / 12) / 4 + 275 * M / 9 + D - 730530L;
        }

        /* Some conversion factors between radians and degrees */
        private const double RadDeg = 180.0 / Math.PI;
        private const double DegRad = Math.PI / 180.0;

        /* The trigonometric functions in degrees */
        private static double SinD(double x)
        {
            return Math.Sin(x * DegRad);
        }

        private static double CosD(double x)
        {
            return Math.Cos(x * DegRad);
        }

        private static double TanD(double x)
        {
            return Math.Tan(x * DegRad);
        }

        private static double AtanD(double x)
        {
            return RadDeg * Math.Atan(x);
        }

        private static double AsinD(double x)
        {
            return RadDeg * Math.Asin(x);
        }

        private static double AcosD(double x)
        {
            return RadDeg * Math.Acos(x);
        }

        private static double Atan2D(double y, double x)
        {
            return RadDeg * Math.Atan2(y, x);
        }

        /// Note: Year,Month,Date = calendar Date, 1801-2099 only.             
        /// Eastern longitude positive, Western longitude negative       
        /// Northern latitude positive, Southern latitude negative       
        /// The longitude value IS critical in this function! 

        /// <param name="SunAltitudeDegrees">
        /// the altitude which the Sun should cross
        /// Set to -35/60 degrees for rise/set, -6 degrees
        /// for civil, -12 degrees for nautical and -18
        /// degrees for astronomical twilight.
        /// </param>

        /// <param name="UpperLimb">
        /// true -> upper limb, false -> center
        /// Set to true (e.g. 1) when computing rise/set
        /// times, and to false when computing start/end of twilight.
        /// </param>
        /// 
        /// <param name="RiseHours">where to store the rise time</param>
        /// 
        /// <param name="SetHours">where to store the set time</param>
        /// 
        /// <returns>
        ///  0	=	sun rises/sets this Day, times stored at RiseHours and SetHours
        /// +1	=	sun above the specified "horizon" 24 hours.
        ///			RiseHours set to time when the sun is at south,
        ///			minus 12 hours while *SetHours is set to the south
        ///			time plus 12 hours. "Day" length = 24 hours
        /// -1	=	sun is below the specified "horizon" 24 hours
        ///			"Day" length = 0 hours, *RiseHours and *SetHours are
        ///			both set to the time when the sun is at south.
        /// </returns>
        private static int SunriseSunset(int Year, int Month, int Day, double Longitude, double Latitude, int TimeZone,
                         double SunAltitudeDegrees, bool UpperLimb, out double RiseHours, out double SetHours)
        {
            double d;          /* Days since 2000 Jan 0.0 (negative before) */
            double solarDistance;         /* Solar distance, astronomical units */
            double sunRightAscension;        /* Sun's Right Ascension */
            double sunDeclination;       /* Sun's declination */
            double sunRadius;    /* Sun's apparent radius */
            double diurnalArc;          /* Diurnal arc */
            double sunSouthTime;     /* Time when Sun is at south */
            double siderealTime;    /* Local sidereal time */

            int rc = 0; /* Return cde from function - usually 0 */

            /* Compute D of 12h local mean solar time */
            d = DaysSince2000Jan0(Year, Month, Day) + 0.5 - Longitude / 360.0;

            /* Compute the local sidereal time of this moment */
            siderealTime = revolution(GMST0(d) + 180.0 + Longitude);

            /* Compute Sun's RA, Decl and distance at this moment */
            sun_RA_dec(d, out sunRightAscension, out sunDeclination, out solarDistance);

            /* Compute time when Sun is at south - in hours UT */
            sunSouthTime = 12.0 - rev180(siderealTime - sunRightAscension) / 15.0;

            /* Compute the Sun's apparent radius in degrees */
            sunRadius = 0.2666 / solarDistance;

            /* Do correction to upper limb, if necessary */
            if (UpperLimb)
                SunAltitudeDegrees -= sunRadius;

            /* Compute the diurnal arc that the Sun traverses to reach */
            /* the specified altitude SunAltitudeDegrees: */
            {
                double cost;
                cost = (SinD(SunAltitudeDegrees) - SinD(Latitude) * SinD(sunDeclination)) /
                (CosD(Latitude) * CosD(sunDeclination));
                if (cost >= 1.0) /* Sun always below SunAltitudeDegrees */
                {
                    rc = -1;
                    diurnalArc = 0.0;
                }
                else if (cost <= -1.0) /* Sun always above SunAltitudeDegrees */
                {
                    rc = +1;
                    diurnalArc = 12.0;
                }
                else
                {
                    diurnalArc = AcosD(cost) / 15.0;   /* The diurnal arc, hours */
                }
            }

            /* Store rise and set times - in hours UT */
            RiseHours = sunSouthTime - diurnalArc + TimeZone;
            SetHours = sunSouthTime + diurnalArc + TimeZone;

            return rc;
        }

        /// <param name="upper_limb">
        /// true -> upper limb, true -> center
        /// Set to true (e.g. 1) when computing Day length
        /// and to false when computing Day+twilight length.
        /// </param>
        public static double DayLen(int year, int month, int day, double lon, double lat,
                          double altit, bool upper_limb)
        {
            double d;          /* Days since 2000 Jan 0.0 (negative before) */
            double obl_ecl;    /* Obliquity (inclination) of Earth's axis */
            double sr;         /* Solar distance, astronomical units */
            double slon;       /* True solar longitude */
            double sin_sdecl;  /* Sine of Sun's declination */
            double cos_sdecl;  /* Cosine of Sun's declination */
            double sradius;    /* Sun's apparent radius */
            double t;          /* Diurnal arc */

            /* Compute D of 12h local mean solar time */
            d = DaysSince2000Jan0(year, month, day) + 0.5 - lon / 360.0;

            /* Compute obliquity of ecliptic (inclination of Earth's axis) */
            obl_ecl = 23.4393 - 3.563E-7 * d;

            /* Compute Sun's ecliptic longitude and distance */
            sunpos(d, out slon, out sr);

            /* Compute sine and cosine of Sun's declination */
            sin_sdecl = SinD(obl_ecl) * SinD(slon);
            cos_sdecl = Math.Sqrt(1.0 - sin_sdecl * sin_sdecl);

            /* Compute the Sun's apparent radius, degrees */
            sradius = 0.2666 / sr;

            /* Do correction to upper limb, if necessary */
            if (upper_limb)
            {
                altit -= sradius;
            }

            /* Compute the diurnal arc that the Sun traverses to reach */
            /* the specified altitude SunAltitudeDegrees: */
            double cost = (SinD(altit) - SinD(lat) * sin_sdecl) / (CosD(lat) * cos_sdecl);

            /* Sun always below SunAltitudeDegrees */
            if (cost >= 1.0)
            {
                t = 0.0;
            }
            /* Sun always above SunAltitudeDegrees */
            else if (cost <= -1.0)
            {
                t = 24.0;
            }
            /* The diurnal arc, hours */
            else
            {
                t = 2.0 / 15.0 * AcosD(cost);
            }

            return t;
        }

        /// <summary>
        /// Computes the Sun's ecliptic longitude and distance 
        /// at an instant given in D, number of days since
        /// 2000 Jan 0.0.  The Sun's ecliptic latitude is not
        /// computed, since it's always very near 0.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="lon"></param>
        /// <param name="r"></param>
        private static void sunpos(double d, out double lon, out double r)
        {
            double M;         /* Mean anomaly of the Sun */
            double w;         /* Mean longitude of perihelion */
            /* Note: Sun's mean longitude = M + w */
            double e;         /* Eccentricity of Earth's orbit */
            double E;         /* Eccentric anomaly */
            double x, y;      /* x, Y coordinates in orbit */
            double v;         /* True anomaly */

            /* Compute mean elements */
            M = revolution(356.0470 + 0.9856002585 * d);
            w = 282.9404 + 4.70935E-5 * d;
            e = 0.016709 - 1.151E-9 * d;

            /* Compute true longitude and radius vector */
            E = M + e * RadDeg * SinD(M) * (1.0 + e * CosD(M));
            x = CosD(E) - e;
            y = Math.Sqrt(1.0 - e * e) * SinD(E);
            r = Math.Sqrt(x * x + y * y);       /* Solar distance */
            v = Atan2D(y, x);                   /* True anomaly */
            lon = v + w;                        /* True solar longitude */
            if (lon >= 360.0)
            {
                lon -= 360.0;                   /* Make it 0..360 degrees */
            }
        }

        /// <summary>
        /// Computes the Sun's equatorial coordinates RA, Decl
        /// and also its distance, at an instant given in D,
        /// the number of days since 2000 Jan 0.0.
        /// </summary>
        private static void sun_RA_dec(double d, out double RA, out double dec, out double r)
        {
            double lon, obl_ecl, x, y, z;

            /* Compute Sun's ecliptical coordinates */
            sunpos(d, out lon, out r);

            /* Compute ecliptic rectangular coordinates (z=0) */
            x = r * CosD(lon);
            y = r * SinD(lon);

            /* Compute obliquity of ecliptic (inclination of Earth's axis) */
            obl_ecl = 23.4393 - 3.563E-7 * d;

            /* Convert to equatorial rectangular coordinates - x is unchanged */
            z = y * SinD(obl_ecl);
            y = y * CosD(obl_ecl);

            /* Convert to spherical coordinates */
            RA = Atan2D(y, x);
            dec = Atan2D(z, Math.Sqrt(x * x + y * y));
        }

        private const double INV360 = 1.0d / 360.0d;

        /// <summary>
        /// This function reduces any angle to within the first revolution
        /// by subtracting or adding even multiples of 360.0 until the
        /// result is >= 0.0 and < 360.0
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double revolution(double x)
        {
            return x - 360.0 * Math.Floor(x * INV360);
        }

        /// <summary>
        /// Reduce angle to within +180..+180 degrees
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double rev180(double x)
        {
            return x - 360.0 * Math.Floor(x * INV360 + 0.5);
        }

        /// <summary>
        /// This function computes GMST0, the Greenwich Mean Sidereal Time  
        /// at 0h UT (i.e. the sidereal time at the Greenwhich meridian at  
        /// 0h UT).  GMST is then the sidereal time at Greenwich at any     
        /// time of the Day.  I've generalized GMST0 as well, and define it 
        /// as:  GMST0 = GMST - UT  --  this allows GMST0 to be computed at 
        /// other times than 0h UT as well.  
        /// 
        /// While this sounds somewhat contradictory, it is very practical:
        /// instead of computing  GMST like:
        /// GMST = (GMST0) + UT * (366.2422/365.2422)                                                                                     
        /// where (GMST0) is the GMST last time UT was 0 hours, one simply  
        /// computes: GMST = GMST0 + UT                                                                                                          
        /// where GMST0 is the GMST "at 0h UT" but at the current moment! 
        /// 
        /// Defined in this way, GMST0 will increase with about 4 min a     
        /// Day.  It also happens that GMST0 (in degrees, 1 hr = 15 degr)   
        /// is equal to the Sun's mean longitude plus/minus 180 degrees!    
        /// (if we neglect aberration, which amounts to 20 seconds of arc   
        /// or 1.33 seconds of time)    
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double GMST0(double d)
        {
            double sidtim0;
            /* Sidtime at 0h UT = L (Sun's mean longitude) + 180.0 degr  */
            /* L = M + w, as defined in sunpos().  Since I'M too lazy to */
            /* add these numbers, I'll let the C compiler do it for me.  */
            /* Any decent C compiler will add the constants at compile   */
            /* time, imposing no runtime or code overhead.               */
            sidtim0 = revolution(180.0 + 356.0470 + 282.9404 + (0.9856002585 + 4.70935E-5) * d);
            return sidtim0;
        }
    }
}
