using FluentValidation;
using Nop.Admin.Models.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Directory
{
    public class LocalityValidator : BaseNopValidator<LocalityModel>
    {
        public LocalityValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Cities.Localities.Fields.Name.Required"));
        }
    }
}