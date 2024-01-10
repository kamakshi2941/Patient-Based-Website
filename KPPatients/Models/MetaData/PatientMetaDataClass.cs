using KPClassLibrary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace KPPatients.Models
{
    [ModelMetadataType(typeof(PatientMetaDataClass))]
    public partial class Patient : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            PatientsContext patientsContext = new PatientsContext();
            FirstName = KPValidations.KPCapitalize(FirstName);
            LastName = KPValidations.KPCapitalize(LastName);
            City = KPValidations.KPCapitalize(City);
            Address = KPValidations.KPCapitalize(Address);
            Gender = KPValidations.KPCapitalize(Gender);

            string countrycode = "CA";
            if (ProvinceCode != null && ProvinceCode != "")
            {
                var ProvinceDetail = patientsContext.Provinces.Where(x => x.ProvinceCode == ProvinceCode).FirstOrDefault();

                if (ProvinceDetail == null)
                {
                    yield return new ValidationResult("Province is not found",new[] { nameof(ProvinceCode) });
                }
                else
                {
                    countrycode = ProvinceDetail.CountryCode;
                }
            }
            if (PostalCode != null && PostalCode != "")
            {
                if (countrycode == "CA")
                {
                    if (ProvinceCode == null || ProvinceCode == "")
                    {
                        yield return new ValidationResult("provinceCode is invalid/missing ... it’s required to edit a postal/zip code",
                          new[] { nameof(PostalCode) });
                    }
                    else
                    {
                        if (!KPValidations.KPPostalCodeValidation(PostalCode))
                        {
                            yield return new ValidationResult("Enter valid postal code",
                          new[] { nameof(PostalCode) });
                        }
                    }
                }
                //if (countrycode == "US")
                //{
                //    if (!KPValidations.KPPostalCodeValidation(PostalCode))
                //    {
                //        yield return new ValidationResult("Zip code should be five or nine character",
                //         new[] { nameof(PostalCode) });

                //    }
                //}
            }

            if (Ohip != null && Ohip != "")
            {
                Ohip = Ohip.ToUpper();
                Regex pattern = new Regex(@"^([0-9]{4})-([0-9]{3})-([0-9]{3})-([A-Z]{2})$", RegexOptions.IgnoreCase);
                if (pattern.IsMatch(Ohip.ToString()))
                {
                    yield return ValidationResult.Success;
                }
                else
                {
                    yield return new ValidationResult(" OHIP must be in such pattern : 1234-123-123-XX",
                         new[] { nameof(Ohip) });
                }
            }

            if (HomePhone != null && HomePhone != "")
            {
                HomePhone = KPValidations.KPExtractDigits(HomePhone);
                if (HomePhone.Length == 10)
                {
                    HomePhone = HomePhone.Substring(0, 3) + "-" + HomePhone.Substring(3, 3) + "-" + HomePhone.Substring(6, 4);
                }
                else
                {
                    yield return new ValidationResult(" Phone number should be 10 digits",
                         new[] { nameof(HomePhone) });
                }
            }
            DateTime today = DateTime.Now;
            if (DateOfBirth != null)
            {

                if (DateOfBirth.Value > today)
                {
                    yield return new ValidationResult(" Date of Birth should not be of future",
                         new[] { nameof(DateOfBirth) });

                }

            }
            if (Deceased)
            {
                if (DateOfDeath == null)
                {
                    yield return new ValidationResult(" Date of Death required",
                         new[] { nameof(DateOfDeath) });
                }

                else
                {
                    if (DateOfDeath > today)
                    {
                        yield return new ValidationResult(" Date of Death can not be of future",
                         new[] { nameof(DateOfDeath) });
                    }

                }
            }
            else
            {
                if (DateOfDeath != null)
                {
                    yield return new ValidationResult(" Date of Death must be null",
                         new[] { nameof(DateOfDeath) });
                }
            }
            if (Gender != "M" && Gender != "X" && Gender != "F")
            {
                yield return new ValidationResult("Gender must be either 'M', 'X' or 'F'",
                         new[] { nameof(Gender) }); ;
            }
            yield return ValidationResult.Success;
        }
    }
    public class PatientMetaDataClass
    {
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Street Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "OHIP")]
        public string Ohip { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
        public bool Deceased { get; set; }

        [Display(Name = "Date of Death")]
        public DateTime? DateOfDeath { get; set; }

        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }

        [Required]
        public string Gender { get; set; }
    }

}

