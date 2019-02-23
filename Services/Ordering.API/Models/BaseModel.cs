using System.Runtime.Serialization;

namespace CasaDoCodigo.Ordering.Models
{
    [DataContract]
    public abstract class BaseModel
    {
        [DataMember]
        public int Id { get; set; }
    }
}
