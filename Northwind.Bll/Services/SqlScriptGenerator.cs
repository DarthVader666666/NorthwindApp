namespace Northwind.Bll.Services
{
    public static class SqlScriptGenerator
    {
        public static void GenerateOwnerScript(string path, string email, string password, string securityStamp, string concurrencyStamp)
        {
            if (path == null || email == null || password == null || securityStamp == null || concurrencyStamp == null)
            { 
                throw new ArgumentNullException();
            }

            var sqlScript = 
                $"INSERT INTO [dbo].[AspNetUsers] ([Id],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash]," +
                $"[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount]) " +
                $"VALUES ('a5dcae83-a142-4ca6-a4ca-a85ac595d471','{email}','{email.ToUpper()}','{email}','{email.ToUpper()}',1," +
                $"'{password}','{securityStamp}','{concurrencyStamp}',null,0,0,null,1,0) " +
                $"INSERT INTO [dbo].[AspNetRoles]([Id],[Name],[NormalizedName],[ConcurrencyStamp]) VALUES ('00fba8d9-b4d3-4ec4-b39f-887a5d35cbfd','owner'," +
                $"'OWNER',null) " +
                $"INSERT INTO [dbo].[AspNetUserRoles] ([UserId],[RoleId]) VALUES ('a5dcae83-a142-4ca6-a4ca-a85ac595d471', '00fba8d9-b4d3-4ec4-b39f-887a5d35cbfd')";

            if (!File.Exists(path))
            {
                using (var sw = new StreamWriter(path, false))
                {
                    sw.Write(sqlScript);
                }
            }
        }
    }
}
