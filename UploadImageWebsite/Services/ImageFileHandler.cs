using UploadImageWebsite.Models;

namespace UploadImageWebsite.Services
{
	public class ImageFileHandler : IFileHandler
	{
		public bool CanHandle(string contentType) => contentType.StartsWith("image/");

		public Task<FileResponse> DeleteAllFileAsync(List<FileResponse> files, HttpRequest request)
		{
			var _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
			if (!Directory.Exists(_storagePath))
			{
				return Task.FromResult(new FileResponse
				{
					Success = false,
					Message = $"Folder not found.",
				});
			}
			foreach (var file in files)
			{
				var filePath = Path.Combine(_storagePath, file.FileName);
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}

			return Task.FromResult(new FileResponse
			{
				Success = true,
				Message = "All files deleted successfully"
			});
		}

		public Task<FileResponse> DeleteFilesAsync(string fileName, HttpRequest request)
		{
			var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
			if (!Directory.Exists(uploadsPath))
			{
				return Task.FromResult(new FileResponse
				{
					Success = false,
					Message = $"Not found.",
					Files = new List<string> { fileName }
				});
			}
			var filePath = Path.Combine(uploadsPath, fileName);

			System.IO.File.Delete(filePath);

			return Task.FromResult(new FileResponse
			{
				Success = true,
				Message = $"File '{fileName}' has been deleted.",
				Files = new List<string> { }
			});
		}

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
				Success = true,
				Message = $"File {fileName} has been created",
				FileName = fileName,
				Url = $"{request.Scheme}://{request.Host}/uploads/{fileName}",
			};
		}

		public Task<IEnumerable<FileResponse>> ListFilesAsync(HttpRequest request)
		{
			var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
			if (!Directory.Exists(uploadsPath))
			{
				return Task.FromResult(Enumerable.Empty<FileResponse>());
			}

			// Define common image file extensions
			var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

			var files = Directory.GetFiles(uploadsPath)
				.Where(filePath => imageExtensions.Contains(Path.GetExtension(filePath).ToLower()))
				.Select(filePath => new FileResponse
				{
					FileName = Path.GetFileName(filePath),
					Url = $"{request.Scheme}://{request.Host}/Uploads/{Path.GetFileName(filePath)}"
				});

			return Task.FromResult(files);
		}

		public async Task<FileResponse> UpdateFileAsync(string fileName, IFormFile file, HttpRequest request)
		{
			var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

			if (!Directory.Exists(uploadsPath))
			{
				return new FileResponse
				{
					Success = false,
					Message = $"Upload directory not found.",
					Files = new List<string> { fileName }
				};
			}

			var filePath = Path.Combine(uploadsPath, fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return new FileResponse
				{
					Success = false,
					Message = $"File '{fileName}' not found.",
					Files = new List<string> { fileName }
				};
			}

			try
			{
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				return new FileResponse
				{
					Success = true,
					Message = $"File '{fileName}' has been updated successfully.",
					Files = new List<string> { fileName },
					Url = $"{request.Scheme}://{request.Host}/uploads/{fileName}"
				};
			}
			catch (Exception ex)
			{
				return new FileResponse
				{
					Success = false,
					Message = $"Error updating file: {ex.Message}",
					Files = new List<string> { fileName }
				};
			}
		}
	}
}
