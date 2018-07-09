using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project1.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [DisplayName("Tên đăng nhập")]
        [RegularExpression(@"^[a-zA-Z0-9_]{5,255}$", ErrorMessage = "Không được chứa ký tự đặt biệt")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [DisplayName("Mật khẩu")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [RegularExpression(@"^(?=.*[\d])(?=.*[a-z])[\w\d!@#$%_]{6,40}$", ErrorMessage = "Mật khâu không đúng định dạng")]
        public string Password { get; set; }
        [NotMapped]
        [DisplayName("Nhập lại mật khẩu")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [RegularExpression(@"^(?=.*[\d])(?=.*[a-z])[\w\d!@#$%_]{6,40}$", ErrorMessage = "Mật khâu không đúng định dạng")]
        public string RPassword { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Lỗi định dạng email")]
        public string Email { get; set; }
        public string SecurityPassword { get; set; }
        public string ActivationCode { get; set; }
        public DateTime TimeGetCode { get; set; }
        public int CountLogin { get; set; }
        public DateTime TimeCountLogin { get; set; }
        public bool ConfirmActivity { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

        public User()
        {
            this.Roles = new HashSet<Role>();
            this.Permissions = new HashSet<Permission>();
            this.Email = "default@gmail.com";
            this.TimeGetCode = DateTime.UtcNow;
            this.TimeCountLogin = DateTime.UtcNow;
            this.ActivationCode = "";
            this.CountLogin = 0;
            this.RPassword = "";
            this.ConfirmActivity = false;
        }
    }
}