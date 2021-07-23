using System;
using System.Threading.Tasks;

namespace Noaa
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Need to download wgrib2.exe and save in c:\Noaa\wgrib2\ before start to use

            Console.WriteLine("Hello, please type date of the forecast (dd/MM/YYYY HH:MM UTC timezone 24hr format, e.g. 14/07/2021 16:13):");
            var dateTime = Console.ReadLine();
            Console.WriteLine("Enter LON value:");
            var lon = Console.ReadLine();
            Console.WriteLine("Enter LAT value:");
            var lat = Console.ReadLine();

            IWeatherDataProcessorManager weatherDataProcessorManager = new WeatherDataProcessorManager();
            IWeatherDataRemoteManager weatherDataRemoteManager = new S3WeatherDataRemoteManager();
            IRepositoryManager fileRepositoryManager = new FileRepositoryManager();

            try
            {
                weatherDataProcessorManager.SetWeatherForecastDateTime(dateTime, DateTime.UtcNow);

                var filePath = fileRepositoryManager.GetWatherDataPath(weatherDataProcessorManager.ForecastDate, weatherDataProcessorManager.ForecastOffSet);

                if (!fileRepositoryManager.IsWeatherDataExists(filePath))
                {
                    fileRepositoryManager.CreateDataPath(filePath);

                    await weatherDataRemoteManager.GetWeatherDataAsFileAsync(weatherDataProcessorManager.ForecastDate, weatherDataProcessorManager.ForecastOffSet, filePath);

                    if (!fileRepositoryManager.IsWeatherDataExists(filePath))
                    {
                        Console.WriteLine("Forecast can't be found");
                        return;
                    }
                }

                decimal temp = weatherDataProcessorManager.GetTemprature(filePath, 2, Convert.ToDecimal(lon), Convert.ToDecimal(lat));
                Console.WriteLine($"The temperature is: {temp} C");
            }
            catch
            {
                Console.WriteLine($"Fail to find the temperature (lon: {lon}, lat: {lat}, date: {dateTime})");
            }
        }
    }
}
