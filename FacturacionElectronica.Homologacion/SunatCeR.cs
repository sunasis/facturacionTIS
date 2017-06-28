using System;
using FacturacionElectronica.Homologacion.Properties;

namespace FacturacionElectronica.Homologacion
{
    using Res;
    /// <summary>
    /// Manage Service Comprobante de Retencion y Percepcion Electronico.
    /// </summary>
    public class SunatCeR : SunatCe
    {
        #region Construct

        /// <summary>
        /// Administrador de WebService de la Sunat. Necesita Clave SOL
        /// </summary>
        /// <param name="config"></param>
        public SunatCeR(SolConfig config)
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
        /// <returns>url of service</returns>
        private static string GetUrlService(ServiceSunatType service)
        {
            string url;
            switch (service)
            {
                case ServiceSunatType.Beta:
                    url = Resources.UrlRetPercBeta;
                    break;
                case ServiceSunatType.Produccion:
                    url = Resources.UrlRetPerc;
                    break;
                default:
                    throw new ArgumentException(@"Servicio Invalido, solo se acepta BETA y Produccion", nameof(service));
            }
           return url;
        }

        #endregion
    }
}
