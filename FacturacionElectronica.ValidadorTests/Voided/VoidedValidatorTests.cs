using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Enums;
using FacturacionElectronica.Validador.Voided;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.ValidadorTests.Voided
{
    [TestClass]
    public class VoidedValidatorTests
    {
        [TestMethod]
        public void VoidedValidatorTest()
        {
            var voided = new VoidedHeader
            {
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                RucEmisor = "20600995805",
                FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                NombreRazonSocialEmisor = "ABLIMATEX EXPORT SAC",
                NombreComercialEmisor = "C-ABLIMATEX EXPORT SAC",
                CorrelativoArchivo = "01",
                DetallesDocumento = new List<VoidedDetail>
                {
                    new VoidedDetail{
                        TipoDocumento = TipoDocumentoElectronico.Factura,
                        SerieDocumento = "F001",
                        CorrelativoDocumento = "1",
                        Motivo = "ERROR EN SISTEMA",
                    },
                    new VoidedDetail{
                        TipoDocumento = TipoDocumentoElectronico.Factura,
                        SerieDocumento = "F001",
                        CorrelativoDocumento = "15",
                        Motivo = "CANCELACION"
                    }
                }
            };
            var validator = new VoidedValidator();
            var result = validator.Validate(voided);
            
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void VoidedFailedValidatorTest()
        {
            var voided = new VoidedHeader
            {
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                RucEmisor = "20600995805",
                FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                NombreRazonSocialEmisor = "ABLIMATEX EXPORT SAC",
                NombreComercialEmisor = "C-ABLIMATEX EXPORT SAC",
                CorrelativoArchivo = "01",
                DetallesDocumento = new List<VoidedDetail>
                {
                    new VoidedDetail{
                        TipoDocumento = TipoDocumentoElectronico.Factura,
                        SerieDocumento = "G001",
                        CorrelativoDocumento = "14567889",
                        Motivo = "ERROR EN SISTEMA",
                    },
                    new VoidedDetail{
                        TipoDocumento = TipoDocumentoElectronico.Factura,
                        SerieDocumento = "F001",
                        CorrelativoDocumento = "15",
                        Motivo = "CANCELACION"
                    }
                }
            };
            IValidator validator = new VoidedValidator();
            var result = validator.Validate(voided);

            Assert.IsFalse(result.IsValid);

            Trace.WriteLine(string.Join(Environment.NewLine, result.Errors.Select(r => r.ErrorMessage)));
        }
    }
}