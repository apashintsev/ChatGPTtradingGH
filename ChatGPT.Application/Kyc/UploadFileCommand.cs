using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using ChatGPTtrading.Infrastructure.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Kyc;

public record UploadFileCommand(IFormFile File) : IRequest<Guid>;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<UploadFileCommandHandler> _logger;

    public UploadFileCommandHandler(ILogger<UploadFileCommandHandler> logger, IFileUploadService fileUploadService, ApplicationDbContext context)
    {
        _logger = logger;
        _fileUploadService = fileUploadService;
        _context = context;
    }

    public async Task<Guid> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {

        var filePath = await _fileUploadService.UploadFileSystem(request.File);
        var fileItem = new FileItem(filePath);

        _context.Files.Add(fileItem);
        await _context.SaveChangesAsync();
        return fileItem.Id;
    }
}
