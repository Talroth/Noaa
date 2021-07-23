using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Noaa
{
    public class S3WeatherDataRemoteManager : IWeatherDataRemoteManager
    {
        #region member
        private const string BUCKET_NAME = "noaa-gfs-bdp-pds";
        #endregion

        #region ctr
        public S3WeatherDataRemoteManager()
        {

        }
        #endregion

        #region public methods
        public Task GetWeatherDataAsFileAsync(string date, int offset, string dataPath)
        {
            Amazon.Runtime.AWSCredentials aWSCredentials = new AnonymousAWSCredentials();
            var config = new AmazonS3Config() { UseArnRegion = true, RegionEndpoint = Amazon.RegionEndpoint.USEast1 };
            IAmazonS3 s3Client = new AmazonS3Client(aWSCredentials, config);
            
            return Task.Run(() =>
            {
                using TransferUtility transferUtility = new TransferUtility(s3Client);
                return transferUtility.DownloadAsync(dataPath, BUCKET_NAME, $"gfs.{date}/00/atmos/gfs.t00z.pgrb2.0p25.f{offset.ToString("000")}");
            });
        }
        #endregion
    }
}
