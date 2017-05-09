using System;

namespace FacturacionElectronica.Homologacion
{
    using Res;

    /// <summary>
    /// <c>SunatGuiaRemision</c> maneja el proceso de envio de Guia de Remision.
    /// </summary>
    public class SunatGuiaRemision : SunatCe
    {
        #region Construct

        /// <summary>
        /// Administrador de WebService de la Sunat. Necesita Clave SOL
        /// </summary>
        /// <param name="config">Configuration</param>
        public SunatGuiaRemision(SolConfig config)
            : base(config)
        {
            BaseUrl = GetUrlService(config.Service);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Establece el Tipo de Servicio que se utilizara para la conexion con el WebService de Sunat.
        /// </summary>
        /// <param name="service">Tipo de Servicio (Validos <see cref="ServiceSunatType.Beta"/> y <see cref="ServiceSunatType.Produccion"/>)</param>
        /// <exception cref="ArgumentException">Servicio Invalido</exception>
        /// <returns>Url of service</returns>
        private static string GetUrlService(ServiceSunatType service)
        {         
            string url;
            switch (service)
            {
                case ServiceSunatType.Beta:
                    url = Resources.UrlGuiaBeta;
                    break;
                case ServiceSunatType.Produccion:
                    url = Resources.UrlGuia;
                    break;
                default:
                    throw new ArgumentException(@"Servicio Invalido, solo se acepta BETA y Produccion", nameof(service));
            }
            return url;
        }
        #endregion
    }
}
