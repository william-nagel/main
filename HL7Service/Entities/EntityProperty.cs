using System;

namespace HL7Service.Entities
{
    public class EntityProperty
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public object? Value { get; set; }


        /// <summary>
        ///     empty ctor
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public EntityProperty()
        {
            this.Index = 0;
            this.Name = string.Empty;
            this.Value = null;
        }

        /// <summary>
        ///     ctor
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public EntityProperty(int index, string name, object value) 
        {
            this.Index = index;
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"Index: {this.Index}; Name: {this.Name}; Value: {this.Value}";
        }
    }
}
