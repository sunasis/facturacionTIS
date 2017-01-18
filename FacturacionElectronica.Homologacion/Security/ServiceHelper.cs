﻿using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FacturacionElectronica.Homologacion.Security
{
    internal class ServiceHelper
    {
        #region Fields & Properties
        private static Binding _objbinding;
        public static string User;
        public static string Password;
        #endregion

        private ServiceHelper() { }

        /// <summary>
        /// Inicializa el Binding por unica vez
        /// </summary>
        private Binding GetBinding()
        {
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
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
            if (_objbinding == null) _objbinding = new ServiceHelper().GetBinding();
            dynamic ws = (TService)Activator.CreateInstance(typeof(TService), _objbinding, new EndpointAddress(url));
            ws.ClientCredentials.UserName.UserName = User;
            ws.ClientCredentials.UserName.Password = Password;
            return ws;
        }
        
    }
}
