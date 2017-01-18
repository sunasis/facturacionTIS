using System;

namespace FacturacionElectronica.Homologacion
{
    using Res;
    /// <summary>
    /// Manage Service Comprobante de Retencion y Percepcion Electronico.
    /// </summary>
    public class SunatCeR : SunatCe
    {
        #region Properties
        /// <summary>
        /// Obtiene o estable el Tipo de WebService Actual para la conexion con SUNAT.
        /// </summary>
        public static ServiceSunatType CurrentService
        {
            get { return Settings.Default.ServiceRetPer; }
            set
            {
                SetWebService(value);
            }
        }
        #endregion

        #region Construct
        /// <summary>
        /// Administrador de WebService de la Sunat. Necesita Clave SOL
        /// </summary>
        /// <param name="ruc">Ruc del emisor</param>
        /// <param name="user">Nombre de Usuario en la Sunat</param>
        /// <param name="clave">Clave SOL</param>
        public SunatCeR(string ruc, string user, string clave)
            : base(ruc, user, clave)
        {
            BaseUrl = Settings.Default.UrlRetPerc;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Establece el Tipo de Servicio que se utilizara para la conexion con el WebService de Sunat.
        /// </summary>
        /// <param name="service">Tipo de Servicio (Validos <see cref="ServiceSunatType.Beta"/> y <see cref="ServiceSunatType.Produccion"/>)</param>
        /// <exception cref="ArgumentException">Servicio Invalido</exception>
        private static void SetWebService(ServiceSunatType service)
        {
            if (service == Settings.Default.ServiceRetPer) return;

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
            Settings.Default.UrlRetPerc = url;
            Settings.Default.ServiceRetPer = service;
            Settings.Default.Save();
        }

        #endregion
    }
}
