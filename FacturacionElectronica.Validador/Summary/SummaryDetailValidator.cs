using FacturacionElectronica.GeneradorXml.Entity.Details;
using FluentValidation;

namespace FacturacionElectronica.Validador.Summary
{
    /// <summary>
    /// Class SummaryDetailValidator.
    /// </summary>
    public class SummaryDetailValidator : AbstractValidator<SummaryDetail>
    {
        /// <inheritdoc />
        public SummaryDetailValidator()
        {
            RuleFor(det => det.SerieDocumento)
                .NotEmpty()
                .Matches("^[B][A-Z0-9]{3}$")
                .WithMessage("El documento no cumple con el estandar");

            //RuleFor(det => det.Documento)
            //    .NotEmpty()
            //    .Matches("^[B][A-Z0-9]{3}-[0-9]{1,8}$")
            //    .WithMessage("El documento no cumple con el estandar");
        }
    }
}
