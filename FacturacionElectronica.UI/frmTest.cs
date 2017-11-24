using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using FacturacionElectronica.GeneradorXml;
using FacturacionElectronica.GeneradorXml.Entity;
using FacturacionElectronica.GeneradorXml.Entity.Details;
using FacturacionElectronica.GeneradorXml.Entity.Misc;
using FacturacionElectronica.GeneradorXml.Res;
using FacturacionElectronica.Homologacion;
using FacturacionElectronica.Homologacion.Res;
using FacturacionElectronica.GeneradorXml.Enums;
using FacturacionElectronica.UI.Properties;
using Gs.Ubl.v2;
using Gs.Ubl.v2.Cac;
using Gs.Ubl.v2.Ext;
using Gs.Ubl.v2.Udt;
using Gs.Ubl.v2.Sac;

namespace FacturacionElectronica.UI
{
    // ReSharper disable once InconsistentNaming
    public partial class frmTest : Form
    {
        private readonly SunatManager _ws;
        private X509Certificate2 _cert;

        public frmTest()
        {
            _ws = new SunatManager("20600995805", "MODDATOS", "moddatos");

            InitializeComponent();
            var pathCert = Path.Combine(Environment.CurrentDirectory, "Resources", "SFSCert.p");

            _cert = new X509Certificate2(pathCert, "123456");
            cboTipoService.SelectedIndex = (int)SunatManager.CurrentService;
            cboTipoService.SelectedIndexChanged += cboTipoService_SelectedValueChanged;
            cboServiceRet.SelectedIndex = (int) SunatCeR.CurrentService;
            cboServiceRet.SelectedIndexChanged += delegate
            {
                if (cboTipoService.SelectedIndex >= 0)
                {
                    var en = (ServiceSunatType)Enum.ToObject(typeof(ServiceSunatType), cboServiceRet.SelectedIndex);
                    SunatCeR.CurrentService = en;
                }
            };
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SendXMl();
            //return;
            try
            {
                #region Se llena la entidad Invoice para pasarla al Xml

                var invoiceHeaderEntity = new InvoiceHeader
                {
                    TipoDocumento = TipoDocumentoElectronico.Factura,
                    SerieDocumento = "F001",
                    CorrelativoDocumento = "00005214",
                    FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    NombreRazonSocialCliente = "SUPERMERCADOS PERUANOS SOCIEDAD ANONIMA 'O ' S.P.S.A.",
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
                      },
                      new AdditionalPropertyType
                      {
                          ID = "2000",
                          Value = "COMPROBANTE DE PERCEPCION"
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
                        new TotalTributosType()
                        {
                            Id = OtrosConceptosTributarios.Detracciones,
                            MontoPagable = 2208.962M,
                            Porcentaje = 9.000M
                        },
                        new TotalTributosType()
                        {
                            Id = OtrosConceptosTributarios.Percepciones,
                            MontoPagable = 1427.10M,
                            MontoTotal = 72782.093M,
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
                #endregion

                var objOperationResult = new OperationResult();
                var xmlResultPath = new XmlDocGenerator(_cert).GeneraDocumentoInvoice(ref objOperationResult, invoiceHeaderEntity);
                if (!objOperationResult.Success)
                {
                    MessageBox.Show(
                        objOperationResult.Error + '\n' + objOperationResult.InnerException + '\n' +
                        objOperationResult.Target, @"Error en el Process.", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(xmlResultPath)) return;
                if (!File.Exists(xmlResultPath)) return;
                Process.Start(xmlResultPath);
                var res = _ws.SendDocument(xmlResultPath);
                if (res.Success)
                {
                    if(res.ApplicationResponse == null) return;
                    File.WriteAllBytes(Path.GetTempPath() + "/fileZip.zip", res.ContentZip);
                    Process.Start(Path.GetTempPath() + "/fileZip.zip");
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error en el llenado de la entidad.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //new WebServiceClient().Run();
            //return;
            //SendXMl();
            //return;
            try
            {
                #region Se llena la entidad Invoice para pasarla al Xml

                var invoiceHeaderEntity = new InvoiceHeader
                {
                    TipoDocumento = TipoDocumentoElectronico.Factura,
                    SerieDocumento = "F001",
                    CorrelativoDocumento = "00000003",
                    FechaEmision = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    NombreRazonSocialCliente = "SUPERMERCADOS PERUANOS SOCIEDAD ANONIMA 'O ' S.P.S.A.",
                    NroDocCliente = "20100070970",
                    RucEmisor = "20600995805",
                    NombreRazonSocialEmisor = "ABLIMATEX EXPORT SAC",
                    CodigoMoneda = "PEN",
                    NombreComercialEmisor = "C-ABLIMATEX EXPORT SAC",
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
                    DetallesDocumento = new List<InvoiceDetail>
                    {
                        new InvoiceDetail
                        {
                            CodigoProducto = "PROD001",
                            Cantidad = 2,
                            DescripcionProducto = "PRODUCTO PRUEBA 001",
                            PrecioUnitario = 50,
                            PrecioAlternativos = new List<PrecioItemType>
                            {
                                new PrecioItemType
                                {
                                    TipoDePrecio = TipoPrecioVenta.PrecioUnitario,
                                    Monto = 59
                                }
                            },
                            UnidadMedida = "NIU",
                            ValorVenta = 118,
                            Impuesto =
                                new List<TotalImpuestosType>
                                {
                                    new TotalImpuestosType {TipoTributo = TipoTributo.IGV_VAT, Monto = 18, TipoAfectacion=TipoAfectacionIgv.GravadoOperacionOnerosa},
                                },
                        }
                    },
                    TotalVenta = 118,
                    TotalTributosAdicionales = new List<TotalTributosType>
                    {
                        new TotalTributosType
                        {
                            Id = OtrosConceptosTributarios.TotalVentaOperacionesGravadas,
                            MontoPagable = 100
                        }
                    },
                    Impuesto = new List<TotalImpuestosType>
                    {
                        new TotalImpuestosType {TipoTributo = TipoTributo.IGV_VAT, Monto = 18},
                        new TotalImpuestosType {TipoTributo = TipoTributo.ISC_EXC, Monto = 0M}
                    },
                    DireccionEmisor = new DireccionType
                    {
                        CodigoUbigueo = "150111",
                        Direccion = "AV. LOS PRECURSORES #1245",
                        Departamento = "LIMA",
                        Provincia = "LIMA",
                        Distrito = "EL AGUSTINO",
                        CodigoPais = "PE"
                    }
                };
                #endregion

                var objOperationResult = new OperationResult();
                var xmlResultPath = new XmlDocGenerator(_cert).GeneraDocumentoInvoice(ref objOperationResult, invoiceHeaderEntity);
                if (!objOperationResult.Success)
                {
                    MessageBox.Show(
                        objOperationResult.Error + '\n' + objOperationResult.InnerException + '\n' +
                        objOperationResult.Target, @"Error en el Process.", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(xmlResultPath)) return;
                if (!File.Exists(xmlResultPath)) return;
                //xmlResultPath = @"C:\Users\Usuario\Downloads\20600695771-01-F001-00000001.xml";
                Process.Start(xmlResultPath);
                var res = _ws.SendDocument(xmlResultPath);
                if (res.Success)
                {
                    if (res.ApplicationResponse == null) return;
                    File.WriteAllBytes(Path.GetTempPath() + "/fileZip.zip", res.ContentZip);
                    Process.Start(Path.GetTempPath() + "/fileZip.zip");
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error en el llenado de la entidad.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnVoided_Click(object sender, EventArgs e)
        {
            VoidedHeader voided = new VoidedHeader
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
            OperationResult objOperationResult = new OperationResult();
            string xmlResultPath = new XmlDocGenerator(_cert).GenerarDocumentoVoided(ref objOperationResult, voided);
            if (!string.IsNullOrEmpty(xmlResultPath))
            {
                if (File.Exists(xmlResultPath))
                {
                    Process.Start(xmlResultPath);
                    TicketResponse res = _ws.SendSummary(xmlResultPath);
                    if (res.Success)
                    {
                        txtTicket.Text = res.Ticket;
                        MessageBox.Show(@"Ticket :" + res.Ticket, @"Exito");
                    }
                    else
                    {
                        MessageBox.Show(
                            string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                    }
                }
            }
        }

        private void SendXMl()
        {
            var dlg = new OpenFileDialog
            {
                Filter = @"Xml Files |*.xml"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var firmar = XmlSignatureProvider.SignXmlFileTest(dlg.FileName, _cert);
            Process.Start(firmar.Value);
            if (firmar.Key)
            {
                var resp = _ws.SendDocument(firmar.Value);
                if (resp.Success)
                {
                    var response = resp.ApplicationResponse;
                    if (response != null)
                    {
                        MessageBox.Show(
                            $"Code: {response.Codigo}\n Description: {response.Descripcion}\n" +
                            $"FileZip : {resp.ContentZip.Length}",
                            @"Exito");
                        var dialog = new SaveFileDialog {Filter = @"Zip Files (*.zip)|*.zip"};
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(dialog.FileName, resp.ContentZip);
                            Process.Start(dialog.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"code : {resp.Error.Code} \n Descripcion: {resp.Error.Description}");
                    }
                }
            }
            else
            {
                MessageBox.Show(@"No se pudo firmar");
            }
        }

        private void TestSummary()
        {
            var dlg = new OpenFileDialog
            {
                Filter = @"Xml Files |*.xml"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var firmar = XmlSignatureProvider.SignXmlFileTest(dlg.FileName, _cert);
            Process.Start(firmar.Value);
            if (firmar.Key)
            {
                var resp = _ws.SendSummary(firmar.Value);
                txtTicket.Text = resp.Ticket;
                MessageBox.Show(resp.Success
                    ? resp.Ticket
                    : $"ErrorCode : {resp.Error.Code}\n Message: {resp.Error.Description}");
            }
            else
            {
                MessageBox.Show(@"No se pudo firmar");
            }
        }
        private void btnResumen_Click(object sender, EventArgs e)
        {
            //TestSummary();
            //return;
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
                        Documento = "B001-00000001",
                        TipoDocumentoIdentidadCliente = TipoDocumentoIdentidad.DocumentoNacionalIdentidad,
                        Estado = EstadoResumen.Anulado,
                        NroDocCliente = "55443322",
                        SerieDocumento = "BC23",
                        NroCorrelativoInicial = "789",
                        NroCorrelativoFinal = "932",
                        Total = 200,
                        //Referencia = new List<DocReferenciaType>()
                        //{
                        //  new DocReferenciaType()
                        //  {
                        //      Documento = "B001-00000003",
                        //      TipoDocumento = TipoDocumentoElectronico.Boleta
                        //  }  
                        //},
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
            var objOperationResult = new OperationResult();
            string xmlResultPath = new XmlDocGenerator(_cert).GenerarDocumentoSummary(ref objOperationResult, summary);
            if (objOperationResult.Success)
            {
                if (File.Exists(xmlResultPath))
                {
                    Process.Start(xmlResultPath);
                    TicketResponse res = _ws.SendSummary(xmlResultPath);
                    if (res.Success)
                    {
                        MessageBox.Show(@"Ticket :" + res.Ticket, @"Exito");
                        txtTicket.Text = res.Ticket;
                    }
                    else
                    {
                        MessageBox.Show(
                            string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                    }
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTicket.Text))
            {
                SunatResponse res = _ws.GetStatus(txtTicket.Text.Trim());
                if (res.Success)
                {
                    var response = res.ApplicationResponse;
                    if (response != null)
                    {
                        MessageBox.Show(
                            $"Code: {response.Codigo}\n Description: {response.Descripcion}\n" +
                            $"FileZip : {res.ContentZip.Length}", @"Exito");
                        SaveFileDialog dialog = new SaveFileDialog {Filter = @"Zip Files (*.zip)|*.zip"};
                        if(dialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(dialog.FileName, res.ContentZip);
                            Process.Start(dialog.FileName);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog
            {
                Filter = @"Cert Files (*.cert, *.pfx)|*.cert;*.pfx",
                Title = @"Abrir certificado Digital"
            };
            if (d.ShowDialog() != DialogResult.OK) return;
            _cert = new X509Certificate2(d.FileName, txtCert.Text);
            _cert.Export(X509ContentType.SerializedCert);
            MessageBox.Show(_cert.Subject, _cert.SubjectName.Name);
        }

        private void btnCredit_Click(object sender, EventArgs e)
        {
            NotasBase<InvoiceDetail> credit = new NotasBase<InvoiceDetail>
            {
                SerieDocumento = "F001",
                CorrelativoDocumento = "211",
                FechaEmision = DateTime.Now.Date,
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                RucEmisor = "20600995805",
                NombreRazonSocialEmisor = "INTELIGENCIA DE VENTAS SAC",
                NombreComercialEmisor = "INTELIGENCIA DE VENTAS SAC",
                NroDocCliente = "20587896411",
                TipoDocumentoIdentidadCliente = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                NombreRazonSocialCliente = "SERVICABINAS S.A.",
                DocumentoRef = "F001-4355",
                TipoDocRef = TipoDocumentoElectronico.Factura,
                Motivo = "Unidades defectuosas, no leen CD que contengan archivos MP3",
                CodigoMoneda = "PEN",
                Total = 9799.99794M,
                DocumentoReferenciaNumero = "F001-2233",
                DocumentoReferenciaTipoDocumento = TipoDocumentoElectronico.Factura,
                TotalTributosAdicionales = new List<TotalTributosType>
                {
                    new TotalTributosType
                    {
                        Id = OtrosConceptosTributarios.TotalVentaOperacionesGravadas,
                        MontoPagable = 8305.083M,
                    }
                },
                GuiaRemisionReferencia = new List<GuiaRemisionType>
                    {
                        new GuiaRemisionType {IdTipoGuiaRemision = "09", NumeroGuiaRemision = "0001-111111"}
                    },
                DireccionEmisor = new DireccionType
                {
                    CodigoUbigueo = "150111",
                    Direccion = "AV. LOS PRECURSORES #1245",
                    Zona = "Urb. Miguel Grau",
                    Departamento = "LIMA",
                    Provincia = "LIMA",
                    Distrito = "EL AGUSTINO",
                    CodigoPais = "PE"
                },
                Impuesto = new List<TotalImpuestosType>
                {
                    new TotalImpuestosType
                    {
                        TipoTributo = TipoTributo.IGV_VAT,
                        Monto = 1494.92M,
                    }
                },
                DetallesDocumento = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        CodigoProducto = "GLG199",
                        DescripcionProducto = "Grabadora LG Externo Modelo: GE20LU10",
                        PrecioUnitario = 83.0523M,
                        PrecioAlternativos = new List<PrecioItemType>
                        {
                            new PrecioItemType
                            {
                                TipoDePrecio = TipoPrecioVenta.PrecioUnitario,
                                Monto = 98
                            }
                        },
                        Cantidad = 100,
                        ValorVenta = 8305.083M,
                        UnidadMedida = "NIU",
                        Impuesto = new List<TotalImpuestosType>
                        {
                            new TotalImpuestosType
                            {
                                TipoAfectacion = TipoAfectacionIgv.GravadoOperacionOnerosa,
                                TipoTributo = TipoTributo.IGV_VAT,
                                Monto = 1494.92M,
                            }
                        },    
                    }
                }
            };
            var notaCredito = new CreditNoteHeader(credit)
            {
                TipoNota = TipoNotaCreditoElectronica.CorrecionPorErrorDescripcion
            };
            var objOperationResult = new OperationResult();
            var xmlResultPath = new XmlDocGenerator(_cert).GenerarDocumentoCreditNote(ref objOperationResult, notaCredito);
            if (!objOperationResult.Success)
            {
                MessageBox.Show(
                    objOperationResult.Error + '\n' + objOperationResult.InnerException + '\n' +
                    objOperationResult.Target, @"Error en el Proceso.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (File.Exists(xmlResultPath))
            {
                Process.Start(xmlResultPath);
                var res = _ws.SendDocument(xmlResultPath);
                if (res.Success)
                {
                    var response = res.ApplicationResponse;
                    if (response == null) return;
                    MessageBox.Show(
                        $"Code: {response.Codigo}\n Description: {response.Descripcion}\n" +
                        $"FileZip : {res.ContentZip.Length}", @"Exito");
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                }
            }
        }

        private void btnDebit_Click(object sender, EventArgs e)
        {
            var debit = new DebitNoteHeader
            {
                SerieDocumento = "F001",
                CorrelativoDocumento = "0005",
                FechaEmision = DateTime.Now.Date,
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                RucEmisor = "20600995805",
                NombreRazonSocialEmisor = "INTELIGENCIA DE VENTAS SAC",
                NombreComercialEmisor = "INTELIGENCIA DE VENTAS SAC",
                NroDocCliente = "20587896411",
                TipoDocumentoIdentidadCliente = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                NombreRazonSocialCliente = "SERVICABINAS S.A.",
                DocumentoRef = "F001-4355",
                TipoDocRef = TipoDocumentoElectronico.Factura,
                TipoNota = TipoNotaDebitoElectronica.AumentoEnElValor,
                Motivo = "Ampliación garantía de memoria DDR-3 B1333 Kingston",
                CodigoMoneda = "PEN",
                Total = 1250.00M,
                TotalTributosAdicionales = new List<TotalTributosType>
                {
                    new TotalTributosType
                    {
                        Id = OtrosConceptosTributarios.TotalVentaOperacionesGravadas,
                        MontoPagable = 1250.00M,
                    }
                },
                DireccionEmisor = new DireccionType
                {
                    CodigoUbigueo = "150111",
                    Direccion = "AV. LOS PRECURSORES #1245",
                    Zona = "Urb. Miguel Grau",
                    Departamento = "LIMA",
                    Provincia = "LIMA",
                    Distrito = "EL AGUSTINO",
                    CodigoPais = "PE"
                },
                DetallesDocumento = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        CodigoProducto = "G1",
                        DescripcionProducto = " Ampliación de garantía de 6 a 12 meses de Memoria DDR-3 B1333 Kingston",
                        PrecioUnitario = 5.00M,
                        PrecioAlternativos = new List<PrecioItemType>
                        {
                            new PrecioItemType
                            {
                                TipoDePrecio = TipoPrecioVenta.PrecioUnitario,
                                Monto = 5.00M
                            }
                        },
                        Cantidad = 1,
                        ValorVenta = 1250.00M,
                        UnidadMedida = "ZZ",
                        Impuesto = new List<TotalImpuestosType>
                        {
                            new TotalImpuestosType
                            {
                                TipoAfectacion = TipoAfectacionIgv.ExoneradoOperacionOnerosa,
                                TipoTributo = TipoTributo.IGV_VAT,
                                Monto = 0
                            }
                        },
                    }
                }
            };
            var objOperationResult = new OperationResult();
            var xmlResultPath = new XmlDocGenerator(_cert).GenerarDocumentoDebitNote(ref objOperationResult, debit);
            if (!objOperationResult.Success)
            {
                MessageBox.Show(
                    objOperationResult.Error + '\n' + objOperationResult.InnerException + '\n' +
                    objOperationResult.Target, @"Error en el Proceso.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (File.Exists(xmlResultPath))
            {
                Process.Start(xmlResultPath);
                var res = _ws.SendDocument(xmlResultPath);
                if (res.Success)
                {
                    var response = res.ApplicationResponse;
                    if (response == null) return;
                    MessageBox.Show(
                        $"Code: {response.Codigo}\n Description: {response.Descripcion}\n" +
                        $"FileZip : {res.ContentZip.Length}", @"Exito");
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                }
            }
        }

        private DespatchAdviceType GenerarGuiaRemitente()
        {
            var disp = new DespatchAdviceType
            {
                UBLVersionID = "2.1",
                CustomizationID = "1.0",
                ID = "T001-00000001",
                IssueDate = DateTime.Now,
               
                DespatchAdviceTypeCode = "09",
                UBLExtensions = new[]
                {
                    new UBLExtensionType
                    {
                        ExtensionContent = new AdditionalsInformationType()
                    }
                },
                Note = new TextType[]
                {
                     "Transporto bolsas para basura"
                },
                OrderReference = new[]
                {
                    new OrderReferenceType
                    {
                        ID = "T001-8",
                        OrderTypeCode = "09"
                    }
                },
                //AdditionalDocumentReference = new[]
                //{
                //    new DocumentReferenceType
                //    {
                //        ID = "T001-8",
                //        DocumentTypeCode  = "09"
                //    }
                //},
                Signature = new[]
                {
                    new SignatureType
                    {
                        ID = "IdSunat",
                        SignatoryParty = new PartyType
                        {
                            PartyIdentification = new PartyIdentificationType []
                            {
                                "20131312955"
                            },
                            PartyName = new PartyNameType[]
                            {
                                "SUNAT"
                            }
                        },
                        DigitalSignatureAttachment = new AttachmentType
                        {
                            ExternalReference = new ExternalReferenceType
                            {
                                URI = "SignSUNAT"
                            }
                        }
                    }
                },
                DespatchSupplierParty = new SupplierPartyType
                {
                    CustomerAssignedAccountID = new IdentifierType
                    {
                        Value = "20131312955",
                        schemeID = "6"
                    },
                    Party = new PartyType
                    {
                        PartyLegalEntity = new[]
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = "PERUQUIMICOS S.A.C."
                            }
                        }
                    }
                },
                DeliveryCustomerParty = new CustomerPartyType
                {
                    CustomerAssignedAccountID = new IdentifierType
                    {
                        Value = "10209865209",
                        schemeID = "6"
                    },
                    Party = new PartyType
                    {
                        PartyLegalEntity = new[]
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = "RODRIGUEZ ROQUE AQUILES RUFO"
                            }
                        }
                    }
                },
                SellerSupplierParty = new SupplierPartyType
                {
                    CustomerAssignedAccountID = new IdentifierType
                    {
                        Value = "20100070970",
                        schemeID = "6"
                    },
                    Party = new PartyType
                    {
                        PartyLegalEntity = new[]
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = "PLAZA VEA"
                            }
                        }
                    }
                },
                Shipment = new ShipmentType
                {
                    ID = "1",
                    HandlingCode = "01",
                    Information = "VENTA",
                    SplitConsignmentIndicator = true,
                    GrossWeightMeasure = new MeasureType
                    {
                        Value = 10000.00M,
                        unitCode = "KGM"
                    },
                    TotalTransportHandlingUnitQuantity = 5,
                    
                    ShipmentStage = new[]
                    {
                        new ShipmentStageType
                        {
                          TransportModeCode  = "01", // Transporte Public
                          TransitPeriod = new PeriodType
                          {
                              StartDate = new DateTime(2016, 7, 01),
                          },
                          CarrierParty = new[]
                          {
                              new PartyType
                              {
                                  PartyIdentification = new[]
                                  {
                                      new PartyIdentificationType
                                      {
                                        ID = new IdentifierType
                                        {
                                            Value = "10209865209",
                                            schemeID = "6"
                                        }
                                      }
                                  },
                                  PartyName = new PartyNameType[]
                                  {
                                      "PERUQUIMICOS S.A.C."
                                  }
                              }
                          },
                          TransportMeans = new TransportMeansType
                          {
                              RoadTransport = new RoadTransportType
                              {
                                  LicensePlateID = "PGY-0988"
                              },
                          }
                        }
                    },
                    Delivery = new DeliveryType
                    {
                        DeliveryAddress = new AddressType
                        {
                            ID = "120606",
                            StreetName = "JR. MANTARO NRO. 257"
                        }
                    },
                    TransportHandlingUnit = new[]
                    {
                        new TransportHandlingUnitType
                        {
                            ID = "120606"
                        }
                    },
                    OriginAddress = new AddressType
                    {
                        ID = "150123", // 8 characters segun Guia
                        StreetName = "CAR. PANAM SUR KM 25 NO. 25050 NRO. 050 Z.I. CONCHAN"
                    },
                    FirstArrivalPortLocation = new LocationType1
                    {
                        ID = "PAI"
                    },

                },
                DespatchLine = new[]
                {
                    new DespatchLineType
                    {
                        ID = "1",
                        DeliveredQuantity  = new QuantityType
                        {
                            Value = 10M,
                            unitCode = "KGM"
                        },
                        OrderLineReference   = new []
                        {
                            new OrderLineReferenceType
                            {
                                LineID = "1" // ID
                            }
                        },
                        Item = new ItemType
                        {
                            Name = "ACETONA - 500.50 BALDE - 500.50 BALAS",
                            SellersItemIdentification = new ItemIdentificationType
                            {
                                ID = "COD1"
                            }
                        }
                    }
                }
            };
            return disp;
        }

        private void btnXML_Click(object sender, EventArgs e)
        {
            //TestGuiaRemision();
            //return;
            //var s = new OperationResult();
            //var path = new XmlDocGenerator(_cert).GenerarGuiaRemision(ref s, GenerarGuiaRemitente());
            var g = new XmlOtrosCeGenerator(_cert).GenerateDocGuia(GenerarGuiaRemitente());
            //var d = new OpenFileDialog
            //{
            //    Filter = @"XML Files(*.xml)|*.xml"
            //};
            if (File.Exists(g))
            {
                Process.Start(g);
                var con = new SunatGuiaRemision("20131312955", "MODDATOS", "moddatos");
                var res = con.SendDocument(g);
                if (res.Success)
                {
                    MessageBox.Show(res.ApplicationResponse.Descripcion);
                }
                else
                {
                    MessageBox.Show($"{res.Error.Code}\n{res.Error.Description}", @"Error");
                }
            }

        }

        private void cboTipoService_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboTipoService.SelectedIndex >= 0)
            {
                var en = (ServiceSunatType)Enum.ToObject(typeof(ServiceSunatType), cboTipoService.SelectedIndex);
                SunatManager.CurrentService = en;
            }
        }

        #region Retencion y Percepcion
        private void btnRetencion_Click(object sender, EventArgs e)
        {
            TestPercepcion();
            return;
            var dlg = new OpenFileDialog
            {
                Filter = @"Xml Files |*.xml"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var firmar = XmlSignatureProvider.SignXmlFileTest(dlg.FileName, _cert);
            Process.Start(firmar.Value);
            var g = new SunatCeR("20600995805", "MODDATOS", "moddatos");
            if (firmar.Key)
            {
                var resp = g.SendDocument(firmar.Value);
                
                MessageBox.Show(resp.Success
                    ? resp.ApplicationResponse.Descripcion
                    : $"ErrorCode : {resp.Error.Code}\n Message: {resp.Error.Description}");
                if (!resp.Success) return;
                SaveFileDialog dialog = new SaveFileDialog { Filter = @"Zip Files (*.zip)|*.zip" };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, resp.ContentZip);
                    Process.Start(dialog.FileName);
                }
            }
            else
            {
                MessageBox.Show(@"No se pudo firmar");
            }
        }

        private void btnCeBaja_Click(object sender, EventArgs e)
        {
            VoidedHeader voided = new VoidedHeader
            {
                TipoDocumentoIdentidadEmisor = TipoDocumentoIdentidad.RegistroUnicoContribuyentes,
                RucEmisor = "20600995805",
                FechaEmision = DateTime.Now.Date,
                NombreRazonSocialEmisor = "K&G Laboratorios",
                NombreComercialEmisor = "C-K&G Laboratorios",
                CorrelativoArchivo = "01",
                DetallesDocumento = new List<VoidedDetail>
                {
                    new VoidedDetail{
                        TipoDocumento = TipoDocumentoElectronico.ComprobanteRetencion,
                        SerieDocumento = "R001",
                        CorrelativoDocumento = "123",
                        Motivo = "ERROR EN SISTEMA",
                    },
                    new VoidedDetail{
                        TipoDocumento = TipoDocumentoElectronico.ComprobanteRetencion,
                        SerieDocumento = "R001",
                        CorrelativoDocumento = "100",
                        Motivo = "CANCELACION"
                    }
                }
            };
            OperationResult objOperationResult = new OperationResult();
            string xmlResultPath = new XmlOtrosCeGenerator(_cert).GenerarDocumentoVoided(voided);
            if (!string.IsNullOrEmpty(xmlResultPath))
            {
                if (File.Exists(xmlResultPath))
                {
                    Process.Start(xmlResultPath);
                    var g = new SunatCeR("20600995805", "MODDATOS", "moddatos");
                    TicketResponse res = g.SendSummary(xmlResultPath);
                    if (res.Success)
                    {
                        txtTicketCre.Text = res.Ticket;
                        MessageBox.Show(@"Ticket :" + res.Ticket, @"Exito");
                    }
                    else
                    {
                        MessageBox.Show(
                            string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                    }
                }
            }
        }

        private void btnCeStatus_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTicketCre.Text))
            {
                var ws = new SunatCeR("20600995805", "MODDATOS", "moddatos");
                var res = ws.GetStatus(txtTicketCre.Text.Trim());
                if (res.Success)
                {
                    var response = res.ApplicationResponse;
                    if (response != null)
                    {
                        MessageBox.Show(
                            $"Code: {response.Codigo}\n Description: {response.Descripcion}\n" +
                            $"FileZip : {res.ContentZip.Length}", @"Exito");
                        var dialog = new SaveFileDialog { Filter = @"Zip Files (*.zip)|*.zip" };
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(dialog.FileName, res.ContentZip);
                            Process.Start(dialog.FileName);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(
                        string.Format(Resources.frmTest_button1_Click_, res.Error.Code, res.Error.Description), @"Error");
                }
            }
        }

        private void TestRetention()
        {
            UblBaseDocumentType.GlbCustomizationID = "1.0";
            AmountType.TlsDefaultCurrencyID = "PEN";
            var ret = new RetentionType
            {
                ID = "R001-123",
                IssueDate = new DateTime(2015, 12, 24),
                UBLExtensions = new[]
                {
                    new UBLExtensionType
                    {
                        ExtensionContent = new AdditionalsInformationType()
                    },
                },
                Signature = new SignatureType
                {
                    ID = "IDSignSP",
                    SignatoryParty = new PartyType
                    {
                        PartyIdentification = new[]{
                            new PartyIdentificationType{
                                ID =  "20600995805"
                            }
                        },
                        PartyName = new[]{
                            new PartyNameType{
                                Name = "K&G Laboratorios"
                            }
                        }
                    },
                    DigitalSignatureAttachment = new AttachmentType
                    {
                        ExternalReference = new ExternalReferenceType
                        {
                            URI = "#SignatureSP"
                        }
                    }
                },
                AgentParty = new PartyType
                {
                    PartyIdentification = new[]
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType()
                                {
                                    schemeID = "6",
                                    Value = "20600995805"
                                }
                        }
                    },
                    PartyName = new PartyNameType[]
                    { "K&G Laboratorios"},
                    PostalAddress = new AddressType
                    {
                        ID = "150114",
                        StreetName = "AV. LOS OLIVOS 767",
                        CitySubdivisionName = "URB. SANTA FELICIA",
                        CityName = "LIMA",
                        CountrySubentity = "LIMA",
                        District = "MOLINA",
                        Country = new CountryType
                        {
                            IdentificationCode = "PE"
                        }
                    },
                    PartyLegalEntity = new[]
                    {
                        new PartyLegalEntityType()
                        {
                            RegistrationName = "K&G Asociados S. A."
                        }
                    }
                },
                ReceiverParty = new PartyType
                {
                    PartyIdentification = new[]
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = "6",
                                Value = "20100070970"
                            }
                        }
                    },
                    PartyName = new PartyNameType[]
                    { "K&G Laboratorios"},
                    PostalAddress = new AddressType
                    {
                        ID = "150130",
                        StreetName = "CAL. CALLE MORELLI 181 INT. P-2",
                        CitySubdivisionName = "-",
                        CityName = "LIMA",
                        CountrySubentity = "LIMA",
                        District = "SAN BORJA",
                        Country = new CountryType
                        {
                            IdentificationCode = "PE"
                        }
                    },
                    PartyLegalEntity = new[]
                    {
                        new PartyLegalEntityType()
                        {
                            RegistrationName = "SUPERMERCADOS PERUANOS SOCIEDAD ANONIMA O S.P.S.A"
                        }
                    }
                },
                SUNATRetentionSystemCode = "01",
                SUNATRetentionPercent = 3,
                Note = "Se emite con facturas del periodo 12/2015",
                TotalInvoiceAmount = 517.50M,
                SUNATTotalPaid = 16732.50M,
                SUNATRetentionDocumentReference = new[]
                {
                    new SUNATRetentionDocumentReferenceType
                    {
                        ID = new IdentifierType()
                        {
                            Value = "0001-14",
                            schemeID = "01"
                        },
                        IssueDate = new DateTime(2016,12,22),
                        TotalInvoiceAmount = 23000.00M,
                        Payment = new PaymentType[]
                        {
                            new PaymentType
                            {
                                ID = "1",
                                PaidAmount = 14000.00M,
                                PaidDate = new DateTime(2015,12,24)
                            },
                        },
                        SUNATRetentionInformation = new SUNATRetentionInformationType
                        {
                            SUNATRetentionAmount = 420.00M,
                            SUNATRetentionDate = new DateTime(2015,12,24),
                            SUNATNetTotalPaid = 13580.00M,
                            ExchangeRate = new ExchangeRateType
                            {
                                SourceCurrencyCode = "PEN",
                                TargetCurrencyCode = "PEN",
                                CalculationRate = 1,
                                Date = new DateTime(2015,12,24)
                            }
                        }
                    }, new SUNATRetentionDocumentReferenceType
                    {
                        ID = new IdentifierType()
                        {
                            Value = "E001-457",
                            schemeID = "01"
                        },
                        IssueDate = new DateTime(2016,12,10),
                        TotalInvoiceAmount = new AmountType
                        {
                            Value = 1000.00M,
                            currencyID = "USD"
                        },
                        Payment = new PaymentType[]
                        {
                            new PaymentType
                            {
                                ID = "1",
                                PaidAmount = new AmountType
                                {
                                    Value = 1000.00M,
                                    currencyID = "USD"
                                },
                                PaidDate = new DateTime(2015,12,24)
                            },
                        },
                        SUNATRetentionInformation = new SUNATRetentionInformationType
                        {
                            SUNATRetentionAmount = 97.50M,
                            SUNATRetentionDate = new DateTime(2015,12,24),
                            SUNATNetTotalPaid = 3152.50M,
                            ExchangeRate = new ExchangeRateType
                            {
                                SourceCurrencyCode = "USD",
                                TargetCurrencyCode = "PEN",
                                CalculationRate = 3.25M,
                                Date = new DateTime(2015,12,24)
                            }
                        }
                    }
                }
            };
            var gen = new XmlOtrosCeGenerator(_cert);
            var pathResult = gen.GenerateDocRetention(ret);
            if (gen.LastResult.Success && File.Exists(pathResult))
            {
                Process.Start(pathResult);
                var g = new SunatCeR("20600995805", "MODDATOS", "moddatos");
                var resp = g.SendDocument(pathResult);

                MessageBox.Show(resp.Success
                    ? resp.ApplicationResponse.Descripcion
                    : $"ErrorCode : {resp.Error.Code}\n Message: {resp.Error.Description}");
                SaveFileDialog dialog = new SaveFileDialog { Filter = @"Zip Files (*.zip)|*.zip" };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, resp.ContentZip);
                    Process.Start(dialog.FileName);
                }

            }
            else
            {
                MessageBox.Show(gen.LastResult.Error, @"No se pudo crear XML");
            }
        }

