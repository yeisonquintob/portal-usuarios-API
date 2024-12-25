using System;

namespace UserPortal.Data.Exceptions;

/// <summary>
/// Excepción para cuando no se encuentra una entidad
/// </summary>
public class EntityNotFoundException : DatabaseException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public EntityNotFoundException(string entityName, object entityId) 
        : base($"Entity '{entityName}' with id '{entityId}' was not found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}