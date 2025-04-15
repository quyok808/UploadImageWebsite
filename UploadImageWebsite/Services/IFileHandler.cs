using UploadImageWebsite.Models;

namespace UploadImageWebsite.Services
{
	public interface IFileHandler
	{
		bool CanHandle(string contentType);
		Task<FileResponse> HandleAsync(IFormFile file, HttpRequest request);
		Task<IEnumerable<FileResponse>> ListFilesAsync(HttpRequest request);
	}
}
