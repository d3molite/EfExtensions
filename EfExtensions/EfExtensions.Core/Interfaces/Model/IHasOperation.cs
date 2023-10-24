using EfExtensions.Core.Enum;

namespace EfExtensions.Core.Interfaces.Model;

/// <summary>
/// Marks a db item as having operations.
/// </summary>
public interface IHasOperation
{
    /// <summary>
    /// Operation type which determines the operation performed by the repository.
    /// </summary>
    public Operation OperationType { get; set; }
}