using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SignCustInfo
    {
        public string signid { get; set; }
        public byte[] photo { get; set; }
        public byte[] saltval { get; set; }
        public string enckeys { get; set; }
        public int custid { get; set; }
        public string custname { get; set; }
        public bool deleted { get; set; }
        public bool verified { get; set; }
        public DateTime lastactivity { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime expirydate { get; set; }
        public string createdby { get; set; }
        public DateTime createddate { get; set; }
        public string modifiedby { get; set; }
        public DateTime modifieddate { get; set; }
        public string verifiedby { get; set; }
        public DateTime? verifieddate { get; set; } // Nullable if not always set
        public bool isfromsign { get; set; }
        public string remarks { get; set; }
        public byte[] photothumb { get; set; }
    }


    public class SignOtherInfo
    {
        public string signid { get; set; }
        public int acctid { get; set; }
        public string accttype { get; set; }
        public int empid { get; set; }
        public string bankcode { get; set; }
        public string solid { get; set; }
        public string imageaccesscode { get; set; }
        public string signpowerno { get; set; }
        public string keyword { get; set; }
        public int entityid { get; set; }
    }


    public class SignMaintenance
    {
        public string signid { get; set; }
        public int signgrpid { get; set; }
        public string sign { get; set; }
        public byte[] saltval { get; set; }
        public string enckeys { get; set; }
        public DateTime effectivedate { get; set; }
        public DateTime expirydate { get; set; }
        public bool active { get; set; }
        public bool deleted { get; set; }
        public bool verified { get; set; }
        public DateTime lastactivity { get; set; }
        public string createdby { get; set; }
        public DateTime createddate { get; set; }
        public string modifiedby { get; set; }
        public DateTime modifieddate { get; set; }
        public string verifiedby { get; set; }
        public DateTime? verifieddate { get; set; }
        public string remarks { get; set; }
        public string alt1_remark { get; set; }
        public byte[] signtthumb { get; set; }
    }


}
