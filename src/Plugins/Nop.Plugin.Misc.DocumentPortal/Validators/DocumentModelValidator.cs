using FluentValidation;
using Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.DocumentPortal.Validators;

public class DocumentModelValidator : BaseNopValidator<DocumentModel>
{
    public DocumentModelValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.DocumentPortal.Document.Fields.Title.Required"))
            .MaximumLength(500);

        RuleFor(x => x.Slug)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Slug));

        RuleFor(x => x.Code)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.Summary)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Summary));

        RuleFor(x => x.Keywords)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Keywords));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
