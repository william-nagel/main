namespace HL7Service.Enums
{
    public enum DataFormats
    {
        /// <summary>
        ///     Fields separated by square braces
        /// </summary>
        BraceDelimited,
        /// <summary>
        ///     Fields separated by commas
        /// </summary>
        CommaDelimited,
        /// <summary>
        ///     Formatted as JSON object
        /// </summary>
        Json,
        /// <summary>
        ///     Formatted as XML document
        /// </summary>
        Xml,
    }
}
