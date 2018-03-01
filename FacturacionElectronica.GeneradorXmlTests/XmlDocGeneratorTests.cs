﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using FacturacionElectronica.GeneradorXml;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Entity.Misc;
using FacturacionElectronica.GeneradorXml.Enums;
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
            var invoice = new InvoiceHeader
            {
                TipoDocumento = TipoDocumentoElectronico.Factura,
                SerieDocumento = "F001",
                CorrelativoDocumento = "00005214",
                FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                NombreRazonSocialCliente = "SUPERMERCADOS Ñro <xml> íó PERUANOS SOCIEDAD ANóNIMA 'O ' S.P.S.A.",
                NroDocCliente = "20100070970",
                CorreoCliente = "juan@gmail.com",
                RucEmisor = "20600995805",
                NombreRazonSocialEmisor = "ABLIMATEX EXPORT SAC",
                CodigoMoneda = "PEN",
                NombreComercialEmisor = "C-ABLIMATEX EXPORT SAC",
                DocumentoReferenciaNumero = "F001-2233",
                Compra = "00000043",
                VendedorCodigo = "112",
                VendedorNombre = "JUAN PEREZ",
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

            var res = _generator.GeneraDocumentoInvoice(invoice);

            if (!res.Success)
            {
                Trace.WriteLine(res.Error);
            }
            Assert.IsTrue(res.Success);

            var name = res.FileName + ".xml";
            File.WriteAllBytes(name, res.Content);
            
            Assert.IsNotNull(res.Content);
            Assert.IsTrue(res.Content.Length > 0);

            if (Environment.GetEnvironmentVariable("CI") == null)
            {
                Process.Start(name);
            }
        }


        [TestMethod]
        public void GeneraDocumentoInvoiceAnticipoTest()
        {
            const string rucEmisor = "20600995805";
            var invoice = new InvoiceHeader
            {
                TipoDocumento = TipoDocumentoElectronico.Factura,
                SerieDocumento = "F001",
                CorrelativoDocumento = "00005214",
                FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                NombreRazonSocialCliente = "SUPERMERCADOS PERUANOS SOCIEDAD ANóNIMA 'O ' S.P.S.A.",
                NroDocCliente = "20100070970",
                RucEmisor = rucEmisor,
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
                Anticipos = new List<AnticipoType>(1)
                {
                    new AnticipoType
                    {
                        NroDocumentRel = "F001-21",
                        RucEmisorDoc = rucEmisor,
                        TipoDocRel = DocRelTributario.FacturaAnticipos,
                        MontoAnticipo = 123.00M
                    }
                },
                DescuentoGlobal = 20.0m,
                TotalVenta = 377.60M,
                TotalAnticipos = 123.00M,
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

            var res = _generator.GeneraDocumentoInvoice(invoice);

            if (!res.Success)
                Trace.WriteLine(res.Error);

            File.WriteAllBytes("anticipos.xml", res.Content);
            Assert.IsTrue(res.Success);
            Assert.IsNotNull(res.Content);
            Assert.IsTrue(res.Content.Length > 0);
        }

        [TestMethod]
        public void GenerarDocumentoVoidedTest()
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

            var res = _generator.GenerarDocumentoVoided(voided);

            if (!res.Success)
                Trace.WriteLine(res.Error);

            Assert.IsTrue(res.Success);
            Assert.IsNotNull(res.Content);
            Assert.IsTrue(res.Content.Length > 0);
        }

        [TestMethod]
        public void GenerateSummaryTest()
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

            var res = _generator.GenerarDocumentoSummary(summary);
            if (!res.Success)
                Trace.WriteLine(res.Error);

            Assert.IsTrue(res.Success);
            Assert.IsNotNull(res.Content);
            Assert.IsTrue(res.Content.Length > 0);

            File.WriteAllBytes("20600995805-RC-20171128-01.xml", res.Content);
        }

        [TestMethod]
        public void GenerateSummaryV2Test()
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
                        Documento = "B001-1",
                        TipoDocumentoIdentidadCliente = TipoDocumentoIdentidad.DocumentoNacionalIdentidad,
                        NroDocCliente = "99887766",
                        Estado = EstadoResumen.Adicionar,
                        Total = 100.422M,
                        Percepcion = new PerceptionSummaryType
                        {
                            CodRegimen = RegimenPercepcion.VENTA_INTERNA,
                            Tasa = 2,
                            Monto = 2,
                            MontoTotal = 102,
                            MontoBase = 100
                        },
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
                        Documento = "B001-00000001",
                        TipoDocumentoIdentidadCliente = TipoDocumentoIdentidad.DocumentoNacionalIdentidad,
                        Estado = EstadoResumen.Anulado,
                        NroDocCliente = "55443322",
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

            var res = _generator.GenerarDocumentoSummary(summary, true);
            if (!res.Success)
                Trace.WriteLine(res.Error);

            Assert.IsTrue(res.Success);
            Assert.IsNotNull(res.Content);
            Assert.IsTrue(res.Content.Length > 0);
        }
    }
}