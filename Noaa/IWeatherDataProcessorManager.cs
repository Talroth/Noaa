using System;
using System.Collections.Generic;
using System.Text;

namespace Noaa
{
    public interface IWeatherDataProcessorManager
    {
        public string ForecastDate { get; }
        public int ForecastOffSet { get; }       
        void SetWeatherForecastDateTime(string dateTime, DateTime refernceDateTime);
        decimal GetTemprature(string dataPath, int height, decimal lon, decimal lat);
    }
}
