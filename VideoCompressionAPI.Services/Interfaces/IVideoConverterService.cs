namespace VideoCompressionAPI.Services.Interfaces
{
    public interface IVideoConverterService
    {
        void ConvertVideo(string filePath, string outputDirectory, out string failedFilePath);
    }
}
