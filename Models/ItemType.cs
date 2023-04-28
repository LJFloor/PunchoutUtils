using System.Runtime.Serialization;

namespace PunchoutUtils.Models
{
    public enum ItemType
    {
        [EnumMember(Value = "R")]
        Root,

        [EnumMember(Value = "O")]
        Outline,

        [EnumMember(Value = "L")]
        Leaf,
    }
}
