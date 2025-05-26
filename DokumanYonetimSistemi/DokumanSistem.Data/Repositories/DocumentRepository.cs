using DokumanSistem.Core.Models;
using DokumanSistem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DokumanSistem.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DocumentDbContext _context;

        public DocumentRepository(DocumentDbContext context)
        {
            _context = context;
        }

        public async Task<Document> GetByIdAsync(Guid id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task AddAsync(Document document, string createdBy)
        {
            document.CreatedBy = createdBy;
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Document document, string updatedBy)
        {
            document.UpdatedBy = updatedBy;
            document.UpdatedDate = DateTime.UtcNow;
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var document = await GetByIdAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
        }
    }
}

