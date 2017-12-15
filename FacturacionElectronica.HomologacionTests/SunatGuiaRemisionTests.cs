using System;
using System.Diagnostics;
using System.IO;
using FacturacionElectronica.Homologacion;
using FacturacionElectronica.Homologacion.Res;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FacturacionElectronica.HomologacionTests
{
    [TestClass]
    public class SunatGuiaRemisionTests
    {
        private readonly SunatGuiaRemision _sender;

        public SunatGuiaRemisionTests()
        {
            _sender = new SunatGuiaRemision(new SolConfig
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
            var name = "20600995805-09-T001-00000001";
            var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", name + ".xml");
            var content = File.ReadAllBytes(filePath);

            var task = _sender.SendDocument(name, content);
            task.Wait(5000);

            var result = task.Result;

            if (!result.Success)
                Trace.WriteLine(result.Error.Code + " - " + result.Error.Description);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.ApplicationResponse);
            Trace.WriteLine(result.ApplicationResponse.Descripcion);
            StringAssert.Contains(result.ApplicationResponse.Descripcion, "aceptado");
        }
    }
}