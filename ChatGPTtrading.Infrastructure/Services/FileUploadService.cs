using Amazon.S3;
using Amazon.S3.Transfer;
using ChatGPTtrading.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace ChatGPTtrading.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _configuration;

        public FileUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileSystem(IFormFile file)
        {
            return await UploadFileAws(file);
        }

        private bool HasImageExtension(string source)
        {
            var allowedExts = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            return allowedExts.Any(x => x == source);
        }

        //public async Task<string> UploadAvatarFileSystem(IFormFile file)
        //{
        //    if (file == null)
        //    {
        //        throw new Exception("No file selected");
        //    }

        //    var fileName = Path.GetFileName(HttpUtility.HtmlEncode(file.FileName));
        //    var filePath = "/documents";//Path.Combine(_webHostEnvironment.WebRootPath, "avatars");

        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);
        //    }
        //    var ext = Path.GetExtension(file.FileName).ToLower();
        //    if (!HasImageExtension(ext))
        //    {
        //        throw new Exception("Photo must be jpg, jpeg or png or gif");
        //    }

        //    if (file.Length < 2000001) //16 MB
        //    {
        //        fileName = Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString().Replace("-", string.Empty) + ext;
        //        var p = Path.Combine(filePath, fileName);
        //        using (var stream = new FileStream(p, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //            await stream.FlushAsync();
        //        }
        //        return p;
        //    }
        //    else
        //    {
        //        throw new Exception("Photo size must be less than 2 MB");
        //    }
        //}

        //public async Task<string> UploadAvatarAzure(IFormFile file)
        //{

        //    if (file == null)
        //    {
        //        throw new Exception("No file selected");
        //    }

        //    var fileName = Path.GetFileName(HttpUtility.HtmlEncode(file.FileName));

        //    var ext = Path.GetExtension(file.FileName).ToLower();
        //    if (!HasImageExtension(ext))
        //    {
        //        throw new Exception("Photo must be jpg, jpeg or png or gif");
        //    }

        //    if (file.Length < 2000001)
        //    {
        //        fileName = Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString().Replace("-", string.Empty) + ext;


        //        // intialize BobClient 
        //        BlobClient blobClient = new BlobClient(
        //            connectionString: _configuration.GetValue<string>("ConnectionStrings:AzureBlobConnectionString"),
        //            blobContainerName: "gbtcstorage",
        //            blobName: fileName);

        //        // upload the file
        //        //blobClient.Upload(filePath);

        //        var p = await blobClient.UploadAsync(file.OpenReadStream());


        //        return fileName;

        //        //var p = Path.Combine(filePath, fileName);
        //        //using (var stream = new FileStream(p, FileMode.Create))
        //        //{
        //        //    await file.File.CopyToAsync(stream);
        //        //    await stream.FlushAsync();
        //        //}
        //        //return p;
        //    }
        //    else
        //    {
        //        throw new Exception("Photo size must be less than 2 MB");
        //    }
        //}

        private async Task<string> UploadFileAws(IFormFile file)
        {
            if (file == null)
            {
                throw new Exception("No file selected");
            }

            var fileName = Path.GetFileName(HttpUtility.HtmlEncode(file.FileName));

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!HasImageExtension(ext))
            {
                throw new Exception("Фото должно быть в формате jpg, jpeg or png or gif");
            }
            if (file.Length < 4000001)
            {
                var awsAccessKeyId = _configuration.GetValue<string>("AWS:AwsAccessKeyId");
                var awsSecretAccessKey = _configuration.GetValue<string>("AWS:AwsSecretAccessKey");
                AmazonS3Config configsS3 = new()
                {
                    ServiceURL = _configuration.GetValue<string>("AWS:ServiceUrl")
                };

                using var client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, configsS3);
                using var newMemoryStream = new MemoryStream();
                file.CopyTo(newMemoryStream);
                fileName = Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString().Replace("-", string.Empty) + ext;
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = fileName,
                    BucketName = _configuration.GetValue<string>("AWS:BucketName"),
                    CannedACL = S3CannedACL.PublicRead
                };

                var fileTransferUtility = new TransferUtility(client);
                await fileTransferUtility.UploadAsync(uploadRequest);
                return fileName;
            }
            else
            {
                throw new Exception("Размер файла должен быть меньше 4 MB");
            }
        }
    }
}
