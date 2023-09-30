using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Portfolios;

public record SeedDataCommand() : IRequest;

public class SeedDataCommandHandler : IRequestHandler<SeedDataCommand>
{
    private readonly ILogger<SeedDataCommandHandler> _logger;
    private readonly ApplicationDbContext _context;

    public SeedDataCommandHandler(ILogger<SeedDataCommandHandler> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(SeedDataCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Portfolios.AnyAsync())
        {
            var seed = new[]
            {
                new Portfolio("BitCoin","BTC",17.2m),
                new Portfolio("Ethereum","ETH",11.5m),
                new Portfolio("Solana","SOL",8.6m),
                new Portfolio("Cardano","ADA",7.4m),
                new Portfolio("Tron","TRON",3.8m),
                new Portfolio("LiteCoin","LTC",3.3m),
                new Portfolio("Cosmos","ATOM",2.9m),
                new Portfolio("Monero","XMR",2.5m)
            };
            _context.Portfolios.AddRange(seed);
            await _context.SaveChangesAsync();
        }

        //размещу временно здесь
        if (!await _context.PlatformSettings.AnyAsync())
        {
            var ps = new PlatformSettings(30, 20, 5000, 1000, 6, 5, 10, 10,1000);
            _context.PlatformSettings.Add(ps);
            await _context.SaveChangesAsync();
        }
    }
}
