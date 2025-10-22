using System.IO;

namespace CRM.Application.Helpers;

public static class GetContentTypeFromFile
{
    public static string GetContentType(string fileName)
    {
        string extensaoArquivo = Path.GetExtension(fileName).ToLowerInvariant();
        return extensaoArquivo switch
        {
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpeg" or ".jpg" => "image/jpeg",
            ".gif" => "image/gif",
            ".mp4" => "video/mp4",
            ".mkv" => "video/x-matroska",
            ".avi" => "video/x-msvideo",
            _ => "application/octet-stream",
        };
    }
}
