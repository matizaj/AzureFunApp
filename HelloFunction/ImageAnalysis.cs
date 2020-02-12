using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.Storage.Blob;

namespace HelloFunction
{
    public static class ImageAnalysis
    {
        [FunctionName("ImageAnalysis")]
        public static async Task Run([BlobTrigger("images/{name}", Connection = "secondfunappstorage")]CloudBlockBlob myBlob, string name, ILogger log,
            [CosmosDB("imagescomsmos", "images", ConnectionStringSetting = "cosmosdb")] IAsyncCollector<FaceAnalysisResult> result)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{myBlob.Name} \n Size: {myBlob.Properties.Length} Bytes");
            var sas = GetSas(myBlob);
            var url = myBlob.Uri + sas;
            log.LogInformation($"Blob url is  {url}");
            var faces = await GetAnalysisAsync(url);
            await result.AddAsync(new FaceAnalysisResult { Faces = faces, ImageId = myBlob.Name });

        }

        public static string GetSas(CloudBlockBlob blob)
        {
            var sasPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-115),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(115)
            };


            var sas = blob.GetSharedAccessSignature(sasPolicy);
            return sas;
        }

        public static async Task<Face[]> GetAnalysisAsync(string url)
        {
            var client = new FaceServiceClient("dfe4b8dfc8e9476fa4e5175ad7417630", "https://pshellofaceapi.cognitiveservices.azure.com/");
            var types = new[] { FaceAttributeType.Gender };
            var result = await client.DetectAsync(url, false, false, types);
            return result;
        } 
    }
}
