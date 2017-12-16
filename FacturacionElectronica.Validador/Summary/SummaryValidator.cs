using System;
using FacturacionElectronica.GeneradorXml.Entity;
using FluentValidation;

namespace FacturacionElectronica.Validador.Summary
{
    /// <summary>
    /// Class SummaryValidator.
    /// </summary>
    public class SummaryValidator : AbstractValidator<SummaryHeader>
    {
        /// <inheritdoc />
        public SummaryValidator()
        {
            RuleFor(inv => inv.FechaEmision)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha de emision es mayor a la fecha de envío del comprobante");
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
            RuleFor(inv => inv.CorrelativoArchivo)
                .NotEmpty()
                .MaximumLength(5)
                .WithMessage("El correlativo del archivo debe tener un maximo de 5 caracteres");
            RuleFor(inv => inv.DetallesDocumento)
                .SetCollectionValidator(new SummaryDetailValidator());
        }
    }
}
