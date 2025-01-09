using System;

namespace MenulioPocMvc.Models.Common
{
    public interface IKey<K>
    {
        K ToKey();
    }

    public interface IVersionable
    {
        Guid VersionId { get; set; }
    }

    public interface IEntity<K> where K : IEquatable<K>
    {
        K Id { get; set; }
        DateTime DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}
