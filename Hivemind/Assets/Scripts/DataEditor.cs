using System;
using UnityEngine;

[Serializable]
public class DataEditor
{
    public MindType type;

    #region Gathering settings
    [Range(0, 5)]
    public int CarryWeight = 1;
    public ResourceType PreferredResource;
    public Gathering.Direction PreferredDirection;
    public bool Scouting;
    #endregion

    #region Combat settings
    public int VisionRadius;
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