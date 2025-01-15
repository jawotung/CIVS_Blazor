using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UserClaims
    {
        public string UserID { get; set; }
        public string DisplayName { get; set; }
        public string UserType { get; set; }
        public string BranchCode { get; set; }
        public string Branch { get; set; }
        public string Group { get; set; }
        public string Menu { get; set; }
        public string LastLoginDate { get; set; }
        public string BuddyCode { get; set; }
        public string BuddyBranch { get; set; }
    }
}
