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
	}
}
