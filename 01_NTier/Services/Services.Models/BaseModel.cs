using System.Runtime.Serialization;

namespace Services.Models
{
    [DataContract]
    public abstract class BaseModel
    {
        [DataMember]
        public int Id { get; set; }
    }
}
