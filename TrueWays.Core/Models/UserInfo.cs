using System;

namespace TrueWays.Core.Models
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string LoginName { get; set; }

        public string PassWord { get; set; }

        public UserRole UserRole { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string SaltValue { get; set; }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }
    }
}
