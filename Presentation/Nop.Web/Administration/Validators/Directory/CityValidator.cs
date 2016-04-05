using FluentValidation;
using Nop.Admin.Models.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Directory
{
    public class CityValidator : BaseNopValidator<CityModel>
    {
        public CityValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.StateProvinces.Cities.Fields.Name.Required"));
        }
    }
}