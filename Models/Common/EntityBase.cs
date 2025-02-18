﻿using System;

namespace MenulioPocMvc.Models.Common
{
    public abstract class EntityBase<K> : IEntity<K> where K : IEquatable<K>
    {
        public virtual K Id { get; set; }
        public bool Active { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }
    }
}
