using System;
using UnityEngine;

public class RequiredAttribute : PropertyAttribute
{
    public Type RequiredType { get; set; }

    public RequiredAttribute(Type type)
    {
        this.RequiredType = type;
    }
}
