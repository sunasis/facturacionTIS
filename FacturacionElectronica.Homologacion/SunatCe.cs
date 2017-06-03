using System;
using System.ServiceModel;
using FacturacionElectronica.Homologacion.Properties;

namespace FacturacionElectronica.Homologacion
{
    using GuiaRemisionService;
    using Res;
    using Security;

    /// <summary>
    /// Class Managed WebService Sunat(Others Documents)
    /// </summary>
    public class SunatCe
    {
        #region Fields
        private readonly SolConfig _config;
        #endregion

        #region Properties
        internal string BaseUrl { get; set; }
        #endregion

        #region Construct
        /// <summary>
        /// Administrador de WebService de la Sunat. Necesita Clave SOL
        /// </summary>
        /// <param name="config">The configuration.</param>
        protected SunatCe(SolConfig config)
        {
            _config = config;
        }
        #endregion

        #region Method Sunat

        /// <summary>
        /// Recibe la ruta XML con un único formato digital y devuelve la Constancia de Recepción – SUNAT. 
        /// </summary>
        /// <param name="pathFile">Ruta del Archivo XML</param>
        /// <param name="content"></param>
        /// <returns>La respuesta contenida en el XML de Respuesta de la Sunat, si existe</returns>
        public SunatResponse SendDocument(string pathFile, byte[] content)
        {
            var fileToZip = pathFile + Resources.ExtensionFile;
            var nameOfFileZip = pathFile + Resources.ExtensionZipFile;

            var response = new SunatResponse
            {
                Success = false
            };
            try
            {
                var zipBytes = ProcessZip.CompressFile(fileToZip, content);
                using (var service = ServiceHelper.GetService<billServiceClient>(_config, BaseUrl))
                {
                    var resultBytes = service.sendBill(nameOfFileZip, zipBytes);
                    using (var xmlCdr = ProcessZip.ExtractFile(resultBytes))
                        response = new SunatResponse
                        {
                            Success = true,
                            ApplicationResponse = ProcessXml.GetAppResponse(xmlCdr),
                            ContentZip = resultBytes
                        };
                }
            }
            catch (FaultException ex)
            {
                response.Error = new ErrorResponse
                {
                    Code = ex.Message,
                    Description = ProcessXml.GetDescriptionError(ex.Message)
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
        /// Envia una Resumen o Comunicaciones de Baja a Sunat
        /// </summary>
        /// <param name="pathFile">Ruta del archivo XML que contiene el resumen</param>
        /// <param name="content"></param>
        /// <returns>Retorna un estado booleano que indica si no hubo errores, con un string que contiene el Nro Ticket,
        /// con el que posteriormente, utilizando el método getStatus se puede obtener Constancia de Recepcióno</returns>
        public TicketResponse SendSummary(string pathFile, byte[] content)
        {
            var fileToZip = pathFile + Resources.ExtensionFile;
            var nameOfFileZip = pathFile + Resources.ExtensionZipFile;

            var res = new TicketResponse();
            try
            {
                var zipBytes = ProcessZip.CompressFile(fileToZip, content);
                using (var service = ServiceHelper.GetService<billServiceClient>(_config, BaseUrl))
                {
                    res.Ticket = service.sendSummary(nameOfFileZip, zipBytes);
                    res.Success = true;
                }
            }
            catch (FaultException ex)
            {
                res.Error = new ErrorResponse
                {
                    Code = ex.Message,
                    Description = ProcessXml.GetDescriptionError(ex.Message)
                };
            }
            catch (Exception er)
            {
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
                using (var service = ServiceHelper.GetService<billServiceClient>(_config, BaseUrl))
                {
                    var response = service.getStatus(pstrTicket);
                    switch (response.statusCode)
                    {
                        case "0":
                        case "99":
                            res.Success = true;
                            using (var xmlCdr = ProcessZip.ExtractFile(response.content))
                                res.ApplicationResponse = ProcessXml.GetAppResponse(xmlCdr);
                            res.ContentZip = response.content;
                            break;
                        case "98":
                            res.Success = false;
                            res.Error = new ErrorResponse { Description = "En Proceso..." };
                            break;
                    }
                }
            }
            catch (FaultException ex)
            {
                res.Error = new ErrorResponse
                {
                    Code = ex.Message,
                    Description = ProcessXml.GetDescriptionError(ex.Message),
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
        #endregion
    }
}
