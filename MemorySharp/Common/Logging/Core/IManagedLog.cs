using Binarysharp.MemoryManagement.Managment.Builders;

namespace Binarysharp.MemoryManagement.Common.Logging.Core
{
    /// <summary>
    ///     Interface IManagedLog defines a log which is part of a pool of other <see cref="ILog" /> members.
    /// </summary>
    public interface IManagedLog : ILog, INamedElement
    {
    }
}