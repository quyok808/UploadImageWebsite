namespace UploadImageWebsite.Services
{
	public class FileHandlerFactory
	{
		private readonly IEnumerable<IFileHandler> _handlers;

		public FileHandlerFactory(IEnumerable<IFileHandler> handlers)
		{
			_handlers = handlers;
		}

		public IFileHandler GetHandler(string contentType)
		{
			var handler = _handlers.FirstOrDefault(h => h.CanHandle(contentType))
						  ?? _handlers.FirstOrDefault()
						  ?? throw new InvalidOperationException("No handler found.");
			return handler;
		}

		public IEnumerable<IFileHandler> GetAllHandlers()
		{
			return _handlers;
		}
	}
}
