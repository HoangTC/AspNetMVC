using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project1.Models
{
    public class Permission
    {
        public Permission()
        {
            this.Users = new HashSet<User>();
            this.Roles = new HashSet<Role>();
        }

        public int Id { get; set; }
        public string CodeName { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}