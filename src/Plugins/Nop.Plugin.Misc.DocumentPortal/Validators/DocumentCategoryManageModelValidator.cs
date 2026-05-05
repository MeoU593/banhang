using FluentValidation;
using Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.DocumentPortal.Validators;

public class DocumentCategoryManageModelValidator : BaseNopValidator<DocumentCategoryManageModel>
{
    public DocumentCategoryManageModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên danh mục là bắt buộc.")
            .MaximumLength(250);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
