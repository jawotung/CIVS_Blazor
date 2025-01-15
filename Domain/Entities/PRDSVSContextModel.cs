using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CustInfo
    {
        [Key]
        [Display(Name = "Account Number")]
        public string ACCNo { get; set; }
        [Display(Name = "Number of Image")]
        public string NumImage { get; set; }
        public DateTime DateLstUpdated { get; set; }

    }

    public class SvsGroup
    {
        [Key]
        public string ACCNo { get; set; }
        public string GroupName { get; set; }
        public Int16 ImageNo { get; set; }
    }

    public class SvsRule
    {
        [Key]
        public string ACCNo { get; set; }
        public Int16 RuleNo { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateTime ExpiredDate { get; set; }

    }

    public class Image
    {
        [Key]
        public string ACCNo { get; set; }
        public Int16 ImageNo { get; set; }
        public byte[] Signature { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string IDCard { get; set; }
        public string Phone { get; set; }
        public DateTime ApproveDate { get; set; }
    }

    public class ImageView
    {
        public string AccNo { get; set; }
        public string Signature { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string IDCard { get; set; }
        public string Phone { get; set; }
        public int ImageNo { get; set; }
    }
}
