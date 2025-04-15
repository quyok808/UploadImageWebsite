using UploadImageWebsite.Models;

namespace UploadImageWebsite.Services
{
	public class ImageFileHandler : IFileHandler
	{
		public bool CanHandle(string contentType) => contentType.StartsWith("image/");

		public async Task<FileResponse> HandleAsync(IFormFile file, HttpRequest request)
		{
			var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
			if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

			var fileName = "IMG_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
			var path = Path.Combine(uploadsPath, fileName);

			using var stream = new FileStream(path, FileMode.Create);
			await file.CopyToAsync(stream);

			return new FileResponse
			{
				FileName = fileName,
				Url = $"{request.Scheme}://{request.Host}/uploads/{fileName}"
			};
		}

		public Task<IEnumerable<FileResponse>> ListFilesAsync(HttpRequest request)
		{
			var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
			if (!Directory.Exists(uploadsPath))
			{
				return Task.FromResult(Enumerable.Empty<FileResponse>());
			}

			var files = Directory.GetFiles(uploadsPath)
				.Select(filePath => new FileResponse
				{
					FileName = Path.GetFileName(filePath),
					Url = $"{request.Scheme}://{request.Host}/Uploads/{Path.GetFileName(filePath)}"
				});

			return Task.FromResult(files);
		}
	}
}
