using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokumanSistem.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool IsGroup { get; set; }

        public string? IndexData { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public ICollection<UserCategoryLink> UserCategoryLinks { get; set; } = new List<UserCategoryLink>();
    }

}
