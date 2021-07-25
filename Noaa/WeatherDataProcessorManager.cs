using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Noaa
{
    public class WeatherDataProcessorManager : IWeatherDataProcessorManager
    {
        #region members
        private readonly IWeatherDataRemoteManager _weatherDataRemoteManager;
        private readonly IRepositoryManager _repositoryManager;
        #endregion

        #region properties
        public string ForecastDate { get; private set; }
        public int ForecastOffSet { get; private set; }
        #endregion

        #region ctor
        public WeatherDataProcessorManager(IWeatherDataRemoteManager weatherDataRemoteManager, IRepositoryManager repositoryManager)
        {
            _weatherDataRemoteManager = weatherDataRemoteManager;
            _repositoryManager = repositoryManager;
        }
        #endregion

        #region public methods
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
                var referenceDateWithoutMinutesAndSec = new DateTime(refernceDateTime.Year, refernceDateTime.Month, refernceDateTime.Day, refernceDateTime.Hour, 0, 0);
                var offsetBetweenForecastAndReferenceDateTime = (int)(parsedDateWithoutMinutesAndSec - referenceDateWithoutMinutesAndSec).TotalHours;                                
                ForecastDate = referenceDateWithoutMinutesAndSec.Year.ToString("0000") + referenceDateWithoutMinutesAndSec.Month.ToString("00") + referenceDateWithoutMinutesAndSec.Day.ToString("00");
                ForecastOffSet = offsetBetweenForecastAndReferenceDateTime + refernceDateTime.Hour;
            }
        }

        public async Task<decimal?> GetTemprature(int height, decimal lon, decimal lat)
        {
            var weatherDataPath = await GetWeatherDataPath();

            if (string.IsNullOrEmpty(weatherDataPath))
            {
                return null;
            }

            using var getTempProcess = new System.Diagnostics.Process();
            getTempProcess.StartInfo.FileName = @"c:\Noaa\wgrib2\wgrib2.exe";
            getTempProcess.StartInfo.Arguments = $" {weatherDataPath} -match \":(TMP:{height} m above ground):\" -lon {lon} {lat}";
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
        #endregion

        #region private methods
        private decimal ConvertKelvinToCelsius(decimal tempKelvin)
        {
            return tempKelvin - 273.15m;
        }

        private async Task<string> GetWeatherDataPath()
        {
            var dataPath = _repositoryManager.GetWatherDataPath(ForecastDate, ForecastOffSet);

            if (!_repositoryManager.IsWeatherDataExists(dataPath))
            {
                _repositoryManager.CreateDataPath(dataPath);

                await _weatherDataRemoteManager.GetWeatherDataAsFileAsync(ForecastDate, ForecastOffSet, dataPath);

                if (!_repositoryManager.IsWeatherDataExists(dataPath))
                {
                    return null;
                }
            }

            return dataPath;
        }

        #endregion
    }
}
