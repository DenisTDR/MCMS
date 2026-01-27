using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MCMS.Base.Data;

/// <summary>
/// Marker interface for EF Core interceptors that should be auto-registered with DbContext.
/// Interceptors implementing this interface will be discovered and added to DbContext options
/// during startup.
/// </summary>
public interface IMcmsDbInterceptor : IInterceptor
{
}
