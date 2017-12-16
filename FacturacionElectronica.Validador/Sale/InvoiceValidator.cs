using System;
using FacturacionElectronica.GeneradorXml.Entity;
using FluentValidation;

namespace FacturacionElectronica.Validador.Sale
{
    /// <summary>
    /// Class InvoiceValidator.
    /// </summary>
    public class InvoiceValidator : AbstractValidator<InvoiceHeader>
    {
        /// <inheritdoc />
        public InvoiceValidator()
        {
            RuleFor(inv => inv.FechaEmision)
                .LessThanOrEqualTo(DateTime.Now.AddDays(2))
                .WithMessage("La fecha de emision es mayor a dos días de la fecha de envío del comprobante");
            RuleFor(inv => inv.CodigoMoneda)
                .NotEmpty()
                .WithMessage("No existe informacion de la moneda");
            RuleFor(voided => voided.RucEmisor)
                .NotEmpty()
                .WithMessage("No existe ruc del emisor");
            RuleFor(voided => voided.NombreRazonSocialEmisor)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("La razon social del emisor debe tener de 1-100 caracteres");
            RuleFor(inv => inv.SerieDocumento)
                .NotEmpty()
                .Matches("^[FB][A-Z0-9]{3}$")
                .WithMessage("La serie no empieza con F(Factura) o B(Boleta)");
            RuleFor(inv => inv.DetallesDocumento)
                .SetCollectionValidator(new InvoiceDetailValidator());
        }
    }
}
