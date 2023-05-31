namespace PunchoutUtils.Attributes
{
    public class FieldNameAttribute : Attribute
    {
        public string Name { get; set; }

        public FieldNameAttribute(string name) => Name = name;
    }
}
