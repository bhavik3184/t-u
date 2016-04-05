using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public class PlanValidator : BaseNopValidator<PlanModel>
    {
        public PlanValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Plans.Fields.Name.Required"));
        }
    }
}