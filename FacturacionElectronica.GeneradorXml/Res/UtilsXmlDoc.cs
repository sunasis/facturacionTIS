using System.Collections.Generic;
using System.Linq;

namespace FacturacionElectronica.GeneradorXml.Res
{
    using Entity;
    using Entity.Details;
    using Entity.Misc;
    using System.IO;
    using System.Security.Cryptography;
    using System.Xml;
    using System.Xml.Serialization;
    using Gs.Ubl.v2.Cac;
    using Gs.Ubl.v2.Udt;
    using Gs.Ubl.v2.Sac;

    internal static class UtilsXmlDoc
    {
        public const string RucIdDocument = "6";

        #region General
        /// <summary>
        /// Genera y Firma un Doc XML
        /// </summary>
        /// <typeparam name="TSunat">Ubl entity</typeparam>
        /// <param name="pobjOperationResult">resultado</param>
        /// <param name="objXml">Obj de Entidad UBL</param>
        /// <param name="pstrXmlFilename">Nombre del archivo xml de Destino</param>
        /// <param name="certificado">Certificado X509 v3</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <returns></returns>
        public static string GenFile<TSunat>(ref OperationResult pobjOperationResult,TSunat objXml, string pstrXmlFilename, System.Security.Cryptography.X509Certificates.X509Certificate2 certificado)
            where TSunat : Gs.Ubl.v2.UblBaseDocumentType
        {
            string result = null;

            #region Escritura y Firma del archivo XML Generado.
            var typeToSerialize = typeof(TSunat);
            var serializer = new XmlSerializer(typeToSerialize);
            var memStream = new MemoryStream();
            serializer.Serialize(memStream, objXml);
            memStream.Seek(0, SeekOrigin.Begin);
            var doc = new XmlDocument();
            doc.Load(memStream);
            memStream.Close();
            #region Se firma el Xml creado.
            pstrXmlFilename = Path.Combine(Path.GetTempPath(), pstrXmlFilename);
            try
            {
                XmlSignatureProvider.SignXmlFile(doc, pstrXmlFilename, certificado, typeof(TSunat));
                result = new FileInfo(pstrXmlFilename).FullName;
                pobjOperationResult.Success = true;
                System.Diagnostics.Debug.WriteLine("Archivo XML firmado.");
            }
            catch (CryptographicException e)
            {
                pobjOperationResult.Success = false;
                pobjOperationResult.Error = e.Message;
            }
            #endregion
            #endregion

            return result;
        }

