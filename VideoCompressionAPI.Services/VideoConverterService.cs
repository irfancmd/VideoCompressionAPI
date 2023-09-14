using NReco.VideoConverter;
using VideoCompressionAPI.Services.Interfaces;

namespace VideoCompressionAPI.Services
{
    public class VideoConverterService : IVideoConverterService
    {

        private readonly int MAX_FILE_SIZE = 2 * 1024 * 1024; // 2MB
        private readonly List<string> ALLOWED_FILE_TYPES = new List<string>()
        {
            ".webm"
        };

        public void ConvertVideo(string filePath, string outputDirectory, out string failedFilePath)
        {
            FileInfo videoFileInfo = new FileInfo(filePath);
            string fileNameWithoutExtension = videoFileInfo.Name.Substring(0, videoFileInfo.Name.LastIndexOf('.'));

            try
            {
                if (videoFileInfo.Length > MAX_FILE_SIZE && ALLOWED_FILE_TYPES.Contains(videoFileInfo.Extension))
                {
                    FFMpegConverter ffMpeg = new FFMpegConverter();
                    ffMpeg.ConvertMedia(
                        filePath,
                        null,
                        Path.Combine(outputDirectory, "temp.webm"),
                        Format.webm,
                        new ConvertSettings()
                        {
                            VideoFrameSize = FrameSize.qvga320x200,
                            CustomOutputArgs = "-vcodec libvpx -cpu-used -3 -deadline realtime"
                        }
                    );

                    // Delete the original file if conversion was successful (temp.webm exists)
                    if (File.Exists(Path.Combine(outputDirectory, "temp.webm")))
                    {
                        File.Delete(filePath);

                        // Change the name of the converted file to match that of the original one
                        File.Move(Path.Combine(outputDirectory, "temp.webm"), Path.Combine(outputDirectory, $"{fileNameWithoutExtension}.webm"));

                        failedFilePath = string.Empty;
                    }
                    else
                    {
                        failedFilePath = filePath;
                    }
                }
                else
                {
                    failedFilePath = string.Empty;
                }
            }
            catch (Exception)
            {
                failedFilePath = filePath;
            }
        }
    }
}
