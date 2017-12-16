using FacturacionElectronica.GeneradorXml.Entity.Details;
using FluentValidation;

namespace FacturacionElectronica.Validador.Voided
{
    /// <summary>
    /// Class VoidedDetailValidator.
    /// </summary>
    public class VoidedDetailValidator : AbstractValidator<VoidedDetail>
    {
        /// <inheritdoc />
        public VoidedDetailValidator()
        {
            RuleFor(det => det.SerieDocumento)
                .NotEmpty()
                .Matches("^[FBRP][A-Z0-9]{3}$")
                .WithMessage("La serie no cumple con el formato");

            RuleFor(det => det.CorrelativoDocumento)
                .NotEmpty()
                .MaximumLength(8)
                .WithMessage("El correlativo del documento debe tener un maximo de 8 digitos");

            RuleFor(det => det.Motivo)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("El motivo debe tener un maximo de 100 caracteres");
        }
    }
}
