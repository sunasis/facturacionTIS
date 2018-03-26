using System;
using System.Diagnostics;
using System.IO;
using FacturacionElectronica.Homologacion;
using FacturacionElectronica.Homologacion.Res;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.HomologacionTests
{
    [TestClass]
    public class SunatManagerTests
    {
        private readonly SunatManager _manager;

        public SunatManagerTests()
        {
            _manager = new SunatManager(new SolConfig
            {
                Ruc = "20600995805",
                Usuario = "MODDATOS",
                Clave = "moddatos",
                Service = ServiceSunatType.Beta
            });
        }

        [TestMethod]
        public void SendDocumentTest()
        {
            var name = "20600995805-01-F001-00005214";
            var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", name + ".xml");
            var content = File.ReadAllBytes(filePath);

            var task = _manager.SendDocument(name, content);
            task.Wait(5000);

            var result = task.Result;

            if (!result.Success)
                Trace.WriteLine(result.Error.Code + " - " + result.Error.Description);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.ApplicationResponse);
            StringAssert.Contains(result.ApplicationResponse.Descripcion, "aceptada");
            Trace.WriteLine(result.ApplicationResponse.Descripcion);
        }

        [TestMethod]
        public void SendDocumentTest_with_Error()
        {
            var name = "20600995805-01-F001-00005214";
            var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", name + ".xml");
            var content = File.ReadAllBytes(filePath);

            var task = _manager.SendDocument("20604595805-01-F001-00005214", content);
            task.Wait(5000);

            var result = task.Result;

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            StringAssert.Contains(result.Error.Code, "Client.1034");

            Trace.WriteLine(result.Error.Code + " - " + result.Error.Description);
        }

        [TestMethod]
        public void SendSummaryTest()
        {
            //TODO: update v2.1
            var name = "20600995805-RC-20171128-01";
            var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", name + ".xml");
            var content = File.ReadAllBytes(filePath);

            var task = _manager.SendSummary(name, content);
            task.Wait(5000);

            var result = task.Result;

            if (!result.Success)
                Trace.WriteLine(result.Error.Code + " - " + result.Error.Description);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Ticket);

            Trace.WriteLine("Ticket: " + result.Ticket);
        }

        [TestMethod]
        public void GetStatusTest()
        {
            var ticket = "1511910611566";
            var task = _manager.GetStatus(ticket);
            task.Wait(5000);

            var result = task.Result;

            if (!result.Success)
            {
                Trace.WriteLine(result.Error.Code + " - " + result.Error.Description);
                return;
            }

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.ApplicationResponse);
            StringAssert.Contains(result.ApplicationResponse.Descripcion, "aceptado");
        }
    }
}