namespace UploadImageWebsite.Models
{
	public class FileResponse
	{
		public string FileName { get; set; }
		public string Url { get; set; }

		// Bổ sung cho delete
		public bool Success { get; set; }
		public string Message { get; set; }
		public List<string> Files { get; set; }
	}
}
