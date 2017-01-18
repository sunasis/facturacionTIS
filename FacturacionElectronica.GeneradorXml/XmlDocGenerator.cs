using System;
using System.Security.Cryptography.X509Certificates;
using Gs.Ubl.v2;
using Gs.Ubl.v2.Cac;
using Gs.Ubl.v2.Ext;
using Gs.Ubl.v2.Udt;
using Gs.Ubl.v2.Sac;

namespace FacturacionElectronica.GeneradorXml
{
    using Entity;
    using Res;
    using System.Collections.Generic;

    /// <summary>
    /// Class para generar documentos XML , firmados cumpliendo el Estandar UBL 2.0 requerido por SUNAT
    /// <remarks>Invoice, Summary, Voided</remarks>
    /// </summary>
    public class XmlDocGenerator
    {
        #region Field
        private readonly X509Certificate2 _certificado;
        #endregion

        #region Construct
        /// <summary>
        /// Crea una Instancia de <c>XmlDocGenerator</c>.
        /// </summary>
        /// <param name="certificado">Certificado X509 v3 con el que se firmara</param>
        public XmlDocGenerator(X509Certificate2 certificado)
        {
            _certificado = certificado;
        }
        #endregion

        #region Method
        /// <summary>
        /// Genera un documento XML para la emision de un comprobante.
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="invoiceHeaderEntity">Entidad Invoice</param>
        /// <returns>Retorna la ruta del archivo XML generado.</returns>
        public string GeneraDocumentoInvoice(ref OperationResult pobjOperationResult, InvoiceHeader invoiceHeaderEntity)
        {
            try
            {
                #region Filename
                var xmlFilename =
                    $"{invoiceHeaderEntity.RucEmisor}-{((int) invoiceHeaderEntity.TipoDocumento).ToString("00")}-" +
                    $"{invoiceHeaderEntity.SerieDocumento}-{invoiceHeaderEntity.CorrelativoDocumento}.xml";
                #endregion

                #region Gen Invoice
                AmountType.TlsDefaultCurrencyID = invoiceHeaderEntity.CodigoMoneda;

                var res = new InvoiceType
                {
                    ID = $"{invoiceHeaderEntity.SerieDocumento}-{invoiceHeaderEntity.CorrelativoDocumento}",
                    IssueDate = invoiceHeaderEntity.FechaEmision,
                    InvoiceTypeCode = ((int)invoiceHeaderEntity.TipoDocumento).ToString("00"),
                    DocumentCurrencyCode = invoiceHeaderEntity.CodigoMoneda,
                    CustomizationID = "1.0",
                    DespatchDocumentReference = UtilsXmlDoc.DevuelveGuiasRemisionReferenciadas(invoiceHeaderEntity.GuiaRemisionReferencia),
                    AdditionalDocumentReference = UtilsXmlDoc.DevuelveDocumentosReferencia(invoiceHeaderEntity),
                    UBLExtensions = new[]
                    {    new UBLExtensionType
                        {
                            ExtensionContent = new AdditionalsInformationType //(mas informacion en el catalogo no 14 del manual.)
                                {
                                    AdditionalInformation = new AdditionalInformationType{
                                        AdditionalMonetaryTotal = UtilsXmlDoc.DevuelveTributosAdicionales(invoiceHeaderEntity.TotalTributosAdicionales),
                                        AdditionalProperty = invoiceHeaderEntity.InfoAddicional.ToArray()
                                    }
                                }
                        },
                        new UBLExtensionType
                        {
                            ExtensionContent = new AdditionalsInformationType()
                        }
                    },
                    Signature = UtilsXmlDoc.GetSignature(invoiceHeaderEntity),
                    AccountingSupplierParty =UtilsXmlDoc.GetInfoEmisor(invoiceHeaderEntity),
                    AccountingCustomerParty = new CustomerPartyType
                    {
                        CustomerAssignedAccountID = invoiceHeaderEntity.NroDocCliente,
                        AdditionalAccountID =
                            new IdentifierType[] { ((int)invoiceHeaderEntity.TipoDocumentoIdentidadCliente).ToString() },
                        Party = new PartyType
                        {
                            PartyLegalEntity = new[]
                            {
                                new PartyLegalEntityType
                                {
                                    RegistrationName = invoiceHeaderEntity.NombreRazonSocialCliente,
                                }
                            }
                        },
                    },
                    LegalMonetaryTotal = new MonetaryTotalType
                    {
                        AllowanceTotalAmount = invoiceHeaderEntity.DescuentoGlobal > 0 ? new AmountType {Value = invoiceHeaderEntity.DescuentoGlobal} : null,
                        PayableAmount = invoiceHeaderEntity.TotalVenta
                    },
                    InvoiceLine = UtilsXmlDoc.DevuelveDetallesDelComprobante(invoiceHeaderEntity.DetallesDocumento),
                    TaxTotal = UtilsXmlDoc.DevuelveSubTotalImpuestos(invoiceHeaderEntity.Impuesto)
                };
                res.AccountingSupplierParty.Party.PostalAddress = UtilsXmlDoc.ObtenerDireccion(invoiceHeaderEntity.DireccionEmisor);
                #endregion

                pobjOperationResult.Success = false;
                return UtilsXmlDoc.GenFile(ref pobjOperationResult, res, xmlFilename, _certificado);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = false;
                pobjOperationResult.Error = ex.Message;
                pobjOperationResult.InnerException = ex.InnerException?.Message ?? string.Empty;
                pobjOperationResult.Target = "XmlGenerator.GeneraDocumentoInvoice()";
            }
            return null;
        }

