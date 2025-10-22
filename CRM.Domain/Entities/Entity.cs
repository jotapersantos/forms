using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Domain.Entities;

public abstract class Entity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id)
    {
        Id = id;
    }

    [Key]
    public Guid Id { get; protected set; }
}
