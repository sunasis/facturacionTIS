using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Enums;
using FluentValidation;

namespace FacturacionElectronica.Validador.Sale
{
    /// <summary>
    /// Class DebitNoteValidator.
    /// </summary>
    public class DebitNoteValidator : AbstractValidator<DebitNoteHeader>
    {
        /// <inheritdoc />
        public DebitNoteValidator()
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
                .WithMessage("La serie no empieza con F o B");
            RuleFor(inv => inv.DocumentoReferenciaNumero)
                .NotEmpty()
                .MaximumLength(13)
                .WithMessage("Numero de documento de referencia requerido hasta 13 caracteres");
            RuleFor(inv => inv.Motivo)
                .Length(1, 250)
                .WithMessage("El motivo de la nota es requerido hasta 250 caracteres");
            RuleFor(inv => inv.DetallesDocumento)
                .SetCollectionValidator(new InvoiceDetailValidator());
        }
    }
}
