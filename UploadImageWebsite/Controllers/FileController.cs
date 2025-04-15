using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using UploadImageWebsite.Models;
using UploadImageWebsite.Services;

namespace UploadImageWebsite.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class FileController : ControllerBase
	{
		private readonly FileHandlerFactory _factory;

		public FileController(FileHandlerFactory factory)
		{
			_factory = factory;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var files = new List<FileResponse>();
				foreach (var handler in _factory.GetAllHandlers())
				{
					var handlerFiles = await handler.ListFilesAsync(Request);
					files.AddRange(handlerFiles);
				}
				return Ok(files);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("upload")]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			try
			{
				if (file == null || file.Length == 0)
					return BadRequest("No file uploaded.");

				var handler = _factory.GetHandler(file.ContentType);
				var result = await handler.HandleAsync(file, Request);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteImage([FromQuery] string fileName)
		{
			try
			{
				if (string.IsNullOrEmpty(fileName))
					return BadRequest("File name is required.");

				var extension = Path.GetExtension(fileName).ToLower();
				var contentType = extension.ToLower() switch
				{
					// Documents
					".pdf" => "application/pdf",
					".doc" => "application/msword",
					".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
					".xls" => "application/vnd.ms-excel",
					".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
					".ppt" => "application/vnd.ms-powerpoint",
					".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
					".txt" => "text/plain",
					".rtf" => "application/rtf",
					".odt" => "application/vnd.oasis.opendocument.text",

					// Images
					".jpg" or ".jpeg" => "image/jpeg",
					".png" => "image/png",
					".gif" => "image/gif",
					".bmp" => "image/bmp",
					".webp" => "image/webp",
					".svg" => "image/svg+xml",
					".ico" => "image/x-icon",

					// Audio
					".mp3" => "audio/mpeg",
					".wav" => "audio/wav",
					".ogg" => "audio/ogg",
					".m4a" => "audio/mp4",

					// Video
					".mp4" => "video/mp4",
					".avi" => "video/x-msvideo",
					".mov" => "video/quicktime",
					".wmv" => "video/x-ms-wmv",
					".webm" => "video/webm",
					".mkv" => "video/x-matroska",

					// Archives
					".zip" => "application/zip",
					".rar" => "application/vnd.rar",
					".7z" => "application/x-7z-compressed",
					".tar" => "application/x-tar",
					".gz" => "application/gzip",

					// Code / Web
					".html" or ".htm" => "text/html",
					".css" => "text/css",
					".js" => "application/javascript",
					".json" => "application/json",
					".xml" => "application/xml",
					".csv" => "text/csv",

					// Fonts
					".ttf" => "font/ttf",
					".otf" => "font/otf",
					".woff" => "font/woff",
					".woff2" => "font/woff2",

					// Default
					_ => "application/octet-stream"
				};


				if (contentType == null)
					return BadRequest("Unsupported file type.");

				var handler = _factory.GetHandler(contentType);
				var result = await handler.DeleteFilesAsync(fileName, Request);

				if (!result.Success)
					return NotFound(result.Message);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpPatch]
		public async Task<IActionResult> UpdateFile([FromQuery] string fileName, IFormFile file)
		{
			try
			{
				if (string.IsNullOrEmpty(fileName))
					return BadRequest("File name is required.");

				if (file == null || file.Length == 0)
					return BadRequest("New file is required.");

				var extension = Path.GetExtension(fileName).ToLower();
				var contentType = extension switch
				{
					// Documents
					".pdf" => "application/pdf",
					".doc" => "application/msword",
					".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
					".xls" => "application/vnd.ms-excel",
					".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
					".ppt" => "application/vnd.ms-powerpoint",
					".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
					".txt" => "text/plain",
					".rtf" => "application/rtf",
					".odt" => "application/vnd.oasis.opendocument.text",

					// Images
					".jpg" or ".jpeg" => "image/jpeg",
					".png" => "image/png",
					".gif" => "image/gif",
					".bmp" => "image/bmp",
					".webp" => "image/webp",
					".svg" => "image/svg+xml",
					".ico" => "image/x-icon",

					// Audio
					".mp3" => "audio/mpeg",
					".wav" => "audio/wav",
					".ogg" => "audio/ogg",
					".m4a" => "audio/mp4",

					// Video
					".mp4" => "video/mp4",
					".avi" => "video/x-msvideo",
					".mov" => "video/quicktime",
					".wmv" => "video/x-ms-wmv",
					".webm" => "video/webm",
					".mkv" => "video/x-matroska",

					// Archives
					".zip" => "application/zip",
					".rar" => "application/vnd.rar",
					".7z" => "application/x-7z-compressed",
					".tar" => "application/x-tar",
					".gz" => "application/gzip",

					// Code / Web
					".html" or ".htm" => "text/html",
					".css" => "text/css",
					".js" => "application/javascript",
					".json" => "application/json",
					".xml" => "application/xml",
					".csv" => "text/csv",

					// Fonts
					".ttf" => "font/ttf",
					".otf" => "font/otf",
					".woff" => "font/woff",
					".woff2" => "font/woff2",

					// Default
					_ => "application/octet-stream"
				};

				var handler = _factory.GetHandler(contentType);
				var result = await handler.UpdateFileAsync(fileName, file, Request);

				if (!result.Success)
					return NotFound(result.Message);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

	}
}
