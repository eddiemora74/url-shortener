using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace url_shortener.core.Primitives;

public abstract class Entity
{
    [Key]
    [Column("id")]
    public required Guid Id { get; set; }
}