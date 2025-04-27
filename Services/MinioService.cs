using Minio;
using Minio.DataModel.Args;

public class MinioService
{
    public class MinioSettings
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
    }
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly ILogger<MinioService> _logger;
    private readonly string _endpoint;
    private readonly string _mkdirProd = "product";

    public MinioService(MinioSettings settings, ILogger<MinioService> logger, IConfiguration configuration)
    {
        _endpoint = configuration["MinioSettings:Endpoint"];
        var accessKey = configuration["MinioSettings:AccessKey"];
        var secretKey = configuration["MinioSettings:SecretKey"];
        _bucketName = configuration["MinioSettings:BucketName"];
        _logger = logger;
        _logger.LogInformation($"Initializing MinIO with endpoint: {_endpoint}, bucket: {_bucketName}");
        _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)
                .Build();
    }

    public class FileUploadResult
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }

    public async Task<string> UploadFileAsync(IFormFile file)
        {
        try
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs()
                .WithBucket(_bucketName));
                // Генерируем уникальное имя файла
            string fileName = $"{_mkdirProd}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Загружаем файл в MinIO с помощью потока
            _logger.LogInformation($"Generated file name: {fileName}");

            // Копируем файл в память для надежности
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            _logger.LogInformation($"File copied to memory stream, size: {memoryStream.Length} bytes");

            // Загружаем файл в MinIO
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithContentType(file.ContentType));
                
            _logger.LogInformation($"File uploaded to MinIO");

            // Проверяем, что файл был загружен
            await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName));
                
            _logger.LogInformation($"File verified in MinIO");

            // Формируем URL
            string fileUrl = $"http://{_endpoint}/{_bucketName}/{fileName}";
            _logger.LogInformation($"Generated URL: {fileUrl}");
            
            return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file: {ex.Message}");
                throw;
            }
        }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from MinIO");
            throw;
        }
    }

    public async Task<string> GetFileUrlAsync(string fileName)
    {
        try
        {
            var presignedArgs = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithExpiry(60 * 60 * 24); // 24 часа

            return await _minioClient.PresignedGetObjectAsync(presignedArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file URL from MinIO");
            throw;
        }
    }
}