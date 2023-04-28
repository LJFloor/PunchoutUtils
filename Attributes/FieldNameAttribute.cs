﻿namespace PunchoutUtils.Attributes
{
    internal class FieldNameAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldNameAttribute(string name) => Name = name;
    }
}