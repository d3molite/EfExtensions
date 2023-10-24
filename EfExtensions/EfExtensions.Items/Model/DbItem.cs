using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfExtensions.Core.Enum;
using EfExtensions.Core.Interfaces.Model;

namespace EfExtensions.Items.Model;

/// <inheritdoc/>
public class DbItem<TKey> : IDbItem<TKey>
{
    /// <inheritdoc/>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column(Order = 0)]
    public TKey Id { get; set; }
    
    /// <inheritdoc/>
    [NotMapped]
    public Operation OperationType { get; set; }
}