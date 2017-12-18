using System;
using System.Collections.Generic;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Entity.Misc;
using FacturacionElectronica.GeneradorXml.Enums;
using FacturacionElectronica.Validador.Sale;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.ValidadorTests.Sale
{
    [TestClass]
    public class InvoiceValidatorTests
    {
        [TestMethod]
        public void InvoiceValidatorTest()
        {
            var invoice = CreateInvoice();

            IValidator validator = new InvoiceValidator();
            var result = validator.Validate(invoice);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void InvoicenValidatorNoValidTest()
        {
            var invoice = CreateInvoice();

            invoice.SerieDocumento = "G001";

            IValidator validator = new InvoiceValidator();
            var result = validator.Validate(invoice);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        private InvoiceHeader CreateInvoice()
        {
            var invoice = new InvoiceHeader
            {
                TipoDocumento = TipoDocumentoElectronico.Factura,
                SerieDocumento = "F001",
                CorrelativoDocumento = "00005214",
                FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                NombreRazonSocialCliente = "SUPERMERCADOS Ñro <xml> íó PERUANOS SOCIEDAD ANóNIMA 'O ' S.P.S.A.",
                NroDocCliente = "20100070970",
                RucEmisor = "20600995805",
                NombreRazonSocialEmisor = "ABLIMATEX EXPORT SAC",
                CodigoMoneda = "PEN",
                NombreComercialEmisor = "C-ABLIMATEX EXPORT SAC",
                DocumentoReferenciaNumero = "F001-2233",
                DocumentoReferenciaTipoDocumento = TipoDocumentoElectronico.Factura,
                TipoDocumentoIdentidadCliente = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                GuiaRemisionReferencia = new List<GuiaRemisionType>
                {
                    new GuiaRemisionType {IdTipoGuiaRemision = "09", NumeroGuiaRemision = "0001-111111"}
                },
                DetallesDocumento = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        CodigoProducto = "PROD001",
                        Cantidad = 120,
                        DescripcionProducto = "PRODUCTO PRUEBA 001",
                        PrecioUnitario = 1,
                        PrecioAlternativos = new List<PrecioItemType>
                        {
                            new PrecioItemType
                            {
                                TipoDePrecio = TipoPrecioVenta.PrecioUnitario,
                                Monto = 1.18M
                            }
                        },
                        UnidadMedida = "NIU",
                        ValorVenta = 120,
                        Impuesto =
                            new List<TotalImpuestosType>
                            {
                                new TotalImpuestosType {TipoTributo = TipoTributo.IGV_VAT, Monto = 21.60M, TipoAfectacion=TipoAfectacionIgv.GravadoOperacionOnerosa},
                            },
                    },
                    new InvoiceDetail
                    {
                        CodigoProducto = "PROD002",
                        Cantidad = 100,
                        DescripcionProducto = "PRODUCTO PRUEBA 002",
                        PrecioUnitario = 2,
                        PrecioAlternativos = new List<PrecioItemType>
                        {
                            new PrecioItemType
                            {
                                TipoDePrecio = TipoPrecioVenta.PrecioUnitario,
                                Monto = 2.36M
                            }
                        },
                        UnidadMedida = "NIU",
                        ValorVenta = 200,
                        Impuesto = new List<TotalImpuestosType>
                        {
                            new TotalImpuestosType {TipoTributo = TipoTributo.IGV_VAT, Monto = 36M, TipoAfectacion = TipoAfectacionIgv.GravadoOperacionOnerosa},
                        },
                    },
                },
                DescuentoGlobal = 20.0m,
                TotalVenta = 377.60M,
                TotalTributosAdicionales = new List<TotalTributosType>
                {
                    new TotalTributosType
                    {
                        Id = OtrosConceptosTributarios.TotalVentaOperacionesGravadas,
                        MontoPagable = 320M
                    },
                    new TotalTributosType
                    {
                        Id = OtrosConceptosTributarios.Detracciones,
                        MontoPagable = 2208.962M,
                        Porcentaje = 9.000M
                    }
                },
                Impuesto = new List<TotalImpuestosType>
                {
                    new TotalImpuestosType {TipoTributo = TipoTributo.IGV_VAT, Monto = 57.6M},
                    new TotalImpuestosType {TipoTributo = TipoTributo.ISC_EXC, Monto = 0M}
                },
                DireccionEmisor = new DireccionType
                {
                    CodigoUbigueo = "150111",
                    Direccion = "AV. LOS PRECURSORES #1245",
                    Zona = "URB. MIGUEL GRAU",
                    Departamento = "LIMA",
                    Provincia = "LIMA",
                    Distrito = "EL AGUSTINO",
                    CodigoPais = "PE"
                }
            };

            return invoice;
        }
    }
}