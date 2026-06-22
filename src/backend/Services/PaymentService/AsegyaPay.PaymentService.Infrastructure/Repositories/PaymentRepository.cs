using AsegyaPay.PaymentService.Domain.Entities;
using AsegyaPay.PaymentService.Application.Interfaces;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using AsegyaPay.PaymentService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AsegyaPay.PaymentService.Infrastructure.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Payments
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Payment?> GetByOrderIdAsync(
        string orderId,
        string merchantId,
        CancellationToken cancellationToken = default) =>
        await _context.Payments
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.OrderId == orderId && p.MerchantId == merchantId, cancellationToken);

    public async Task<IEnumerable<Payment>> GetByMerchantIdAsync(
        string merchantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        await _context.Payments
            .Where(p => p.MerchantId == merchantId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default) =>
        await _context.Payments
            .CountAsync(p => p.MerchantId == merchantId, cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
