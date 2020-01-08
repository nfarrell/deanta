using StatusGeneric;

namespace Deanta.Cosmos.ServiceLayer.TodosSqlWithEvents
{
    public interface IHardResetCacheService
    {
        IStatusGeneric<string> CheckUpdateTodoCacheProperties();
    }
}