using HL7Service.Enums;
using System.Text;

namespace HL7Service.Entities
{
    /// <summary>
    ///     Not sure if it makes sense to implement a factory pattern after all
    /// </summary>
    public class EntityObject
    {
        public List<EntityProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }
        private List<EntityProperty> _properties;



        /// <summary>
        ///     Empty ctor
        /// </summary>
        public EntityObject()
        {
            _properties = new List<EntityProperty>();
        }

        /// <summary>
        ///     Method used to serialize (flatten) structure for transfer 
        /// </summary>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string ToString(DataFormats dataFormat)
        {
            StringBuilder result = new StringBuilder();


            foreach (var property in this.Properties)
            {
                switch (dataFormat)
                {
                    case DataFormats.BraceDelimited:
                        // TODO: test this
                        result.AppendLine(property.ToString());
                        break;

                    case DataFormats.CommaDelimited:
                        // TODO: implement this or comment out
                        throw new NotImplementedException("EntityFormatter.SerializeEntities(): DataFormats.CommaDelimited");
                        break;

                    case DataFormats.Json:
                        // TODO: implement this or comment out
                        throw new NotImplementedException("EntityFormatter.SerializeEntities(): DataFormats.Json");
                        break;

                    case DataFormats.Xml:
                        // TODO: implement this or comment out
                        throw new NotImplementedException("EntityFormatter.SerializeEntities(): DataFormats.Xml");
                        break;
                }
            }

            return result.ToString();
        }
    }
}
