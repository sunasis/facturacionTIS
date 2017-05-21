using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using FacturacionElectronica.GeneradorXml;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Entity.Misc;
using FacturacionElectronica.GeneradorXml.Enums;
using FacturacionElectronica.GeneradorXml.Res;
using Gs.Ubl.v2.Sac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.GeneradorXmlTests
{
    [TestClass]
    public class XmlDocGeneratorTests
    {
        private readonly XmlDocGenerator _generator;

        public XmlDocGeneratorTests()
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "Resources", "SFSCert.p");
            var certify = new X509Certificate2(File.ReadAllBytes(filename), "123456");
            _generator = new XmlDocGenerator(certify);
        }

        [TestMethod]
        public void GeneraDocumentoInvoiceTest()
        {
            var result = new OperationResult();
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
                InfoAddicional = new List<AdditionalPropertyType>()
                {
                    new AdditionalPropertyType
                    {
                        ID = "1000",
                        Value = "MIL QUINIENTOS SETENTE Y TRES CON 60/100"//UtilsFacturacion.ConvertirenLetras(156377.60M)
                    }
                },
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

            var res = _generator.GeneraDocumentoInvoice(ref result, invoice);

            if (!result.Success)
                Trace.WriteLine(result.Error);

            Assert.IsTrue(result.Success);

            var pathXml = Path.Combine(Path.GetTempPath(), res.FileName + ".xml");
            File.WriteAllBytes(pathXml, res.Content);

            Assert.IsTrue(File.Exists(pathXml));
            Trace.WriteLine("Ruta: " + pathXml);
        }

        [TestMethod]
        public void GenerarDocumentoVoidedTest()
        {
            var result = new OperationResult();
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

            var res = _generator.GenerarDocumentoVoided(ref result, voided);

            if (!result.Success)
                Trace.WriteLine(result.Error);

            Assert.IsTrue(result.Success);

            var pathXml = Path.Combine(Path.GetTempPath(), res.FileName + ".xml");
            File.WriteAllBytes(pathXml, res.Content);

            Assert.IsTrue(File.Exists(pathXml));
            Trace.WriteLine("Ruta: " + pathXml);
        }
    }
}