        /// <summary>
        /// Genera un documento XML para Comunicacion de Baja.
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="voidedHeaderEntity">Entidad Voided</param>
        /// <returns>Retorna la ruta del archivo XML generado.</returns>
        public string GenerarDocumentoVoided(ref OperationResult pobjOperationResult, VoidedHeader voidedHeaderEntity)
        {
            try
            {
                #region Filename
                string id = $"RA-{DateTime.Today.ToString("yyyyMMdd")}-{voidedHeaderEntity.CorrelativoArchivo}";
                string xmlFilename = voidedHeaderEntity.RucEmisor + "-" + id + ".xml";
                #endregion

                #region Gen Voided
                var voidedDoc = new VoidedDocumentsType
                {
                    ID = id,
                    ReferenceDate = voidedHeaderEntity.FechaEmision,
                    CustomizationID = "1.0",
                    IssueDate = DateTime.Today.Date,
                    UBLExtensions = new[]
                    {
                        new UBLExtensionType
                        {
                            ExtensionContent = new AdditionalsInformationType()
                        },
                    },
                    Signature = UtilsXmlDoc.GetSignature(voidedHeaderEntity),
                    AccountingSupplierParty = UtilsXmlDoc.GetInfoEmisor(voidedHeaderEntity),
                    VoidedDocumentsLine = UtilsXmlDoc.GetVoidedLines(voidedHeaderEntity.DetallesDocumento)
                };
                #endregion

                return UtilsXmlDoc.GenFile(ref pobjOperationResult, voidedDoc, xmlFilename, _certificado);
            }
            catch (Exception er)
            {
                pobjOperationResult.Success = false;
                pobjOperationResult.Error = er.Message;
                pobjOperationResult.Target = "XmlDocGenerator.GenerarDocumentoVoided()";
                return null;
            }
        }

