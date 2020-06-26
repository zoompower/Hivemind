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

    #endregion Gathering settings

    #region Combat settings

    public int VisionRadius;
    public bool AttackingQueen;
    #endregion

    public IMind GenerateMind()
    {
        switch (type)
        {
            case MindType.Gathering:
                return new Gathering(PreferredResource, CarryWeight, PreferredDirection, Scouting);

            case MindType.Combat:
                return new CombatMind(VisionRadius, AttackingQueen);
                
            default:
                return null;
        }
    }

    public static DataEditor GetGatheringEditor()
    {
        return new DataEditor() { type = MindType.Gathering };
    }

    public static DataEditor GetGatheringEditor(ResourceType PreferredResource, int CarryWeight, Gathering.Direction PreferredDirection, bool Scouting)
    {
        return new DataEditor() { type = MindType.Gathering, PreferredResource = PreferredResource, CarryWeight = CarryWeight, PreferredDirection = PreferredDirection, Scouting = Scouting };
    }

    public static DataEditor GetCombatEditor()
    {
        return new DataEditor() { type = MindType.Combat };
    }

    public static DataEditor GetCombatEditor(int VisionRadius, bool AttackingQueen)
    {
        return new DataEditor() { type = MindType.Combat, VisionRadius = VisionRadius, AttackingQueen = AttackingQueen };
    }
}
