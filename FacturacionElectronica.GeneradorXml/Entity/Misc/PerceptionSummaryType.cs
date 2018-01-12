using FacturacionElectronica.GeneradorXml.Enums;

namespace FacturacionElectronica.GeneradorXml.Entity.Misc
{
    /// <summary>
    /// Class PerceptionSummaryType.
    /// </summary>
    public class PerceptionSummaryType
    {
        /// <summary>
        /// Gets or sets the Regimen de percepción.
        /// </summary>
        /// <value>The cod regimen.</value>
        public RegimenPercepcion CodRegimen { get; set; }
        /// <summary>
        /// Gets or sets the Tasa de la percepción.
        /// </summary>
        /// <value>The tasa.</value>
        public decimal Tasa { get; set; }
        /// <summary>
        /// Gets or sets the monto de la percecpcion.
        /// </summary>
        /// <value>The monto.</value>
        public decimal Monto { get; set; }
        /// <summary>
        /// Gets or sets the Monto total a cobrar incluida la percepción.
        /// </summary>
        /// <value>The monto total.</value>
        public decimal MontoTotal { get; set; }
        public decimal MontoBase { get; set; }
    }
}
