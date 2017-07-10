using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using FacturacionElectronica.Homologacion.Res;


namespace FacturacionElectronica.Homologacion.Security
{
    internal class ServiceHelper
    {
        #region Fields & Properties
        private static Binding _objbinding;
        #endregion

        private ServiceHelper() { }

        /// <summary>
        /// Inicializa el Binding por unica vez
        /// </summary>
        private Binding GetBinding()
        {
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls; // Activar por tls sino funciona con ssl3

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            var elements = binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            return new CustomBinding(elements);
        }

        /// <summary>
        /// Crea una Instancia de Conexion a WebService.
        /// </summary>
        /// <typeparam name="TService">Type del WebService</typeparam>
        /// <param name="config">configuration</param>
        /// <param name="url">url del servicio</param>
        /// <returns>Instancia de conexion</returns>
        public static TService GetService<TService>(SolConfig config, string url)
        {
            if (_objbinding == null) _objbinding = new ServiceHelper().GetBinding();

            var channel = new ChannelFactory<TService>(_objbinding);

            var cred = channel.Endpoint
                .Behaviors.Find<ClientCredentials>();
            cred.UserName.UserName = config.Ruc + config.Usuario;
            cred.UserName.Password = config.Clave;
            return channel.CreateChannel(new EndpointAddress(url));
        }
    }
}
