using Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category : BaseModel
    { 
        public Category() { }

        public Category(string name)
        {
            Name = name;
        }

        [Required]
        public string Name { get; private set; }

        public override bool Equals(object obj)
        {
            var category = obj as Category;
            return category != null &&
                   Id == category.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -1919740922;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
