using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FacturacionElectronica.Homologacion.Security
{
    internal static class ServiceHelper
    {
        #region Fields & Properties
        public static string User;
        public static string Password;
        #endregion

        /// <summary>
        /// Inicializa el Binding por unica vez
        /// </summary>
        private static Binding GetBinding()
        {
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls; // Activar por tls sino funciona con ssl3

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            var elements = binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            return new CustomBinding(elements);
        }

        /// <summary>
        /// Crea una Instancia de Conexion a WebService.
        /// </summary>
        /// <typeparam name="TService">Type del WebService</typeparam>
        /// <param name="url">Url del WebService</param>
        /// <returns>Instancia de conexion</returns>
        public static TService GetService<TService>(string url)
        {
            var objbinding = GetBinding();

            dynamic ws = (TService)Activator.CreateInstance(typeof(TService), objbinding, new EndpointAddress(url));
            ws.ClientCredentials.UserName.UserName = User;
            ws.ClientCredentials.UserName.Password = Password;
            return ws;
        }
        
    }
}
