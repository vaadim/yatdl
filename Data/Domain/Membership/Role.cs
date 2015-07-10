using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YATDL
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public virtual Guid Id { get; set; }

        [Required]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        private ICollection<User> _users;
        public virtual ICollection<User> Users
        {
            get { return _users ?? (_users = new HashSet<User>()); }
            set { _users = value; }
        }

    }
}