using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public class MembershipCategoryValidator : BaseNopValidator<MembershipCategoryModel>
    {
        public MembershipCategoryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
        }
    }
}