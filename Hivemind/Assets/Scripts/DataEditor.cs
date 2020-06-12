public class DataEditor
{
    public MindType type;

    #region Gathering settings
    public int CarryWeight;
    public ResourceType PreferredResource;
    public Gathering.Direction PreferredDirection;
    public bool Scouting;
    #endregion

    #region Combat settings

    #endregion

    public IMind GenerateMind()
    {
        switch (type)
        {
            case MindType.Gathering:
                return new Gathering(PreferredResource, CarryWeight, PreferredDirection, Scouting);
            case MindType.Combat:
                return new CombatMind();
            default:
                return null;
        }
    }
}