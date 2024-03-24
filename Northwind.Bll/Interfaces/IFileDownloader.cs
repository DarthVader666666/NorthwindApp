namespace Northwind.Bll.Interfaces
{
    public interface IFileDownloader
    {
        Task DownloadScriptFileAsync(string? url = null, string? path = null);
    }
}