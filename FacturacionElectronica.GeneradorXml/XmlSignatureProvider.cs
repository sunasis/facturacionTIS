//
// This example signs an XML file using an
// envelope signature. It then verifies the 
// signed XML.
//
// You must have a certificate with a subject name
// of "CN=XMLDSIG_Test" in the "My" certificate store. 
//
// Run the following command to create a certificate
// and place it in the store.
// makecert -r -pe -n "CN=XMLDSIG_Test" -b 01/01/2005 -e 01/01/2010 -sky signing -ss my

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace FacturacionElectronica.GeneradorXml
{
    /// <summary>
    /// Class para Firmar XML
    /// </summary>
    public static class XmlSignatureProvider
    {
        /// <summary>
        /// Firma el Xml y lo guarda como un nuevo archvo xml.
        /// </summary>
        /// <param name="doc">Documento XML object</param>
        /// <param name="cert">Certificado X509</param>
        public static void SignXmlFile(XmlDocument doc, X509Certificate2 cert)
        {
            doc.PreserveWhitespace = true;
            var signedXml = new SignedXml(doc) {SigningKey = cert.PrivateKey};
            var reference = new Reference {Uri = ""};

            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            var keyInfo = new KeyInfo();
            var x509KeyInfo = new KeyInfoX509Data(cert);
            //x509KeyInfo.AddSubjectName(cert.SubjectName.Name);
            keyInfo.AddClause(x509KeyInfo);
            signedXml.KeyInfo = keyInfo;
            signedXml.Signature.Id = "SignatureSP";
            signedXml.ComputeSignature();

            var nameSpace = new XmlNamespaceManager(doc.NameTable);
            nameSpace.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            var signNodes = doc.SelectNodes("//ext:ExtensionContent", nameSpace);
            var xmlDigitalSignature = signedXml.GetXml();
            xmlDigitalSignature.Prefix = "ds";

            if (signNodes != null && signNodes.Count > 0)
                signNodes[signNodes.Count - 1].AppendChild(doc.ImportNode(xmlDigitalSignature, true)); // Firma y agrega al doc XML
            
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "ISO-8859-1", "no");
            doc.ReplaceChild(xmlDeclaration, doc.FirstChild);
        }

#if DEBUG
        /// <summary>
        /// Verifica que el archivo ZML este correctamente firmado
        /// </summary>
        /// <param name="fileName">Path of XML File</param>
        /// <param name="certificado">Certificado Digital</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static bool VerifyXmlFile(string fileName, X509Certificate2 certificado)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            var signedXml = new SignedXml(xmlDocument);
            var nodeList = xmlDocument.GetElementsByTagName("ds:Signature");
            signedXml.LoadXml((XmlElement)nodeList[0]);
            return signedXml.CheckSignature(certificado, true);
        }

        /*
        public static KeyValuePair<bool, string> SignXmlFile(string signedFileName, X509Certificate2 cert, Type typedoc)
        {
            var doc = new XmlDocument();
            doc.Load(signedFileName);
            Debug.Assert(doc.DocumentElement != null, "doc.DocumentElement != null");
            doc.DocumentElement.SetAttribute("xmlns:ds", SignedXml.XmlDsigNamespaceUrl);
            var signedXml = new SignedXml(doc) { SigningKey = cert.PrivateKey };
            var reference = new Reference { Uri = "" };

            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            var keyInfo = new KeyInfo();
            var x509KeyInfo = new KeyInfoX509Data(cert, X509IncludeOption.WholeChain);

            keyInfo.AddClause(x509KeyInfo);
            signedXml.KeyInfo = keyInfo;
            signedXml.Signature.Id = "SignatureSP";
            signedXml.SignatureFormatValidator = g => true;
            signedXml.ComputeSignature();
            var xmlDigitalSignature = signedXml.GetXml();
            xmlDigitalSignature.Prefix = "ds";

            var nombreSpace = (XmlRootAttribute)Attribute.GetCustomAttribute(typedoc, typeof(XmlRootAttribute));
            var nameSpace = new XmlNamespaceManager(doc.NameTable);
            nameSpace.AddNamespace("tns", nombreSpace.Namespace);
            nameSpace.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            var signNodes = doc.SelectNodes("/tns:" + nombreSpace.ElementName + "/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent", nameSpace);

            if (signNodes != null && signNodes.Count > 0)
                signNodes[signNodes.Count - 1].AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            doc.ReplaceChild(doc.CreateXmlDeclaration("1.0", "ISO-8859-1", "no"), doc.FirstChild);
            var pathResult = System.IO.Path.GetTempPath() + "/" + System.IO.Path.GetFileName(signedFileName);
            using (var xmltw = new XmlTextWriter(pathResult, Encoding.GetEncoding("iso-8859-1")))
            {
                doc.WriteTo(xmltw);
                xmltw.Close();
            }
            return new KeyValuePair<bool, string>(VerifyXmlFile(pathResult, cert), pathResult);
        }

        private static void SetPrefix(String prefix, XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
                SetPrefix(prefix, n);
            node.Prefix = prefix;
        }
         */

        public static KeyValuePair<bool, string> SignXmlFileTest(string signedFileName, X509Certificate2 cert)
        {
            var doc = new XmlDocument();
            doc.Load(signedFileName);
            doc.DocumentElement.SetAttribute("xmlns:ds", SignedXml.XmlDsigNamespaceUrl);
            var signedXml = new SignedXml(doc) { SigningKey = cert.PrivateKey };
            var reference = new Reference { Uri = "" };

            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);
            var keyInfo = new KeyInfo();
            var x509KeyInfo = new KeyInfoX509Data(cert, X509IncludeOption.WholeChain);

            keyInfo.AddClause(x509KeyInfo);
            signedXml.KeyInfo = keyInfo;
            signedXml.Signature.Id = "SignatureSP";
            signedXml.ComputeSignature();
            var xmlDigitalSignature = signedXml.GetXml();
            xmlDigitalSignature.Prefix = "ds";
            var nameSpace = new XmlNamespaceManager(doc.NameTable);
            nameSpace.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1"); // Namespace del Xml (Cambiar)
            nameSpace.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            var signNodes = doc.SelectNodes("/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent", nameSpace); // PathExtions del Xml (Cambiar)

            if (signNodes != null && signNodes.Count > 0)
                signNodes[signNodes.Count - 1].AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            doc.ReplaceChild(doc.CreateXmlDeclaration("1.0", "ISO-8859-1", "no"), doc.FirstChild);
            var pathResult = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(signedFileName));
            using (var xmltw = new XmlTextWriter(pathResult, Encoding.GetEncoding("iso-8859-1")))
            {
                doc.WriteTo(xmltw);
                xmltw.Close();
            }
            return new KeyValuePair<bool, string>(VerifyXmlFile(pathResult, cert), pathResult);
        }
#endif
    }
}