using ClercSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Infrastructure.Interfaces
{
    public interface IDocumentUserRepository
    {

        Task AddAndSaveAsync(DocumentUser documentUser);
    }
}
