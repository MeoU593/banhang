using FluentValidation;
using Nop.Plugin.Misc.DocumentPortal.Models.Admin;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.DocumentPortal.Validators;

public class DocumentTypeManageModelValidator : BaseNopValidator<DocumentTypeManageModel>
{
    public DocumentTypeManageModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên loại tài liệu là bắt buộc.")
            .MaximumLength(250);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
