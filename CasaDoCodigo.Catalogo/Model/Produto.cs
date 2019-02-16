﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Catalog.API.Model
{
    public class Produto : BaseModel
    {
        public Produto()
        {

        }

        [Required]
        public Categoria Categoria { get; private set; }
        [Required]
        [DataMember]
        public string Codigo { get; private set; }
        [Required]
        [DataMember]
        public string Nome { get; private set; }
        [Required]
        public decimal Preco { get; private set; }

        public Produto(string codigo, string nome, decimal preco, Categoria categoria)
        {
            this.Codigo = codigo;
            this.Nome = nome;
            this.Preco = preco;
            this.Categoria = categoria;
        }
    }
}
