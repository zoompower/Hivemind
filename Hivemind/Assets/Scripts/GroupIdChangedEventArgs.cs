using System;

public class GroupIdChangedEventArgs : EventArgs
{
    public Guid oldGuid;
    public Guid newGuid;

    public GroupIdChangedEventArgs(Guid oldGuid, Guid newGuid)
    {
        this.oldGuid = oldGuid;
        this.newGuid = newGuid;
    }
}