        private void TestPercepcion()
        {
            UblBaseDocumentType.GlbCustomizationID = "1.0";
            AmountType.TlsDefaultCurrencyID = "PEN";
            var ret = new PerceptionType
            {
                ID = "P001-123",
                IssueDate = new DateTime(2015, 12, 24),
                UBLExtensions = new[]
                {
                    new UBLExtensionType
                    {
                        ExtensionContent = new AdditionalsInformationType()
                    },
                },
                Signature = new SignatureType
                {
                    ID = "IDSignSP",
                    SignatoryParty = new PartyType
                    {
                        PartyIdentification = new[]{
                            new PartyIdentificationType{
                                ID =  "20600995805"
                            }
                        },
                        PartyName = new[]{
                            new PartyNameType{
                                Name = "K&G Laboratorios"
                            }
                        }
                    },
                    DigitalSignatureAttachment = new AttachmentType
                    {
                        ExternalReference = new ExternalReferenceType
                        {
                            URI = "#SignatureSP"
                        }
                    }
                },
                AgentParty = new PartyType
                {
                    PartyIdentification = new[]
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                                {
                                    schemeID = "6",
                                    Value = "20600995805"
                                }
                        }
                    },
                    PartyName = new PartyNameType[]
                    { "K&G Laboratorios"},
                    PostalAddress = new AddressType
                    {
                        ID = "150114",
                        StreetName = "AV. LOS OLIVOS 767",
                        CitySubdivisionName = "URB. SANTA FELICIA",
                        CityName = "LIMA",
                        CountrySubentity = "LIMA",
                        District = "MOLINA",
                        Country = new CountryType
                        {
                            IdentificationCode = "PE"
                        }
                    },
                    PartyLegalEntity = new[]
                    {
                        new PartyLegalEntityType()
                        {
                            RegistrationName = "K&G Asociados S. A."
                        }
                    }
                },
                ReceiverParty = new PartyType
                {
                    PartyIdentification = new[]
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = "6",
                                Value = "20100070970"
                            }
                        }
                    },
                    PartyName = new PartyNameType[]
                    { "K&G Laboratorios"},
                    PostalAddress = new AddressType
                    {
                        ID = "150130",
                        StreetName = "CAL. CALLE MORELLI 181 INT. P-2",
                        CitySubdivisionName = "-",
                        CityName = "LIMA",
                        CountrySubentity = "LIMA",
                        District = "SAN BORJA",
                        Country = new CountryType
                        {
                            IdentificationCode = "PE"
                        }
                    },
                    PartyLegalEntity = new[]
                    {
                        new PartyLegalEntityType()
                        {
                            RegistrationName = "SUPERMERCADOS PERUANOS SOCIEDAD ANONIMA O S.P.S.A"
                        }
                    }
                },
                SUNATPerceptionSystemCode = "01",
                SUNATPerceptionPercent = 2,
                Note = "Se emite con facturas del periodo 12/2015",
                TotalInvoiceAmount = 344.00M,
                SUNATTotalCashed = 17544.00M,
                SUNATPerceptionDocumentReference = new[]
                {
                    new SUNATPerceptionDocumentReferenceType
                    {
                        ID = new IdentifierType
                        {
                            Value = "0001-14",
                            schemeID = "01"
                        },
                        IssueDate = new DateTime(2016,12,23),
                        TotalInvoiceAmount = new AmountType
                        {
                            currencyID = "USD",
                            Value = 14000.00M
                        },
                        Payment = new PaymentType[]
                        {
                            new PaymentType
                            {
                                ID = "1",
                                PaidAmount = new AmountType
                                {
                                    currencyID = "USD",
                                    Value = 5000.00M
                                },
                                PaidDate = new DateTime(2015,12,24)
                            },
                        },
                        SUNATPerceptionInformation = new SUNATPerceptionInformationType
                        {
                            SUNATPerceptionAmount = 324.00M,
                            SUNATPerceptionDate = new DateTime(2015,12,24),
                            SUNATNetTotalCashed = 16524.00M,
                            ExchangeRate = new ExchangeRateType
                            {
                                SourceCurrencyCode = "USD",
                                TargetCurrencyCode = "PEN",
                                CalculationRate = 3.24M,
                                Date = new DateTime(2015,12,24)
                            }
                        }
                    }
                    , new SUNATPerceptionDocumentReferenceType
                    {
                        ID = new IdentifierType()
                        {
                            Value = "E001-457",
                            schemeID = "01"
                        },
                        IssueDate = new DateTime(2016,12,10),
                        TotalInvoiceAmount =1000.00M,
                        Payment = new PaymentType[]
                        {
                            new PaymentType
                            {
                                ID = "1",
                                PaidAmount = 1000.00M,
                                PaidDate = new DateTime(2015,12,24)
                            },
                        },
                        SUNATPerceptionInformation = new SUNATPerceptionInformationType
                        {
                            SUNATPerceptionAmount = 20.00M,
                            SUNATPerceptionDate = new DateTime(2015,12,24),
                            SUNATNetTotalCashed = 1020.00M,
                            ExchangeRate = new ExchangeRateType
                            {
                                SourceCurrencyCode = "PEN",
                                TargetCurrencyCode = "PEN",
                                CalculationRate = 1,
                                Date = new DateTime(2015,12,24)
                            }
                        }
                    }
                }
            };
            var gen = new XmlOtrosCeGenerator(_cert);
            var pathResult = gen.GenerateDocPerception(ret);
            if (gen.LastResult.Success && File.Exists(pathResult))
            {
                Process.Start(pathResult);
                var g = new SunatCeR("20600995805", "MODDATOS", "moddatos");
                var resp = g.SendDocument(pathResult);

                MessageBox.Show(resp.Success
                    ? resp.ApplicationResponse.Descripcion
                    : $"ErrorCode : {resp.Error.Code}\n Message: {resp.Error.Description}");
                SaveFileDialog dialog = new SaveFileDialog { Filter = @"Zip Files (*.zip)|*.zip" };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, resp.ContentZip);
                    Process.Start(dialog.FileName);
                }

            }
            else
            {
                MessageBox.Show(gen.LastResult.Error, @"No se pudo crear XML");
            }
        }

        private void TestGuiaRemision()
        {
            var dlg = new OpenFileDialog
            {
                Filter = @"Xml Files |*.xml"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var firmar = XmlSignatureProvider.SignXmlFileTest(dlg.FileName, _cert);
            Process.Start(firmar.Value);
            var g = new SunatGuiaRemision("20131312955", "MODDATOS", "moddatos");
            if (firmar.Key)
            {
                var resp = g.SendDocument(firmar.Value);

                MessageBox.Show(resp.Success
                    ? resp.ApplicationResponse.Descripcion
                    : $"ErrorCode : {resp.Error.Code}\n Message: {resp.Error.Description}");
                if (!resp.Success) return;
                SaveFileDialog dialog = new SaveFileDialog { Filter = @"Zip Files (*.zip)|*.zip" };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, resp.ContentZip);
                    Process.Start(dialog.FileName);
                }
            }
            else
            {
                MessageBox.Show(@"No se pudo firmar");
            }
        }
        #endregion
    }
}
