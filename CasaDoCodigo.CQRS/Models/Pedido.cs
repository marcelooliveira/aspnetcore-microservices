using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Models
{
    public class Pedido : BaseModel
    {
        public Pedido()
        {
            Registration = new Registration();
        }

        public Pedido(Registration registration)
        {
            Registration = registration;
        }

        public List<ItemPedido> Items { get; private set; } = new List<ItemPedido>();
        [Required]
        public virtual Registration Registration { get; private set; }
    }
}
