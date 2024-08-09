using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Entities
{
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public DateTime InsertDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
