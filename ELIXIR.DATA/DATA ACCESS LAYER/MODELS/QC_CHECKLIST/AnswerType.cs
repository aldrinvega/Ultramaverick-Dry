using FluentValidation;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public enum AnswerType
    {
        YerOrNo,
        OpenField,
        Dimensions
    }

    public class AnswerTypeValidator : AbstractValidator<AnswerType>
    {
        public AnswerTypeValidator()
        {
            RuleFor(answerType => answerType)
                .IsInEnum()
                .WithMessage("Invalid Answer Type. Please provide a valid Answer Type.");

            RuleFor(answerType => answerType)
                .Null()
                .WithMessage("Answer type is required");
        }
    }
}