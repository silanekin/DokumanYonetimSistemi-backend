using DokumanSistem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> GetByIdAsync(Guid id);
        Task<IEnumerable<Document>> GetAllAsync();
        Task AddAsync(Document document, string createdBy);
        Task UpdateAsync(Document document, string updatedBy);
        Task DeleteAsync(Guid id);
    }
}
