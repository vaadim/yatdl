using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YATDL
{
    [Table("UserProfiles")]
    public class UserProfile
    {
        [Key]
        [ForeignKey("ProfileOf")]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }

        public User ProfileOf { get; set; }
    }
}