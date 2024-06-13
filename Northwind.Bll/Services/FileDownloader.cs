using Microsoft.Extensions.Configuration;

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

        public static async Task GenerateOwnerScriptAsync(ConfigurationManager config)
        {
            var ownerScriptPath = config["OwnerScriptPath"];
            var ownerEmail = config["OwnerEmail"];
            var ownerPasswordHash = config["OwnerPasswordHash"];
            var ownerSecurityStamp = config["OwnerSecurityStamp"];
            var ownerConcurrencyStamp = config["OwnerConcurrencyStamp"];

            var sqlScript =
                $"INSERT INTO [dbo].[AspNetUsers] ([Id],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash]," +
                $"[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount]) " +
                $"VALUES ('a5dcae83-a142-4ca6-a4ca-a85ac595d471','{ownerEmail}','{ownerEmail?.ToUpper()}','{ownerEmail}','{ownerEmail?.ToUpper()}',1," +
                $"'{ownerPasswordHash}','{ownerSecurityStamp}','{ownerConcurrencyStamp}',null,0,0,null,1,0) " +
                $"INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('00fba8d9-b4d3-4ec4-b39f-887a5d35cbfd','owner'," +
                $"'OWNER',null) " +
                $"INSERT INTO [dbo].[AspNetUserRoles] ([UserId],[RoleId]) VALUES ('a5dcae83-a142-4ca6-a4ca-a85ac595d471', '00fba8d9-b4d3-4ec4-b39f-887a5d35cbfd')";

            if (!File.Exists(ownerScriptPath))
            {
                using var writer = new StreamWriter(ownerScriptPath!, false);
                await writer.WriteAsync(sqlScript);
            }
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
