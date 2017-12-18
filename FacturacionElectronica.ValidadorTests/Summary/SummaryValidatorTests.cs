using System;
using System.Collections.Generic;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Entity.Misc;
using FacturacionElectronica.GeneradorXml.Enums;
using FacturacionElectronica.Validador.Summary;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.ValidadorTests.Summary
{
    [TestClass]
    public class SummaryValidatorTests
    {
        [TestMethod]
        public void SummaryValidatorTest()
        {
            var summary = CreateSummary();

            IValidator validator = new SummaryValidator();
            var result = validator.Validate(summary);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void SummaryValidatorNoValidTest()
        {
            var summary = CreateSummary();
            summary.FechaEmision = DateTime.Now.AddDays(3);
            IValidator validator = new SummaryValidator();
            var result = validator.Validate(summary);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        private SummaryHeader CreateSummary()
        {
            var summary = new SummaryHeader
            {
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                RucEmisor = "20600995805",
                FechaEmision = DateTime.Now.Date,
                NombreRazonSocialEmisor = "ABLIMATEX EXPORT SAC",
                NombreComercialEmisor = "C-ABLIMATEX EXPORT SAC",
                CorrelativoArchivo = "01",
                CodigoMoneda = "PEN",
                DetallesDocumento = new List<SummaryDetail>
                {
                    new SummaryDetail{
                        TipoDocumento = TipoDocumentoElectronico.Boleta,
                        SerieDocumento = "BA98",
                        NroCorrelativoInicial = "456",
                        NroCorrelativoFinal = "764",
                        Total = 100,
                        Importe = new List<TotalImporteType>
                        {
                            new TotalImporteType
                            {
                                TipoImporte = TipoValorVenta.Gravado,
                                Monto = 98232.00M,
                            },
                            new TotalImporteType
                            {
                                TipoImporte = TipoValorVenta.Exonerado,
                                Monto = 20,
                            },
                            new TotalImporteType
                            {
                                TipoImporte = TipoValorVenta.Inafecto,
                                Monto = 232,
                            }
                        },
                        OtroImporte = new List<TotalImporteExtType>
                        {
                            new TotalImporteExtType
                            {
                                Indicador = true,
                                Monto = 5,
                            }
                        },
                        Impuesto = new List<TotalImpuestosType>
                        {
                            new TotalImpuestosType
                            {
                                Monto = 17681.76M,
                                TipoTributo = TipoTributo.IGV_VAT
                            },
                            new TotalImpuestosType
                            {
                                Monto = 1200,
                                TipoTributo = TipoTributo.ISC_EXC
                            }
                        }
                    },
                    new SummaryDetail{
                        TipoDocumento = TipoDocumentoElectronico.Boleta,
                        SerieDocumento = "BC23",
                        NroCorrelativoInicial = "789",
                        NroCorrelativoFinal = "932",
                        Total = 200,
                        Importe = new List<TotalImporteType>
                        {
                            new TotalImporteType
                            {
                                TipoImporte = TipoValorVenta.Gravado,
                                Monto = 78223,
                            },
                            new TotalImporteType
                            {
                                TipoImporte = TipoValorVenta.Exonerado,
                                Monto = 24423,
                            },
                            new TotalImporteType
                            {
                                TipoImporte = TipoValorVenta.Inafecto,
                                Monto = 45,
                            }
                        },
                        OtroImporte = new List<TotalImporteExtType>(),
                        Impuesto = new List<TotalImpuestosType>
                        {
                            new TotalImpuestosType
                            {
                                Monto = 14080.14M,
                                TipoTributo = TipoTributo.IGV_VAT
                            },
                            new TotalImpuestosType
                            {
                                Monto = 0,
                                TipoTributo = TipoTributo.ISC_EXC
                            }
                        }
                    }
                }
            };

            return summary;
        }
    }
}