        /// <summary>
        /// Genera un documento XML para Resumen Diario.
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="summaryHeaderEntity">Entidad de Resumen</param>
        /// <returns>Retorna la ruta del archivo XML generado.</returns>
        public string GenerarDocumentoSummary(ref OperationResult pobjOperationResult, SummaryHeader summaryHeaderEntity)
        {
            try
            {
                #region Filename
                string id = $"RC-{DateTime.Today.ToString("yyyyMMdd")}-{summaryHeaderEntity.CorrelativoArchivo}";
                string xmlFilename = summaryHeaderEntity.RucEmisor + "-" + id + ".xml";
                #endregion

                #region Gen Summary
                AmountType.TlsDefaultCurrencyID = summaryHeaderEntity.CodigoMoneda;
                var summaryDoc = new SummaryDocumentsType
                {
                    ID = id,
                    CustomizationID = "1.0", // 2017 = 1.1
                    ReferenceDate = summaryHeaderEntity.FechaEmision,
                    IssueDate = DateTime.Today.Date,
                    UBLExtensions = new[]
                    {
                        new UBLExtensionType
                        {
                            ExtensionContent = new AdditionalsInformationType()
                        },
                    },
                    Signature = UtilsXmlDoc.GetSignature(summaryHeaderEntity),
                    AccountingSupplierParty = UtilsXmlDoc.GetInfoEmisor(summaryHeaderEntity),
                    SummaryDocumentsLine = UtilsXmlDoc.GetSummaryLines(summaryHeaderEntity.DetallesDocumento)
                };
                #endregion

                return UtilsXmlDoc.GenFile(ref pobjOperationResult, summaryDoc, xmlFilename, _certificado);
            }
            catch(Exception er)
            {
                pobjOperationResult.Success = false;
                pobjOperationResult.Error = er.Message;
                pobjOperationResult.Target = "XmlDocGenerator.GenerarDocumentoSummary()";
                return null;
            }
        }

