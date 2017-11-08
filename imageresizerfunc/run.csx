#r "Microsoft.Azure.WebJobs.Extensions.EventGrid"
#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host.Bindings.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using ImageResizer;
using ImageResizer.ExtensionMethods;

static string storageAccountConnectionString = System.Environment.GetEnvironmentVariable("myBlobStorage_STORAGE");
static string assetContainerName = System.Environment.GetEnvironmentVariable("myContainerName");

public static async Task Run(EventGridEvent myEvent, Stream inputBlob, TraceWriter log)
{
    log.Info(myEvent.ToString());

    // Get the blobname from the event's JObject.
    string blobName = GetBlobNameFromUrl((string)myEvent.Data["url"]);

    var webAddr = "http://mockbin.org/echo";
    var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
    httpWebRequest.ContentType = "application/json; charset=utf-8";
    httpWebRequest.Method = "POST";

    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
    {
        string json = $"{{\"name\":\"{blobName}\", \"url\": \"{(string)myEvent.Data["url"]}\"}}";

        streamWriter.Write(json);
    }

    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
    {
        var result = streamReader.ReadToEnd();
        log.Info(result);
    }

    // Retrieve storage account from connection string.
    // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

    // Create the blob client.
    // CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

    // Retrieve reference to a previously created container.
    // CloudBlobContainer container = blobClient.GetContainerReference(assetContainerName);

    // Create reference to a blob named "blobName".
    // CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);


}
private static string GetBlobNameFromUrl(string bloblUrl)
{
    var myUri = new Uri(bloblUrl);
    var myCloudBlob = new CloudBlob(myUri);
    return myCloudBlob.Name;
}
