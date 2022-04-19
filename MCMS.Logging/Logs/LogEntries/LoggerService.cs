using System.Threading.Tasks;
using MCMS.Base.Data;

namespace MCMS.Logging.Logs.LogEntries
{
    public class LoggerService
    {
        private readonly IRepository<LogEntryEntity> _entriesRepo;

        public LoggerService(IRepository<LogEntryEntity> entriesRepo)
        {
            _entriesRepo = entriesRepo;
        }

        public async Task AddLog(string type, string title, string data, string rawData, string context)
        {
            await _entriesRepo.Add(new LogEntryEntity
            {
                Type = type, Title = title, Data = data, Context = context, RawData = rawData
            });
        }
    }
}