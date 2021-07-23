using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Noaa
{
    public class WeatherDataProcessorManager : IWeatherDataProcessorManager
    {
        public string ForecastDate { get; private set; }
        public int ForecastOffSet { get; private set; }        

        public void SetWeatherForecastDateTime(string dateTimeUTC, DateTime refernceDateTime)
        {            
            DateTime parsedDate = DateTime.ParseExact(dateTimeUTC, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            if (parsedDate < refernceDateTime)
            {
                ForecastDate = parsedDate.Year.ToString("0000") + parsedDate.Month.ToString("00") + parsedDate.Day.ToString("00");
                ForecastOffSet = parsedDate.Hour;                
            }
            else
            {
                var parsedDateWithoutMinutesAndSec = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, parsedDate.Hour, 0, 0);                
                var nowateWithoutMinutesAndSec = new DateTime(refernceDateTime.Year, refernceDateTime.Month, refernceDateTime.Day, refernceDateTime.Hour, 0, 0);
                var offsetBetweenForecastAndReferenceDateTime = (int)(parsedDateWithoutMinutesAndSec - nowateWithoutMinutesAndSec).TotalHours;                                
                ForecastDate = nowateWithoutMinutesAndSec.Year.ToString("0000") + nowateWithoutMinutesAndSec.Month.ToString("00") + nowateWithoutMinutesAndSec.Day.ToString("00");
                ForecastOffSet = offsetBetweenForecastAndReferenceDateTime + refernceDateTime.Hour;
            }
        }

        public decimal GetTemprature(string dataPath, int height, decimal lon, decimal lat)
        {
            using var getTempProcess = new System.Diagnostics.Process();
            getTempProcess.StartInfo.FileName = @"c:\Noaa\wgrib2\wgrib2.exe";
            getTempProcess.StartInfo.Arguments = $" {dataPath} -match \":(TMP:{height} m above ground):\" -lon {lon} {lat}";
            getTempProcess.StartInfo.RedirectStandardOutput = true;
            getTempProcess.Start();

            string tempKelvinString = string.Empty;
            while (!getTempProcess.StandardOutput.EndOfStream)
            {
                string line = getTempProcess.StandardOutput.ReadLine();
                var indexOfTemp = line.IndexOf("val=");
                tempKelvinString = line.Substring(indexOfTemp + 4);                
            }

            getTempProcess.Close();         

            decimal tempKelvin = decimal.Parse(tempKelvinString, NumberStyles.Float);
            return ConvertKelvinToCelsius(tempKelvin);
        }

        private decimal ConvertKelvinToCelsius(decimal tempKelvin)
        {
            return tempKelvin - 273.15m;
        }
    }
}
