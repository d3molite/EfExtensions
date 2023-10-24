namespace EfExtensions.Core.Interfaces.Model;

/// <summary>
/// Extends a db item with a concurrency RowVersion timestamp.
/// </summary>
public interface IHasConcurrencyToken
{
    /// <summary>
    /// RowVersion Column
    /// </summary>
    public byte[]? RowVersion { get; set; }
}