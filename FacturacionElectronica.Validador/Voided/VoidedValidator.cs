using System;
using FacturacionElectronica.GeneradorXml.Entity;
using FluentValidation;

namespace FacturacionElectronica.Validador.Voided
{
    /// <summary>
    /// Class VoidedValidator.
    /// </summary>
    public class VoidedValidator : AbstractValidator<VoidedHeader>
    {
        /// <inheritdoc />
        public VoidedValidator()
        {
            RuleFor(voided => voided.FechaEmision).LessThan(DateTime.Now).WithMessage("La fecha de emision es mayor a la fecha actual");
            RuleFor(voided => voided.RucEmisor).NotEmpty().WithMessage("No existe ruc del emisor");
            RuleFor(voided => voided.NombreRazonSocialEmisor).NotEmpty().MaximumLength(100).WithMessage("La razon social del emisor debe tener de 1-100 caracteres");
            RuleFor(voided => voided.DetallesDocumento).SetCollectionValidator(new VoidedDetailValidator());
        }
    }
}
