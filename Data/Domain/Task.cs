using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YATDL
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool? Done { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public System.DateTime Created { get; set; }

        public ImportanceLevel Importance { get; set; }
        public string Description { get; set; }


        // foreign key
        public System.Guid UserId { get; set; }
        public User User { get; set; }
    }
}