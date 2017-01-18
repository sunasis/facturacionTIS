/*
 * Url de info: http://orientacion.sunat.gob.pe/index.php/empresas-menu/comprobantes-de-pago-empresas/comprobantes-de-pago-electronicos-empresas/see-desde-los-sistemas-del-contribuyente/2-comprobantes-que-se-pueden-emitir-desde-see-sistemas-del-contribuyente/factura-electronica-desde-see-del-contribuyente/3544-servicio-web-de-consultas
 * Method : getStatusCdr();
 */
using System;
using System.IO;
using System.ServiceModel;
using FacturacionElectronica.Homologacion.Res;
using FacturacionElectronica.Homologacion.Security;

namespace FacturacionElectronica.Homologacion
{
    /// <summary>
    /// Controlador para comunicaicon con WebServices de SUNAT
    /// </summary>
    public class SunatManager
    {
        #region Properties
        /// <summary>
        /// Obtiene o estable el Tipo de WebService Actual para la conexion con SUNAT.
        /// </summary>
        public static ServiceSunatType CurrentService
        {
            get { return Settings.Default.ServiceCurrent; }
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
        public SunatManager(string ruc, string user, string clave)
        {
            ServiceHelper.User = string.Concat(ruc, user);
            ServiceHelper.Password = clave;
        }
        #endregion

        #region Method Sunat
        /// <summary>
        /// Recibe la ruta XML con un único formato digital y devuelve la Constancia de Recepción – SUNAT. 
        /// </summary>
        /// <param name="pathFileXml">Ruta del Archivo XML</param>
        /// <returns>La respuesta contenida en el XML de Respuesta de la Sunat, si existe</returns>
        public SunatResponse SendDocument(string pathFileXml)
        {
            var nameOfFileZip = Path.GetFileNameWithoutExtension(pathFileXml) + Resources.ExtensionFile;

            var response = new SunatResponse()
            {
                Success = false
            };
            try
            {
                var zipBytes = ProcessZip.CompressFile(pathFileXml);
                using (var service = ServiceHelper.GetService<ClientService.billServiceClient>(Settings.Default.UrlServiceSunat))
                {
                    var resultBytes = service.sendBill(nameOfFileZip, zipBytes);
                    var outputXml = ProcessZip.ExtractFile(resultBytes, Path.GetTempPath());
                    response = new SunatResponse
                    {
                        Success = true,
                        ApplicationResponse = ProcessXml.GetAppResponse(outputXml),
                        ContentZip = resultBytes
                    }; 
                }
            }
            catch (FaultException ex)
            {
                response.Error = new ErrorResponse
                {
                    Code = ex.Code.Name,
                    Description = ProcessXml.GetDescriptionError(ex.Code.Name)
                };
            }
            catch (Exception er)
            {
                response.Error = new ErrorResponse
                {
                    Description = er.Message
                };
            }
            
            return response;
        }
        /// <summary>
        /// Envia una Resumen de Boletas o Comunicaciones de Baja a Sunat
        /// </summary>
        /// <param name="pathFileXml">Ruta del archivo XML que contiene el resumen</param>
        /// <returns>Retorna un estado booleano que indica si no hubo errores, con un string que contiene el Nro Ticket,
        /// con el que posteriormente, utilizando el método getStatus se puede obtener Constancia de Recepcióno</returns>
        public TicketResponse SendSummary(string pathFileXml)
        {
            var nameOfFileZip = Path.GetFileNameWithoutExtension(pathFileXml) + Resources.ExtensionFile;
            var res = new TicketResponse();
            try
            {
                var zipBytes = ProcessZip.CompressFile(pathFileXml);
                using (var service = ServiceHelper.GetService<ClientService.billServiceClient>(Settings.Default.UrlServiceSunat))
                {                
                    res.Ticket = service.sendSummary(nameOfFileZip, zipBytes);
                    res.Success = true;
                }
            }
            catch (FaultException ex)
            {
                res.Error = new ErrorResponse
                {
                    Code = ex.Code.Name,
                    Description =  ProcessXml.GetDescriptionError(ex.Code.Name)
                };
            }
            catch (Exception er){
                res.Error = new ErrorResponse
                {
                    Description = er.Message
                };
            }
            return res;
        }
        /// <summary>
        /// Devuelve un objeto que indica el estado del proceso y en caso de haber terminado, devuelve adjunta la ruta del XML que contiene la Constancia de Recepción
        /// </summary>
        /// <param name="pstrTicket">Ticket proporcionado por la sunat</param>
        /// <returns>Estado del Ticket, y la ruta de la respuesta si existe</returns>
        public SunatResponse GetStatus(string pstrTicket)
        {
            var res = new SunatResponse();
            try
            {
                using (var service = ServiceHelper.GetService<ClientService.billServiceClient>(Settings.Default.UrlServiceSunat))
                {
                    var response = service.getStatus(pstrTicket);
                    switch (response.statusCode)
                    {
                        case "0":
                        case "99":
                            res.Success = true;
                            var pathXml = ProcessZip.ExtractFile(response.content, Path.GetTempPath());
                            res.ApplicationResponse = ProcessXml.GetAppResponse(pathXml);
                            res.ContentZip = response.content;
                            break;
                        case "98":
                            res.Success = false;
                            res.Error = new ErrorResponse { Description = "En Proceso..."};
                            break;
                    }
                }
            }
            catch (FaultException ex)
            {
                res.Error = new ErrorResponse
                {
                    Code = ex.Code.Name,
                    Description = ProcessXml.GetDescriptionError(ex.Code.Name),
                };
            }
            catch (Exception er)
            {
                res.Error = new ErrorResponse
                {
                    Description = er.Message,
                };
            }
            return res;
        }
        /// <summary>
        ///  Obtiene el estado de un Comprobante
        /// </summary>
        /// <param name="ruc">Es el ruc del emisor del comprobante de pago a consultar</param>
        /// <param name="comprobante">Un Comprobante a consultar</param>
        /// <returns></returns>
        public StatusCompResponse GetStatusCdr(string ruc, ComprobanteEletronico comprobante)
        {
            var res = new StatusCompResponse();
            try
            {
                using (var service = ServiceHelper.GetService<ClientServiceConsult.billServiceClient>(Resources.UrlServiceConsult))
                {
                    var response = service.getStatusCdr(ruc, comprobante.Tipo, comprobante.Serie, comprobante.Numero);
                    res.Success = true;
                    var pathXml = ProcessZip.ExtractFile(response.content, Path.GetTempPath());
                    res.ApplicationResponse = ProcessXml.GetAppResponse(pathXml);
                    res.Code = response.statusCode;
                    res.Message = response.statusMessage;
                    res.ContentZip = response.content;
                }
            }
            catch (FaultException ex)
            {
                res.Error = new ErrorResponse
                {
                    Code = ex.Code.Name,
                    Description = ProcessXml.GetDescriptionError(ex.Code.Name)
                };
            }
            catch (Exception er)
            {
                res.Error = new ErrorResponse
                {
                    Code = er.Message,
                };
            }
            return res;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Establece el Tipo de Servicio que se utilizara para la conexion con el WebService de Sunat.
        /// </summary>
        /// <param name="service">Tipo de Servicio al que se conectara</param>
        private static void SetWebService(ServiceSunatType service)
        {
            if (service == Settings.Default.ServiceCurrent) return;

            string url;
            switch (service)
            {
                case ServiceSunatType.Produccion:
                    url = Resources.UrlProduccion;
                    break;
                case ServiceSunatType.Homologacion:
                    url = Resources.UrlHomologacion;
                    break;
                default:
                    url = Resources.UrlBeta;
                    break;
            }
            Settings.Default.UrlServiceSunat = url;
            Settings.Default.ServiceCurrent = service;
            Settings.Default.Save();
        }
        #endregion
    }
}
