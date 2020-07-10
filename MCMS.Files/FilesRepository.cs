using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Exceptions;
using MCMS.Data;
using MCMS.Files.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MCMS.Files
{
    public class FilesRepository : Repository<FileEntity>
    {
        private readonly ILogger<FilesRepository> _logger;

        public FilesRepository(BaseDbContext dbContext, ILogger<FilesRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public override async Task<bool> Delete(FileEntity e)
        {
            e = await DeleteFile(e);
            return await base.Delete(e);
        }

        public override async Task<bool> Delete(string id)
        {
            var e = new FileEntity {Id = id};
            e = await DeleteFile(e);
            return await base.Delete(e);
        }

        public override async Task<bool> Delete(Expression<Func<FileEntity, bool>> predicate)
        {
            var files = await _dbSet.Where(predicate).ToListAsync();
            foreach (var fileEntity in files)
            {
                await DeleteFile(fileEntity);
            }

            _dbSet.RemoveRange(files);
            await SaveChangesAsyncIfNeeded();
            return true;
        }

        private async Task<FileEntity> DeleteFile(FileEntity e)
        {
            if (e.PhysicalFullPath == null)
            {
                if (!DbContext.Entry(e).IsKeySet)
                {
                    throw new KnownException("Can't delete a file, not enough intel provided.");
                }

                e = await GetOne(e.Id);
            }

            if (e.PhysicalFullPath == null)
            {
                throw new KnownException("Can't delete a file, not enough intel provided.");
            }

            if (File.Exists(e.PhysicalFullPath))
            {
                _logger.LogInformation("Deleting file: " + e.PhysicalFullPath);
                File.Delete(e.PhysicalFullPath);
            }

            return e;
        }
    }
}