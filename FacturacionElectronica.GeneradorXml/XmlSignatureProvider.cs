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

using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
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

        public static bool VerifyXmlFile(string fileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            var signedXml = new SignedXml(xmlDocument);
            var nodeList = xmlDocument.GetElementsByTagName("ds:Signature");
            var certNode = xmlDocument.GetElementsByTagName("ds:X509Certificate");
            if (certNode.Count == 0)
                certNode = xmlDocument.GetElementsByTagName("X509Certificate");

            try
            {
                var cert = new X509Certificate2(Convert.FromBase64String(certNode[0].InnerText));
                signedXml.LoadXml((XmlElement)nodeList[0]);

                return signedXml.CheckSignature(cert, true);
            }
            catch
            {
                return false;
            }
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

    }
}