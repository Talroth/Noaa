using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Noaa
{
    public interface IWeatherDataProcessorManager
    {
        public string ForecastDate { get; }
        public int ForecastOffSet { get; }       
        void SetWeatherForecastDateTime(string dateTime, DateTime refernceDateTime);
        Task<decimal?> GetTemprature(int height, decimal lon, decimal lat);
    }
}
