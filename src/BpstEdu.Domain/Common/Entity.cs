namespace BpstEdu.Domain.Common;

/// <summary>Base for every table. Ids are time-ordered GUIDs (v7): safe to show in URLs and index-friendly.</summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}
