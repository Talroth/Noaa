using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Noaa
{
    public interface IWeatherDataRemoteManager
    {
        Task GetWeatherDataAsFileAsync(string date, int offset, string filePath);
    }
}
