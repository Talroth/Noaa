using System;
using System.Collections.Generic;
using System.Text;

namespace Noaa
{
    public interface IRepositoryManager
    {
        bool IsWeatherDataExists(string dataPath);
        string GetWatherDataPath(string date, int offset);
        void CreateDataPath(string dataPath);
    }
}
