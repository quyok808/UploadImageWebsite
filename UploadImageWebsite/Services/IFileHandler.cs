using UploadImageWebsite.Models;

namespace UploadImageWebsite.Services
{
	public interface IFileHandler
	{
		bool CanHandle(string contentType);
		Task<FileResponse> HandleAsync(IFormFile file, HttpRequest request);
		Task<IEnumerable<FileResponse>> ListFilesAsync(HttpRequest request);
		Task<FileResponse> DeleteFilesAsync(string fileName, HttpRequest request);
		Task<FileResponse> UpdateFileAsync(string fileName, IFormFile file, HttpRequest request);
		Task<FileResponse> DeleteAllFileAsync(List<FileResponse> files, HttpRequest request);
	}
}
