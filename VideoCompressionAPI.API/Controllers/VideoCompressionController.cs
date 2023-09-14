using Microsoft.AspNetCore.Mvc;
using VideoCompressionAPI.API.Enums;
using VideoCompressionAPI.API.Models;
using VideoCompressionAPI.Services.Interfaces;

namespace VideoCompressionAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoCompressionController : ControllerBase
    {
        private IVideoConverterService _videoConverterService;
        private IWebHostEnvironment _webHostEnvironment;

        public VideoCompressionController(IWebHostEnvironment environment, IVideoConverterService videoConverterService)
        {
            _videoConverterService = videoConverterService;
            _webHostEnvironment = environment;
        }

        [HttpPost]
        public IActionResult CompressVideo([FromForm] VideoCompressionRequestModel requestModel)
        {
            string videoPath = Path.Combine(_webHostEnvironment.ContentRootPath, requestModel.Path);

            if (Directory.Exists(videoPath))
            {
                string[] videoFileNames = Directory.GetFiles(videoPath);

                foreach (string fileName in videoFileNames)
                {
                    _videoConverterService.ConvertVideo(fileName, videoPath, out string failedFilePath);

                    if (failedFilePath != string.Empty)
                    {
                        return BadRequest(new VideoCompressionResponseModel()
                        {
                            StatusCode = ((int)CompressionStatusCode.Failed),
                            Message = $"ERROR: Failed to convert the video: {failedFilePath}"
                        });
                    }
                }

                return Ok(new VideoCompressionResponseModel()
                {
                    StatusCode = ((int)CompressionStatusCode.Success),
                    Message = "Successfully compressed all videos in the given directory."
                });
            }
            else
            {
                return BadRequest(new VideoCompressionResponseModel()
                {
                    StatusCode = ((int)CompressionStatusCode.Failed),
                    Message = $"ERROR: The provided video directory, {videoPath} is not accessible or does not exist."
                });
            }
        }
    }
}