        /// <summary>
        /// Genera un documento XML para Notas de Credito.
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="creditHeaderEntity">Entidad de Nota de Credito</param>
        /// <returns>Retorna la ruta del archivo XML generado.</returns>
        public string GenerarDocumentoCreditNote(ref OperationResult pobjOperationResult, CreditNoteHeader creditHeaderEntity)
        {
            try
            {
                #region FileName
                string xmlFilename =
                    $"{creditHeaderEntity.RucEmisor}-07-{creditHeaderEntity.SerieDocumento}-{creditHeaderEntity.CorrelativoDocumento}.xml";
                #endregion

                #region Gen CreditNote
                AmountType.TlsDefaultCurrencyID = creditHeaderEntity.CodigoMoneda;
                var creditDoc = new CreditNoteType
                {
                    ID = string.Concat(creditHeaderEntity.SerieDocumento, "-", creditHeaderEntity.CorrelativoDocumento),
                    IssueDate = creditHeaderEntity.FechaEmision,
                    DocumentCurrencyCode = creditHeaderEntity.CodigoMoneda,
                    CustomizationID = "1.0",
                    DespatchDocumentReference = UtilsXmlDoc.DevuelveGuiasRemisionReferenciadas(creditHeaderEntity.GuiaRemisionReferencia),
                    AdditionalDocumentReference = UtilsXmlDoc.DevuelveDocumentosReferenciaNote(creditHeaderEntity),
                    DiscrepancyResponse = new[]
                    {
                        new ResponseType
                        {
                            ReferenceID = creditHeaderEntity.DocumentoRef,
                            ResponseCode = ((int)creditHeaderEntity.TipoNota).ToString("00"),
                            Description = new TextType[]
                            {
                                creditHeaderEntity.Motivo
                            }
                        }
                    },
                    BillingReference = new[]
                    {
                        new BillingReferenceType
                        {
                            InvoiceDocumentReference = new DocumentReferenceType
                            {
                                ID = creditHeaderEntity.DocumentoRef,
                                DocumentTypeCode = ((int)creditHeaderEntity.TipoDocRef).ToString("00")
                            }
                        }
                    },
                    Signature = UtilsXmlDoc.GetSignature(creditHeaderEntity),
                    AccountingSupplierParty = UtilsXmlDoc.GetInfoEmisor(creditHeaderEntity),
                    AccountingCustomerParty = new CustomerPartyType
                    {
                        CustomerAssignedAccountID = creditHeaderEntity.NroDocCliente,
                        AdditionalAccountID =
                                new IdentifierType[] { ((int)creditHeaderEntity.TipoDocumentoIdentidadCliente).ToString() },
                        Party = new PartyType
                        {
                            PartyLegalEntity = new[]
                                {
                                    new PartyLegalEntityType
                                    {
                                        RegistrationName = creditHeaderEntity.NombreRazonSocialCliente,
                                    }
                                }
                        },
                    },
                    TaxTotal = UtilsXmlDoc.DevuelveSubTotalImpuestos(creditHeaderEntity.Impuesto),
                    LegalMonetaryTotal = new MonetaryTotalType
                    {
                        ChargeTotalAmount = creditHeaderEntity.TotalCargos > 0 ? new AmountType { Value = creditHeaderEntity.TotalCargos } : null,
                        AllowanceTotalAmount = creditHeaderEntity.DescuentoGlobal > 0 ? new AmountType { Value = creditHeaderEntity.DescuentoGlobal } : null,
                        PayableAmount = creditHeaderEntity.Total
                    },
                    CreditNoteLine = UtilsXmlDoc.ToCredit(UtilsXmlDoc.DevuelveDetallesDelComprobante(creditHeaderEntity.DetallesDocumento)),
                };
                #region Ext
                var lisExt = new List<UBLExtensionType>(2);
                if (creditHeaderEntity.TotalTributosAdicionales != null)
                    lisExt.Add(new UBLExtensionType
                    {
                        ExtensionContent = new AdditionalsInformationType //(mas informacion en el catalogo no 14 del manual.)
                        { AdditionalInformation = new AdditionalInformationType { AdditionalMonetaryTotal = UtilsXmlDoc.DevuelveTributosAdicionales(creditHeaderEntity.TotalTributosAdicionales), } }
                    });
                lisExt.Add( new UBLExtensionType {ExtensionContent = new AdditionalsInformationType()
                });
                creditDoc.UBLExtensions = lisExt.ToArray();
                #endregion
                creditDoc.AccountingSupplierParty.Party.PostalAddress = UtilsXmlDoc.ObtenerDireccion(creditHeaderEntity.DireccionEmisor);
                #endregion|

                return UtilsXmlDoc.GenFile(ref pobjOperationResult, creditDoc, xmlFilename, _certificado);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = false;
                pobjOperationResult.Error = ex.Message;
                pobjOperationResult.InnerException = ex.InnerException?.Message ?? string.Empty;
                pobjOperationResult.Target = "XmlGenerator.GenerarDocumentoCreditNote()";
            }
            return null;
        }

