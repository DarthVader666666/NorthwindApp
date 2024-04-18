namespace Northwind.Bll.Services
{
    public static class FileDownloader
    {
        public static async Task<string> DownloadScriptFileAsync(string? url, string? path)
        {
            var script = "";

            using (var client = new HttpClient())
            {
                using var stream = await client.GetStreamAsync(url);

                using (var fileStream = new FileStream(path!, FileMode.Create, FileAccess.ReadWrite))
                {
                    await stream.CopyToAsync(fileStream);
                    script = await ReadText(fileStream);
                }
            }

            return script;
        }

        public static byte[] DownloadImage(string path)
        { 
            var image = new byte[0];

            if (File.Exists(path))
            { 
                image = File.ReadAllBytes(path);
            }

            return image;
        }

        private static async Task<string> ReadText(FileStream fileStream)
        {
            var script = "";

            using (StreamReader reader = new StreamReader(fileStream))
            {
                if (fileStream.CanRead)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    script = await reader.ReadToEndAsync();
                }
            }

            return script;
        }
    }
}
