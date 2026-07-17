public interface IEntity
{
    int Id { get; }
    CtxFlag AvailableTags { get; }
    EEntityType EntType { get; }
    /// <summary>
    /// Can release flag enum for variants updated
    /// </summary>
    bool HaveChange { get; set; }
}