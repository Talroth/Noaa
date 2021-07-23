using Noaa;
using System;
using Xunit;

namespace NoaaTest
{
    public class WeatherDataProcessorManagerTest
    {
        //void SetWeatherForecastDateTime(string dateTime);
        //decimal GetTemprature(string dataPath, int height, decimal lon, decimal lat);

        [Fact]
        public void SetWeatherForecastDateTime_today_future_time()
        {
            var weatherDataProcessorManager = new WeatherDataProcessorManager(new S3WeatherDataRemoteManager(), new FileRepositoryManager());            
            weatherDataProcessorManager.SetWeatherForecastDateTime($"24/07/2021 17:23", new DateTime(2021, 7, 24, 16, 0, 0));            

            Assert.Equal($"20210724", weatherDataProcessorManager.ForecastDate);
            Assert.Equal(17, weatherDataProcessorManager.ForecastOffSet);
        }

        [Fact]
        public void SetWeatherForecastDateTime_today_past_time()
        {
            var weatherDataProcessorManager = new WeatherDataProcessorManager(new S3WeatherDataRemoteManager(), new FileRepositoryManager());            
            weatherDataProcessorManager.SetWeatherForecastDateTime($"24/07/2021 15:23", new DateTime(2021, 7, 24, 16, 0, 0));

            Assert.Equal($"20210724", weatherDataProcessorManager.ForecastDate);
            Assert.Equal(15, weatherDataProcessorManager.ForecastOffSet);
        }

        [Fact]
        public void SetWeatherForecastDateTime_yesterday()
        {
            var weatherDataProcessorManager = new WeatherDataProcessorManager(new S3WeatherDataRemoteManager(), new FileRepositoryManager());            
            weatherDataProcessorManager.SetWeatherForecastDateTime($"23/07/2021 15:23", new DateTime(2021, 7, 24, 16, 0, 0));

            Assert.Equal($"20210723", weatherDataProcessorManager.ForecastDate);
            Assert.Equal(15, weatherDataProcessorManager.ForecastOffSet);
        }

        [Fact]
        public void SetWeatherForecastDateTime_future()
        {
            var weatherDataProcessorManager = new WeatherDataProcessorManager(new S3WeatherDataRemoteManager(), new FileRepositoryManager());            
            weatherDataProcessorManager.SetWeatherForecastDateTime($"26/07/2021 16:23", new DateTime(2021, 7, 24, 16, 0, 0));

            Assert.Equal($"20210724", weatherDataProcessorManager.ForecastDate);
            Assert.Equal(64, weatherDataProcessorManager.ForecastOffSet);
        }
    }
}
