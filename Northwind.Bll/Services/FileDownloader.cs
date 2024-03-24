using System.Net;

namespace Northwind.Bll.Services
{
    public static class FileDownloader
    {
        public static void DownloadScriptFileAsync(string? url, string? path)
        {
            using (var client = new WebClient())
            { 
                client.DownloadFile(url, path);
            }

            //using (var client = new HttpClient())
            //{
            //    using var stream = await client.GetStreamAsync(url);

            //    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            //    {
            //        await stream.CopyToAsync(fileStream);
            //    }
            //}
        }
    }
}
