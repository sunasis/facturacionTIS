using FacturacionElectronica.GeneradorXml.Entity.Details;
using FluentValidation;

namespace FacturacionElectronica.Validador.Sale
{
    /// <summary>
    /// Class InvoiceDetailValidator.
    /// </summary>
    public class InvoiceDetailValidator : AbstractValidator<InvoiceDetail>
    {
        /// <inheritdoc />
        public InvoiceDetailValidator()
        {
            RuleFor(det => det.CodigoProducto)
                .MaximumLength(30)
                .WithMessage("Codigo de producto debe tener un maximo de 30 caracteres");
            RuleFor(det => det.UnidadMedida)
                .NotEmpty()
                .WithMessage("La unidad de medida es requerido");
            RuleFor(det => det.DescripcionProducto)
                .NotEmpty()
                .MaximumLength(250)
                .WithMessage("La descripcion es requerido hasta 250 caracteres");
        }
    }
}
