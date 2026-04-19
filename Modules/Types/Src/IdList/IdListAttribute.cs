using System;

public class IdListAttribute : Attribute
{
    public string ListName { get; }
    
    public IdListAttribute(string listName)
    {
        ListName = listName;
    }
}