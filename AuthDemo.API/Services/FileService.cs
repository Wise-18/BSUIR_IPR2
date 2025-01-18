namespace AuthDemo.API.Services
{
    public interface IFileService
    {
        Task<string> SaveAvatarAsync(IFormFile file, string userId);
        Task DeleteAvatarAsync(string filePath);
    }

    public class FileService : IFileService
    {
        private readonly string _uploadsPath;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _uploadsPath = Path.Combine(environment.WebRootPath, "uploads", "avatars");
            _logger = logger;

            if (!Directory.Exists(_uploadsPath))
                Directory.CreateDirectory(_uploadsPath);
        }

        public async Task<string> SaveAvatarAsync(IFormFile file, string userId)
        {
            try
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{userId}{fileExtension}";
                var filePath = Path.Combine(_uploadsPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return $"/uploads/avatars/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving avatar for user {UserId}", userId);
                throw;
            }
        }

        public Task DeleteAvatarAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_uploadsPath, Path.GetFileName(filePath));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting avatar {FilePath}", filePath);
                throw;
            }
        }
    }
}
