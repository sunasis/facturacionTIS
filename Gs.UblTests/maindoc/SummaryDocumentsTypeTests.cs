using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Gs.Ubl.v2;
using Gs.Ubl.v2.Cac;
using Gs.Ubl.v2.Ext;
using Gs.Ubl.v2.Sac;
using Gs.Ubl.v2.Udt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gs.UblTests.maindoc
{
    [TestClass]
    public class SummaryDocumentsTypeTests
    {
        [TestMethod]
        public void GenerateTest()
        {
            string xmlFilename = @"summary-v2.1.xml";

            var invoice = PopulateInvoiceWithSampleData();

            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(xmlFilename, setting))
            {
                Type typeToSerialize = typeof(SummaryDocumentsType);
                XmlSerializer xs = new XmlSerializer(typeToSerialize);
                xs.Serialize(writer, invoice);
            }

            Trace.WriteLine("Invoice written to:\n{0}", new FileInfo(xmlFilename).FullName);
        }


        private static SummaryDocumentsType PopulateInvoiceWithSampleData()
        {
            // Default that shpould be set when you load the library. Don't need to set it for each document.
            UblBaseDocumentType.GlbCustomizationID =
                "urn:oasis:names:specification:ubl:xpath:Invoice-2.0:sbs-1.0-draft";
            UblBaseDocumentType.GlbProfileID =
                "bpid:urn:oasis:names:draft:bpss:ubl-2-sbs-invoice-notification-draft";

            // Default value assinged to all amounts in this thread
            AmountType.TlsDefaultCurrencyID = "PEN";


            // This initialization will only work with C# 3.0 and above
            var res = new SummaryDocumentsType
            {
                // UBLVersionID = "2.0", Don't need to set this one. hardcoded in the library
                ID = "A00095678",
                IssueDate = new DateTime(2005, 6, 21),
                Note = new TextType[] { "sample" },
                UBLExtensions = new[]
                    {
                        new UBLExtensionType
                        {
                            
                        }
                    },
                AccountingSupplierParty = new SupplierPartyType
                {
                    CustomerAssignedAccountID = "CO001",
                    Party = new PartyType
                    {
                        PartyName = new PartyNameType[] { "Consortial" },
                        PostalAddress = new AddressType
                        {
                            StreetName = "Lima jejejeje",
                            BuildingName = "Thereabouts",
                            BuildingNumber = "56A",
                            CityName = "Farthing",
                            PostalZone = "AA99 1BB",
                            CountrySubentity = "Heremouthshire",
                            AddressLine = new AddressLineType[] { "The Roundabout" },
                            Country = new CountryType { IdentificationCode = "GB" }
                        },
                        PartyTaxScheme = new PartyTaxSchemeType[]
                        {
                            new PartyTaxSchemeType
                            {
                                RegistrationName = "Eduardo Quiroz Cosme",
                                CompanyID = "45810953",
                                ExemptionReason = "N/A",
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = "VAT",
                                    TaxTypeCode = "VAT"
                                }
                            }
                        },
                        Contact = new ContactType
                        {
                            Name = "Mrs Bouquet",
                            Telephone = "0158 1233714",
                            Telefax = "0158 1233856",
                            ElectronicMail = "bouquet@fpconsortial.co.uk",
                        }
                    }
                },
                SummaryDocumentsLine = new []
                {
                    new SummaryDocumentsLineType
                    {
                        LineID = "1",
                        DocumentTypeCode = "03",
                        ID = "B001-1",
                        SUNATPerceptionSummaryDocumentReference = new SUNATPerceptionSummaryDocumentReferenceType
                        {
                            SUNATPerceptionSystemCode = "01",
                            SUNATPerceptionPercent = 2.00M,
                            TotalInvoiceAmount  = 10.00M,
                            SUNATTotalCashed = 110.00M,
                            TaxableAmount = 100.00M
                        },
                        Status = new StatusType
                        {
                            ConditionCode = "1"
                        },
                        BillingPayment = new []
                        {
                            new PaymentType
                            {
                                PaidAmount = 100,
                                InstructionID = "1001"
                            }
                        },
                        TaxTotal = new TaxTotalType[]
                        {
                            new TaxTotalType
                            {
                                TaxAmount = 17.50M,
                                TaxEvidenceIndicator = true,
                                TaxSubtotal = new TaxSubtotalType[]
                                {
                                    new TaxSubtotalType
                                    {
                                        TaxableAmount = 100.00M,
                                        TaxAmount = 17.50M,
                                        TaxCategory = new TaxCategoryType
                                        {
                                            ID = "A",
                                            Percent = 17.5M,
                                            TaxScheme = new TaxSchemeType { ID = "UK VAT", TaxTypeCode = "VAT"}
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return res;
        }
    }
}