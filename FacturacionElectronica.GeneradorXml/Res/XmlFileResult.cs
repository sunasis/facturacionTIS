namespace FacturacionElectronica.GeneradorXml.Res
{
    /// <summary>
    /// Class XmlFileResult
    /// </summary>
    public class XmlFileResult
    {
        /// <summary>
        /// Nombre del archivo XML sin extension.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Contenido del Archivo Xml.
        /// </summary>
        public byte[] Content { get; set; }
    }
}
