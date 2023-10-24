using System.ComponentModel.DataAnnotations;
using EfExtensions.Core.Interfaces.Model;

namespace EfExtensions.Items.Model;

/// <summary>
/// Extends the base db item with a concurrency RowVersion timestamp.
/// </summary>
public class ConcurrentDbItem<TKey> : DbItem<TKey>, IHasConcurrencyToken
{
    /// <inheritdoc/>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}