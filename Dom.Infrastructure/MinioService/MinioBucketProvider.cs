using Microsoft.AspNetCore.Http;
using Minio;
using System.Web;

namespace Taxiverk.Infrastructure.MinioService;

public class MinioBucketProvider<TMinioBucketConfiguration> where TMinioBucketConfiguration: MinioBucketConfiguration 
{
    private readonly MinioObjectFileStrategy _minioObjectFileStrategy;
    private readonly TMinioBucketConfiguration _bucket;
    private readonly MinioFactory _minioFactory;

    public MinioBucketProvider(TMinioBucketConfiguration bucket, MinioFactory minioFactory, MinioObjectFileStrategy minioObjectFileStrategy)
    {
        _minioObjectFileStrategy = minioObjectFileStrategy;
        _bucket = bucket;
        _minioFactory = minioFactory;
    }

    private async Task CreateBucketIfNotExist()
    {
        var beArgs = new BucketExistsArgs().WithBucket(_bucket.Name);
        
        bool bucketExist = await _minioFactory.Client.BucketExistsAsync(beArgs);
        
        if (!bucketExist)
        {
            var mbArgs = new MakeBucketArgs()
                .WithBucket(_bucket.Name);
            await _minioFactory.Client.MakeBucketAsync(mbArgs);
        }
    }

    public async Task<FilePersisted> PutFormFile(IFormFile file)
    {
        await CreateBucketIfNotExist();
        
        await using var stream = file.OpenReadStream();
        
        var fileName = _minioObjectFileStrategy(file.FileName);
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucket.Name)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(file.ContentType);
        
        await _minioFactory.Client.PutObjectAsync(putObjectArgs);

        var filePersisted = new FilePersisted
        {
            Created = DateTime.Now,
            Path = _bucket.Name + "/" + fileName,
            NameOriginal = file.FileName,
            Type = FilePersistType.Minio
        };

        return filePersisted;
    }
    public async Task<FilePersisted> PutByPath(string filePath)
    {
        await CreateBucketIfNotExist();
        
        await using var stream = new FileStream(filePath, FileMode.Open);

        var fileName = Path.GetFileName(filePath);
        
        var minioFileName = _minioObjectFileStrategy(fileName);
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucket.Name)
            .WithObject(minioFileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(MimeTypes.GetMimeType(filePath));
        
        await _minioFactory.Client.PutObjectAsync(putObjectArgs);

        var filePersisted = new FilePersisted
        {
            Created = DateTime.Now,
            Path = _bucket.Name + "/" + minioFileName,
            NameOriginal = fileName,
            Type = FilePersistType.Minio
        };

        return filePersisted;
    }
}


public class FilePersisted
{
    public string Path { get; set; }
    public string NameOriginal { get; set; }
    public DateTime Created { get; set; }
    public FilePersistType Type { get; set; }
    
}

public enum FilePersistType
{
    None,
    Minio
}