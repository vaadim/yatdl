using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YATDL
{
    [Table("Users")]
    public class User
    {
        [Key, Index]
        public Guid Id { get; set; }

        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public virtual string Password { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Comment { get; set; }

        public virtual bool IsApproved { get; set; }
        public virtual bool IsConfirmed { get; set; }
        public virtual int PasswordFailuresSinceLastSuccess { get; set; }
        public virtual DateTime? LastPasswordFailureDate { get; set; }
        public virtual DateTime? LastActivityDate { get; set; }
        public virtual DateTime? LastLockoutDate { get; set; }
        public virtual DateTime? LastLoginDate { get; set; }
        public virtual string ConfirmationToken { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime? LastPasswordChangedDate { get; set; }
        public virtual string PasswordVerificationToken { get; set; }
        public virtual DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        
        [Required]
        public UserProfile UserProfile { get; set; }


        public ICollection<Task> Tasks { get; set; }
    }
}