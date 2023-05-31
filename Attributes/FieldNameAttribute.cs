namespace PunchoutUtils.Attributes
{
    PUBLIC class FieldNameAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldNameAttribute(string name) => Name = name;
    }
}
