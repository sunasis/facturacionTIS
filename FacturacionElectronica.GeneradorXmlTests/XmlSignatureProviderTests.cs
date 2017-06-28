using System;
using System.IO;
using FacturacionElectronica.GeneradorXml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.GeneradorXmlTests
{
    [TestClass]
    public class XmlSignatureProviderTests
    {
        [TestMethod]
        public void VerifyXmlFileTest()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Resources",
                "20552256647-01-FF12-242.xml");

            var result = XmlSignatureProvider.VerifyXmlFile(path);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VeifyXmlFile_NotValid_Test()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Resources",
                "20600995805-03-B001-1.xml");

            var result = XmlSignatureProvider.VerifyXmlFile(path);
            Assert.IsFalse(result);
        }
    }
}