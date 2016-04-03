namespace Reference.Domain.Map
{
    public interface IEntity
    {
        Location Location { get; set; }
        int GetPossiblePoints();
    }
}
