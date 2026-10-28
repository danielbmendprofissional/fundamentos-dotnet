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
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(CalendarContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        {
            var affectedRows = await _context.SaveChangesAsync(ct);
            return affectedRows > 0;
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null) return;

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            try
            {
                await _context.SaveChangesAsync(ct);

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
