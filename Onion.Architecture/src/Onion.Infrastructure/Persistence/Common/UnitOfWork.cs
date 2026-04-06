using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Onion.Application.Common.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Common
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CalendarContext _context;
        private readonly CalendarUserContext _userContext;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(CalendarContext context, CalendarUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null) return;

            _currentTransaction = await _context.Database.BeginTransactionAsync();

            await _userContext.Database.UseTransactionAsync(_currentTransaction.GetDbTransaction());
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
                await _userContext.SaveChangesAsync(ct);

                if (_currentTransaction != null)
                    await _currentTransaction.CommitAsync(ct);
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public async Task RollbackAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                DisposeTransaction();
            }
        }

        private void DisposeTransaction()
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }
}
