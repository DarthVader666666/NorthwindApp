using Microsoft.Extensions.Configuration;
using Northwind.Bll.Interfaces;

namespace Northwind.Bll.Services
{
    public class FileDownloader : IFileDownloader
    {
        private readonly string? scriptPath;
        private readonly string? scriptUrl;

        public FileDownloader(IConfiguration configuration)
        {
            scriptPath = configuration["ScriptPath"];
            scriptUrl = configuration["ScriptUrl"];
        }

        public async Task DownloadScriptFileAsync(string? url = null, string? path = null)
        {
            url ??= scriptUrl;
            path ??= scriptPath;

            if (url == null || path == null)
            {
                throw new ArgumentNullException("Path or URL of Seed Database Sql script is null.");
            }

            using (var client = new HttpClient())
            {
                using var stream = await client.GetStreamAsync(url);

                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
