using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.STS.Identity.ViewModels.Manage
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [MaxLength(255)]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [MaxLength(255)]
        [Display(Name = "Profile")]
        public string Profile { get; set; }

        [MaxLength(255)]
        [Display(Name = "Region")]
        public string Region { get; set; }

        [MaxLength(255)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [MaxLength(255)]
        [Display(Name = "City")]
        public string Locality { get; set; }

        [MaxLength(255)]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [MaxLength(255)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string StatusMessage { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [MaxLength(100)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        //[Display(Name = "Gender")]
        //public EGenders Gender { get; set; }


        public string UserType { get; set; }
        
        public bool SignNotifyActive { get; set; }
        
        public bool SignPushNotifyActive { get; set; }
        
        [MaxLength(3)]
        public string RegionAddrRef { get; set; }
        
        public bool UserFromEsia { get; set; }
        
        public bool SignAutoLocationActive { get; set; }
        
        public string AddressFias { get; set; }
        
        [MaxLength(12)]
        public string LoginNameIp { get; set; }
        
        [MaxLength(10)]
        public string LoginNameUl { get; set; }
        
        public string NameOrg { get; set; }
        
        [MaxLength(15)]
        [Display(Name = "ogrn_ip")]
        public string OgrnIp { get; set; }
        
        [MaxLength(13)]
        public string OgrnUl { get; set; }
        
        [MaxLength(5)]
        public string Opf { get; set; }

        public bool ShowSvetAttributes { get; set; }

        public bool ShowExtendedAttributes { get; set; }

        public string BankName { get; set; }
        public string Account { get; set; }
        public string NoCorAccount { get; set; }
        public string LoginNameFL { get; set; }
        public string StreetaddrReg { get; set; }
        public string HeadPostOther { get; set; }
        public bool SignNotifyTP2 { get; set; }
        public bool? Sbc1Value { get; set; } = null;
        public bool? Sbc2Value { get; set; } = null;
        public bool? Sbc3Value { get; set; } = null;
        public bool? Sbc4Value { get; set; } = null;
        public string Sbc5Value { get; set; } = null;
        public int? DULnum { get; set; }
        public string RegionaddrFactRef { get; set; }
        public string SetFromPortal { get; set; }
        public string Corpaddr { get; set; }
        public string BoroughaddrFact { get; set; } = null;
        public string RepresentDoc1 { get; set; } = null;
        public string ShdShowSvetAttributes { get; set; } = null;
        public string SignAutoLocation { get; set; } = null;
        public string Code { get; set; } = null;
        public bool BlockedId { get; set; }
        public string IndexaddrReg { get; set; } = null;
        public string AreaaddrReg { get; set; } = null;
        public string LoginNameUL { get; set; } = null;
        public string TstBtn { get; set; } = null;
        public int? FactAddrMatch { get; set; } = null;
        public int? HouseaddrReg { get; set; } = null;
        public int? PostAddrMatch { get; set; } = null;
        public string FromMlk { get; set; } = null;
        public string StreetAddr { get; set; } = null;
        public string Description { get; set; } = null;
        public string SignLoginConfirm { get; set; } = null;
        public int? HouseAddrFact { get; set; }
        public string Settlement { get; set; } = null;
        public string DULorgan { get; set; } = null;
        public int? ApartmentAddr { get; set; }
        public string ContactPhone { get; set; } = null;
        public string CBuildingAddrFact { get; set; } = null;
        public string CorpAddrFact { get; set; } = null;
        public string SaveProfileButton { get; set; } = null;
        public string WarningTasks { get; set; } = null;
        public string SignNotify { get; set; } = null;
        public string SignExpert { get; set; } = null;
        public string Avatar { get; set; } = null;
        public string PostAddrType { get; set; } = null;
        public string RegionaddrRef { get; set; } = null;
        public string RegionaddrReg { get; set; } = null;
        public string DULTypeOther { get; set; } = null;
        public string RepresentDocType { get; set; } = null;
        public string DULSer { get; set; } = null;
        public string RepresentDocNum { get; set; } = null;
        public string DULTypeRef { get; set; } = null;
        public string WorkSchedule { get; set; } = null;
    }
}
