using UnityEngine;

public class NameAttribute : PropertyAttribute
{
    public string Name { get; set; }

    public NameAttribute(string name)
    {
        this.Name = name;
    }
}