        /// <summary>
        ///  Obtiene el Signature para el Documento
        /// </summary>
        /// <typeparam name="TSunat">Documento SUNAT</typeparam>
        /// <param name="entidad">Entidad Doc</param>
        /// <returns>UBL Signature</returns>
        public static SignatureType[] GetSignature<TSunat>(SunatDocumentBase<TSunat> entidad)
            where TSunat : new()
        {
            var sign = new[]
            {
                new SignatureType
                {
                    ID = "IDSignSP",
                    SignatoryParty = new PartyType
                    {
                        PartyIdentification = new[]
                        {
                            new PartyIdentificationType
                            {
                                ID = entidad.RucEmisor
                            }
                        },
                        PartyName = new[]
                        {
                            new PartyNameType
                            {
                                Name = entidad.NombreRazonSocialEmisor
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
                }
            };
            return sign;
        }

        /// <summary>
        ///  Obtiene la Informacion del Emisor para el Documento
        /// </summary>
        /// <typeparam name="TSunat">Documento SUNAT</typeparam>
        /// <param name="entidad">Entidad Doc</param>
        /// <returns>UBL SupplierPartyType</returns>
        public static SupplierPartyType GetInfoEmisor<TSunat>(SunatDocumentBase<TSunat> entidad)
            where TSunat : new()
        {
            var account = new SupplierPartyType
            {
                CustomerAssignedAccountID = entidad.RucEmisor,
                AdditionalAccountID =
                    new IdentifierType[] { ((int)entidad.TipoDocumentoIdentidadEmisor).ToString() },
                Party = new PartyType
                {
                    PartyName = new PartyNameType[] { entidad.NombreComercialEmisor },
                    PartyLegalEntity = new[]
                    {
                        new PartyLegalEntityType
                        {
                            RegistrationName = entidad.NombreRazonSocialEmisor,
                        }
                    },
                }
            };
            return account;
        }

        /// <summary>
        /// Convierte Invoicelines en CreditLines
        /// </summary>
        /// <param name="lines">Lineas de un Invoice</param>
        /// <returns>Credit Lines</returns>
        public static CreditNoteLineType[] ToCredit(IEnumerable<InvoiceLineType> lines)
        {
            var result = lines.Select(line => new CreditNoteLineType
            {
                ID = line.ID,
                CreditedQuantity = line.InvoicedQuantity,
                LineExtensionAmount = line.LineExtensionAmount,
                PricingReference = line.PricingReference,
                TaxTotal = line.TaxTotal,
                Item = line.Item,
                Price = line.Price,
            });
            return result.ToArray();
        }

        /// <summary>
        /// Convierte Invoicelines en DebitLines
        /// </summary>
        /// <param name="lines">Lineas de un Invoice</param>
        /// <returns>Debit Lines</returns>
        public static DebitNoteLineType[] ToDebit(IEnumerable<InvoiceLineType> lines)
        {
            var result = lines.Select(line => new DebitNoteLineType
            {
                ID = line.ID,
                DebitedQuantity = line.InvoicedQuantity,
                LineExtensionAmount = line.LineExtensionAmount,
                PricingReference = line.PricingReference,
                TaxTotal = line.TaxTotal,
                Item = line.Item,
                Price = line.Price,
            });
            return result.ToArray();
        }
        #endregion

        #region Invoice

        /// <summary>
        /// Devuelve una seccion de xml si la entidad de venta contiene un nro de documento de referencia.
        /// </summary>
        /// <param name="invoiceHeaderEntity"></param>
        /// <returns></returns>
        public static DocumentReferenceType[] DevuelveDocumentosReferencia(InvoiceHeader invoiceHeaderEntity)
        {
            if (invoiceHeaderEntity.DocumentoReferenciaNumero != null)
            {
                var res = new[]
                {
                    new DocumentReferenceType
                    {
                        ID = invoiceHeaderEntity.DocumentoReferenciaNumero,
                        DocumentTypeCode = ((int)invoiceHeaderEntity.DocumentoReferenciaTipoDocumento).ToString("00")
                    }
                };

                return res;
            }
            return null;
        }
        /// <summary>
        /// Devuelve una seccion de xml si la entidad de venta contiene un nro de documento de referencia.
        /// </summary>
        /// <param name="noteHeaderEntity"></param>
        /// <returns></returns>
        public static DocumentReferenceType[] DevuelveDocumentosReferenciaNote(NotasBase<InvoiceDetail> noteHeaderEntity)
        {
            if (noteHeaderEntity.DocumentoReferenciaNumero != null)
            {
                var res = new[]
                {
                    new DocumentReferenceType
                    {
                        ID = noteHeaderEntity.DocumentoReferenciaNumero,
                        DocumentTypeCode = ((int)noteHeaderEntity.DocumentoReferenciaTipoDocumento).ToString("00")
                    }
                };

                return res;
            }
            return null;
        }
        /// <summary>
        /// Devuelve una o varias secciones de xml si la entidad de venta contiene una o varias guias de remision de referencia.
        /// </summary>
        /// <param name="guiasRemision">Lista de Guia de Remision</param>
        /// <returns></returns>
        public static DocumentReferenceType[] DevuelveGuiasRemisionReferenciadas(List<GuiaRemisionType> guiasRemision)
        {
            if (guiasRemision != null && guiasRemision.Any())
            {
                return guiasRemision.Select(guiaRemision => new DocumentReferenceType
                {
                    ID = guiaRemision.NumeroGuiaRemision,
                    DocumentTypeCode = guiaRemision.IdTipoGuiaRemision
                }).ToArray();
            }
            return null;
        }

        /// <summary>
        /// Retorna una lista de secciones del tipo 'InvoiceLineType' que se procesa a partir del detalle de la venta.
        /// </summary>
        /// <param name="invoiceDetails"></param>
        /// <returns></returns>
        public static InvoiceLineType[] DevuelveDetallesDelComprobante(IEnumerable<InvoiceDetail> invoiceDetails)
        {
            var result = new List<InvoiceLineType>();
            var counter = 1;
            foreach (var line in invoiceDetails.Select(detail => new InvoiceLineType
            {
                // ReSharper disable once AccessToModifiedClosure
                ID = counter.ToString(),
                InvoicedQuantity = new QuantityType { unitCode = detail.UnidadMedida, Value = detail.Cantidad },
                LineExtensionAmount = detail.ValorVenta,
                Item = new ItemType
                {
                    Description = new TextType[] { detail.DescripcionProducto },
                    SellersItemIdentification = new ItemIdentificationType { ID = detail.CodigoProducto }
                },
                Price = new PriceType
                {
                    PriceAmount = detail.PrecioUnitario,
                },
                PricingReference = new PricingReferenceType
                {
                    AlternativeConditionPrice = detail.PrecioAlternativos.Select(alt => new PriceType
                    {
                        PriceAmount = alt.Monto,
                        PriceTypeCode = ((int)alt.TipoDePrecio).ToString("00")
                    }).ToArray()
                },
                TaxTotal = DevuelveSubTotalImpuestos(detail.Impuesto)
            }))
            {
                counter++;
                result.Add(line);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Devuelve el total de importes afectos e inafectos segun la lista de Impuestos asignados en la entidad.
        /// </summary>
        /// <param name="totalAdicional"></param>
        /// <returns></returns>
        public static AdditionalMonetaryTotalType[] DevuelveTributosAdicionales(IEnumerable<TotalTributosType> totalAdicional)
        {
            try
            {
                return totalAdicional.Select(tributo => new AdditionalMonetaryTotalType
                {
                    ID = ((int)tributo.Id).ToString(),
                    PayableAmount = tributo.MontoPagable,
                    TotalAmount = tributo.MontoTotal.HasValue ? new AmountType {Value = tributo.MontoTotal.Value} : null,
                    Percent = tributo.Porcentaje.HasValue ? new PercentType {Value = tributo.Porcentaje.Value}: null
                }).ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna una seccion XML del tipo TaxTotal para indicar el impuesto de un detalle de venta o bien del total de la venta.
        /// </summary>
        /// <param name="detaiList"></param>
        /// <returns></returns>
        public static TaxTotalType[] DevuelveSubTotalImpuestos(List<TotalImpuestosType> detaiList)
        {
            if (detaiList == null || !detaiList.Any()) return null;
            return detaiList.Select(totalTributosType => new TaxTotalType
            {
                TaxAmount = totalTributosType.Monto,
                TaxSubtotal = new[]
                {
                    new TaxSubtotalType
                    {
                        TaxAmount = totalTributosType.Monto, TaxCategory = new TaxCategoryType
                        {
                            TierRange = totalTributosType.TipoIsc.HasValue ? ((int)totalTributosType.TipoIsc).ToString("00") : null,
                            TaxExemptionReasonCode = totalTributosType.TipoAfectacion.HasValue ?  ((int)totalTributosType.TipoAfectacion).ToString("00") : null,
                            TaxScheme = new TaxSchemeType
                            {
                                ID = ((int) totalTributosType.TipoTributo).ToString(), 
                                TaxTypeCode = totalTributosType.TipoTributo.ToString().Split('_')[1], 
                                Name = totalTributosType.TipoTributo.ToString().Split('_')[0]
                            }
                        }
                    }
                }
            }).ToArray();
        }

        /// <summary>
        /// Obtiene la direccion a UBL 2.0
        /// </summary>
        /// <param name="direccion">Entidad Direccion</param>
        /// <returns></returns>
        public static AddressType ObtenerDireccion(DireccionType direccion)
        {
            AddressType res = null;
            try
            {
                 if (direccion != null)
                 {
                     res = new AddressType
                     {
                         ID = direccion.CodigoUbigueo,
                         StreetName = direccion.Direccion,
                         Country = new CountryType
                         {
                             IdentificationCode = direccion.CodigoPais
                         }
                     };
                     if (direccion.Zona != null) res.CitySubdivisionName = direccion.Zona;
                    res.CityName = direccion.Departamento;
                    res.CountrySubentity = direccion.Provincia;
                    res.District = direccion.Distrito;
                    return res;
                }
            }
            catch
            {
                // ignored
            }
            return res;
        }

        /// <summary>
        /// Gets the anticipos.
        /// </summary>
        /// <param name="anticipos">The anticipos.</param>
        /// <returns>Gs.Ubl.v2.Cac.PaymentType[].</returns>
        public static PaymentType[] GetAnticipos(List<AnticipoType> anticipos)
        {
            if (anticipos == null || anticipos.Count == 0)
            {
                return null;
            }

            var elements = anticipos.Select(item => new PaymentType
            {
                ID = new IdentifierType
                {
                    schemeID = ((int)item.TipoDocRel).ToString("00"),
                    Value = item.NroDocumentRel
                },
                PaidAmount = item.MontoAnticipo,
                InstructionID = new IdentifierType
                {
                    schemeID = RucIdDocument,
                    Value = item.RucEmisorDoc
                }
            });

            return elements.ToArray();
        }
        #endregion

        #region Voided
        /// <summary>
        /// Devuelve los Items para el Voided Document
        /// </summary>
        /// <param name="voidedDetails">items detail voided</param>
        /// <returns></returns>
        public static VoidedDocumentsLineType[] GetVoidedLines(IEnumerable<VoidedDetail> voidedDetails)
        {
            var result = new List<VoidedDocumentsLineType>();
            int counter = 1;
            foreach (var item in voidedDetails)
            {
                var line = new VoidedDocumentsLineType
                {
                    LineID = counter.ToString(),
                    DocumentTypeCode = ((int)item.TipoDocumento).ToString("00"),
                    DocumentSerialID = item.SerieDocumento,
                    DocumentNumberID = item.CorrelativoDocumento,
                    VoidReasonDescription = item.Motivo
                };
                result.Add(line);
                counter++;
            }
            return result.ToArray();
        }
        #endregion

        #region Summary

        /// <summary>
        /// Genera las lineas de un documento de Resumen Diario
        /// </summary>
        /// <param name="summaryDetails">Entidad Detalles de Resumen</param>
        /// <param name="version2"></param>
        /// <returns>Entidad en Estandar UBL2.0</returns>
        public static SummaryDocumentsLineType[] GetSummaryLines(IEnumerable<SummaryDetail> summaryDetails, bool version2 = false)
        {
            var result = new List<SummaryDocumentsLineType>();
            var counter = 1;
            foreach (var item in summaryDetails)
            {
                var line = new SummaryDocumentsLineType
                {
                    LineID = counter.ToString(),
                    DocumentTypeCode = ((int)item.TipoDocumento).ToString("00"),
                    TotalAmount = item.Total,
                    BillingPayment = item.Importe.Select(i => new PaymentType
                    {
                        PaidAmount = i.Monto,
                        InstructionID = ((int)i.TipoImporte).ToString("00")
                    }).ToArray(),
                    AllowanceCharge = item.OtroImporte.Select(i => new AllowanceChargeType
                    {
                        ChargeIndicator = i.Indicador,
                        Amount = i.Monto
                    }).ToArray(),
                    TaxTotal = item.Impuesto.Select(i => new TaxTotalType
                    {
                        TaxAmount = i.Monto,
                        TaxSubtotal = new[]
                        {
                            new TaxSubtotalType
                            {
                                TaxAmount = i.Monto, TaxCategory = new TaxCategoryType
                                {
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = ((int) i.TipoTributo).ToString(),
                                        TaxTypeCode = i.TipoTributo.ToString().Split('_')[1],
                                        Name = i.TipoTributo.ToString().Split('_')[0]
                                    }
                                }
                            }
                        }
                    }).ToArray()
                };
                if (version2)
                {
                    line.ID = item.Documento;
                    line.AccountingCustomerParty = new CustomerPartyType
                    {
                        CustomerAssignedAccountID = item.NroDocCliente,
                        AdditionalAccountID = new IdentifierType[]
                        {
                            ((int) item.TipoDocumentoIdentidadCliente).ToString()
                        }
                    };

                    if (item.Percepcion != null)
                    {
                        var perc = item.Percepcion;
                        line.SUNATPerceptionSummaryDocumentReference = new SUNATPerceptionSummaryDocumentReferenceType
                        {
                            SUNATPerceptionSystemCode = ((int)perc.CodRegimen).ToString("00"),
                            SUNATPerceptionPercent = perc.Tasa,
                            TotalInvoiceAmount = perc.Monto,
                            SUNATTotalCashed = perc.MontoTotal,
                            TaxableAmount = perc.MontoBase,
                        };
                    }

                    line.Status = new StatusType
                    {
                        ConditionCode = ((int) item.Estado).ToString()
                    };

                    if (item.Referencia != null && item.Referencia.Count > 0)
                    {
                        var docRef = item.Referencia[0];
                        line.BillingReference = new[] {new BillingReferenceType
                        {
                            InvoiceDocumentReference = new DocumentReferenceType
                            {
                                ID = docRef.Documento,
                                DocumentTypeCode = ((int)docRef.TipoDocumento).ToString("00")
                            }
                        } };
                    }
                }
                else
                {
                    line.DocumentSerialID = item.SerieDocumento;
                    line.StartDocumentNumberID = item.NroCorrelativoInicial;
                    line.EndDocumentNumberID = item.NroCorrelativoFinal;
                }

                result.Add(line);
                counter++;
            }

            return result.ToArray();
        }
        #endregion

    }
}
