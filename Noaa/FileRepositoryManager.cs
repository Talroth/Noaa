using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Noaa
{
    public class FileRepositoryManager : IRepositoryManager
    {
        #region ctr
        public FileRepositoryManager()
        {

        }
        #endregion

        #region public methods

        public void CreateDataPath(string dataPath)
        {
            System.IO.Directory.CreateDirectory(dataPath.Substring(0, dataPath.LastIndexOf('\\')));
        }

        public string GetWatherDataPath(string date, int offset)
        {            
            if (string.IsNullOrEmpty(date))
            {
                throw new ArgumentException("Missing date of forecast");
            }

            var dataPath = @$"c:\Noaa\{date}\gfs.t00z.pgrb2.0p25.f{offset}";
            return dataPath.Replace("file://", "");
        }

        public bool IsWeatherDataExists(string dataPath)
        {
            return File.Exists(dataPath);
        }

        #endregion
    }
}
