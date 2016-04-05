using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    [Validator(typeof(RegisterValidator))]
    public partial class RegisterModel : BaseNopModel
    {
        public RegisterModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.AvailableCities = new List<SelectListItem>();
            this.AvailableLocalities = new List<SelectListItem>();
            this.AvailableShippingCountries = new List<SelectListItem>();
            this.AvailableShippingStates = new List<SelectListItem>();
            this.AvailableShippingCities = new List<SelectListItem>();
            this.AvailableShippingLocalities = new List<SelectListItem>();
            this.CustomerAttributes = new List<CustomerAttributeModel>();
            this.AvailableChildren = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Account.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NopResourceDisplayName("Account.Fields.ConfirmPassword")]
        [AllowHtml]
        public string ConfirmPassword { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Account.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [NopResourceDisplayName("Account.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }
        public bool chkshippingaddress { get; set; }
        
        public bool DateOfBirthEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }

        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [NopResourceDisplayName("Account.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

      
        [NopResourceDisplayName("Account.Fields.StreetAddress")]
        [AllowHtml]
        public string ShippingStreetAddress { get; set; }

        
        [NopResourceDisplayName("Account.Fields.StreetAddress2")]
        [AllowHtml]
        public string ShippingStreetAddress2 { get; set; }

        
        [NopResourceDisplayName("Account.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ShippingZipPostalCode { get; set; }

        
        [NopResourceDisplayName("Account.Fields.City")]
        [AllowHtml]
        public string ShippingCity { get; set; }

        public int NoOfChildren { get; set; }

        public string Name1 { get; set; }

        [UIHint("DateTimeNullable")]
        public DateTime? BirthDate1 { get; set; }

        public string Name2 { get; set; }

        [UIHint("DateTimeNullable")]
        public DateTime? BirthDate2 { get; set; }


        public string Name3 { get; set; }

        [UIHint("DateTimeNullable")]
        public DateTime? BirthDate3 { get; set; }


        public string Name4 { get; set; }

        [UIHint("DateTimeNullable")]
        public DateTime? BirthDate4 { get; set; }

        public string Name5 { get; set; }

        [UIHint("DateTimeNullable")]
        public DateTime? BirthDate5 { get; set; }
        
        public IList<SelectListItem> AvailableChildren { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public IList<SelectListItem> AvailableShippingCountries { get; set; }

        public int ShippingCountryId { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.StateProvince")]
        public int StateProvinceId { get; set; }

        public int ShippingStateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public IList<SelectListItem> AvailableShippingStates { get; set; }

        public int CityId { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        public IList<SelectListItem> AvailableShippingCities { get; set; }
        public int ShippingCityId { get; set; }
        public int LocalityId { get; set; }
        public IList<SelectListItem> AvailableLocalities { get; set; }

        public IList<SelectListItem> AvailableShippingLocalities { get; set; }
        public int ShippingLocalityId { get; set; }
        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }
        
        public bool NewsletterEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Newsletter")]
        public bool Newsletter { get; set; }
        
        public bool AcceptPrivacyPolicyEnabled { get; set; }

        //time zone
        [NopResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowCustomersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [NopResourceDisplayName("Account.Fields.VatNumber")]
        public string VatNumber { get; set; }
        public bool DisplayVatNumber { get; set; }

        public bool HoneypotEnabled { get; set; }
        public bool DisplayCaptcha { get; set; }

        public IList<CustomerAttributeModel> CustomerAttributes { get; set; }
    }
}