        /// <summary>
        /// Genera un documento XML para Notas de Debito.
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="debitHeaderEntity">Entidad de Nota de Debito</param>
        /// <returns>Retorna la ruta del archivo XML generado.</returns>
        public string GenerarDocumentoDebitNote(ref OperationResult pobjOperationResult, DebitNoteHeader debitHeaderEntity)
        {
            try
            {
                #region FileName
                string xmlFilename =
                    $"{debitHeaderEntity.RucEmisor}-08-{debitHeaderEntity.SerieDocumento}-{debitHeaderEntity.CorrelativoDocumento}.xml";
                #endregion

                #region Gen DebitNote
                AmountType.TlsDefaultCurrencyID = debitHeaderEntity.CodigoMoneda;
                var debitDoc = new DebitNoteType
                {
                    ID = string.Concat(debitHeaderEntity.SerieDocumento, "-", debitHeaderEntity.CorrelativoDocumento),
                    IssueDate = debitHeaderEntity.FechaEmision,
                    DocumentCurrencyCode = debitHeaderEntity.CodigoMoneda,
                    CustomizationID = "1.0",
                    DespatchDocumentReference = UtilsXmlDoc.DevuelveGuiasRemisionReferenciadas(debitHeaderEntity.GuiaRemisionReferencia),
                    AdditionalDocumentReference = UtilsXmlDoc.DevuelveDocumentosReferenciaNote(debitHeaderEntity),
                    DiscrepancyResponse = new[]
                    {
                        new ResponseType
                        {
                            ReferenceID = debitHeaderEntity.DocumentoRef,
                            ResponseCode = ((int)debitHeaderEntity.TipoNota).ToString("00"),
                            Description = new TextType[]
                            {
                                debitHeaderEntity.Motivo
                            }
                        }
                    },
                    BillingReference = new[]
                    {
                        new BillingReferenceType
                        {
                            InvoiceDocumentReference = new DocumentReferenceType
                            {
                                ID = debitHeaderEntity.DocumentoRef,
                                DocumentTypeCode = ((int)debitHeaderEntity.TipoDocRef).ToString("00")
                            }
                        }
                    },
                    Signature = UtilsXmlDoc.GetSignature(debitHeaderEntity),
                    AccountingSupplierParty = UtilsXmlDoc.GetInfoEmisor(debitHeaderEntity),
                    AccountingCustomerParty = new CustomerPartyType
                    {
                        CustomerAssignedAccountID = debitHeaderEntity.NroDocCliente,
                        AdditionalAccountID =
                                new IdentifierType[] { ((int)debitHeaderEntity.TipoDocumentoIdentidadCliente).ToString() },
                        Party = new PartyType
                        {
                            PartyLegalEntity = new[]
                                {
                                    new PartyLegalEntityType
                                    {
                                        RegistrationName = debitHeaderEntity.NombreRazonSocialCliente,
                                    }
                                }
                        },
                    },
                    TaxTotal = UtilsXmlDoc.DevuelveSubTotalImpuestos(debitHeaderEntity.Impuesto),
                    RequestedMonetaryTotal = new MonetaryTotalType
                    {
                        ChargeTotalAmount = debitHeaderEntity.TotalCargos > 0 ? new AmountType { Value = debitHeaderEntity.TotalCargos } : null,
                        AllowanceTotalAmount = debitHeaderEntity.DescuentoGlobal > 0 ? new AmountType { Value = debitHeaderEntity.DescuentoGlobal } : null,
                        PayableAmount = debitHeaderEntity.Total
                    },
                    DebitNoteLine = UtilsXmlDoc.ToDebit(UtilsXmlDoc.DevuelveDetallesDelComprobante(debitHeaderEntity.DetallesDocumento)),
                };
                #region Ext
                var lisExt = new List<UBLExtensionType>(2);
                if (debitHeaderEntity.TotalTributosAdicionales != null)
                    lisExt.Add(new UBLExtensionType
                    {
                        ExtensionContent = new AdditionalsInformationType //(mas informacion en el catalogo no 14 del manual.)
                        { AdditionalInformation = new AdditionalInformationType { AdditionalMonetaryTotal = UtilsXmlDoc.DevuelveTributosAdicionales(debitHeaderEntity.TotalTributosAdicionales), } }
                    });
                lisExt.Add(new UBLExtensionType
                {
                    ExtensionContent = new AdditionalsInformationType()
                });
                debitDoc.UBLExtensions = lisExt.ToArray();
                #endregion
                debitDoc.AccountingSupplierParty.Party.PostalAddress = UtilsXmlDoc.ObtenerDireccion(debitHeaderEntity.DireccionEmisor);
                #endregion|

                return UtilsXmlDoc.GenFile(ref pobjOperationResult, debitDoc, xmlFilename, _certificado);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = false;
                pobjOperationResult.Error = ex.Message;
                pobjOperationResult.InnerException = ex.InnerException?.Message ?? string.Empty;
                pobjOperationResult.Target = "XmlGenerator.GenerarDocumentoDebitNote()";
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    /// Generate Other Files - (CPE,CRE,CGR)
    /// </summary>
    public class XmlOtrosCeGenerator
    {
        #region Fields
        private readonly X509Certificate2 _certificado;
        private OperationResult _result;
        #endregion

        #region Properties

        /// <summary>
        /// Contiene Informacion de la Ultima Operacion.
        /// </summary>
        public OperationResult LastResult => _result;

        #endregion

        #region Construct
        /// <summary>
        /// Crea una Instancia de <see cref="XmlOtrosCeGenerator"/>
        /// </summary>
        /// <param name="certificado">Certificado X509 v3 con el que se firmara</param>
        public XmlOtrosCeGenerator(X509Certificate2 certificado)
        {
            _certificado = certificado;
            _result = new OperationResult();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Genera Documento de Retencion.
        /// </summary>
        /// <param name="doc">Retention UBL2.0</param>
        /// <returns>Path of XML</returns>
        public string GenerateDocRetention(RetentionType doc)
        {
            var filename = $"{doc.AgentParty.PartyIdentification[0].ID.Value}-20-{doc.ID.Value}.xml";
            return UtilsXmlDoc.GenFile(ref _result, doc, filename, _certificado);
        }

        /// <summary>
        /// Genera Documento de Perecepcion
        /// </summary>
        /// <param name="doc">Pereception UBL2.0</param>
        /// <returns>Path of XML</returns>
        public string GenerateDocPerception(PerceptionType doc)
        {
            var filename = $"{doc.AgentParty.PartyIdentification[0].ID.Value}-40-{doc.ID.Value}.xml";
            return UtilsXmlDoc.GenFile(ref _result, doc, filename, _certificado);
        }

        /// <summary>
        /// Genera y Firma Documento de Guia de Remision.
        /// </summary>
        /// <param name="doc">Guia de Remision UBL2.1</param>
        /// <returns>Path of XML</returns>
        public string GenerateDocGuia(DespatchAdviceType doc)
        {
            var filename = $"{doc.DespatchSupplierParty.CustomerAssignedAccountID.Value}-09-{doc.ID.Value}.xml";
            return UtilsXmlDoc.GenFile(ref _result, doc, filename, _certificado);
        }

        /// <summary>
        /// Genera un documento XML para Resumen de Reversion.
        /// </summary>
        /// <param name="voidedHeaderEntity">Entidad Voided</param>
        /// <returns>Retorna la ruta del archivo XML generado.</returns>
        public string GenerarDocumentoVoided(VoidedHeader voidedHeaderEntity)
        {
            try
            {
                #region Filename
                string id = $"RR-{DateTime.Today.ToString("yyyyMMdd")}-{voidedHeaderEntity.CorrelativoArchivo}";
                string xmlFilename = voidedHeaderEntity.RucEmisor + "-" + id + ".xml";
                #endregion

                #region Gen Voided
                var voidedDoc = new VoidedDocumentsType
                {
                    ID = id,
                    ReferenceDate = voidedHeaderEntity.FechaEmision,
                    CustomizationID = "1.0",
                    IssueDate = DateTime.Today.Date,
                    UBLExtensions = new[]
                    {
                        new UBLExtensionType
                        {
                            ExtensionContent = new AdditionalsInformationType()
                        },
                    },
                    Signature = UtilsXmlDoc.GetSignature(voidedHeaderEntity),
                    AccountingSupplierParty = UtilsXmlDoc.GetInfoEmisor(voidedHeaderEntity),
                    VoidedDocumentsLine = UtilsXmlDoc.GetVoidedLines(voidedHeaderEntity.DetallesDocumento)
                };
                #endregion

                return UtilsXmlDoc.GenFile(ref _result, voidedDoc, xmlFilename, _certificado);
            }
            catch (Exception er)
            {
                _result.Success = false;
                _result.Error = er.Message;
                _result.Target = "XmlOtrosCeGenerator.GenerarDocumentoVoided()";
                return null;
            }
        }
        #endregion
    }
}
