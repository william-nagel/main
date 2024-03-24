using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HL7Service.Entities
{
    public static class EntitySerializer
    {
        /// <summary>
        ///     Example Output:
        ///     [Patient Name] [SSN] [Age] [Phone Number] [Status]
        ///     [Prescott, Zeke] [542-51-6641] [21] [801-555-2134] [Opratory=2,PCP=1]
        ///     [Goldstein, Bucky] [635-45-1254] [42] [435-555-1541] [Opratory=1, PCP=1]
        ///     [Vox, Bono] [414-45-1475] [51] [801-555-2100] [Opratory=3, PCP=2]
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static string ToBraceSeparatedString(List<EntityObject> entities)
        {
            StringBuilder result = new StringBuilder();

            // lay down the header row first
            foreach (var property in entities[0].Properties)
            {
                result.Append($"[{property.Name}] ");
            }

            result.Length--;
            result.AppendLine();

            // then append the data rows
            foreach (var entity in entities)
            {
                foreach (var property in entity.Properties)
                {
                    result.Append($"[{property.Value}] ");
                }

                result.Length--;
                result.AppendLine();
            }

            return result.ToString();
        }

        public static string ToCommaSeparatedString(List<EntityObject> entities)
        {
            // TODO: code this if and when it is necessary
            throw new NotImplementedException("EntitySerializer.ToCommaSeparatedString()");
        }

        public static string ToJsonString(List<EntityObject> entities)
        {
            // TODO: code this if and when it is necessary
            throw new NotImplementedException("EntitySerializer.ToJsonString()");
        }

        public static string ToXmlString(List<EntityObject> entities)
        {
            // TODO: code this if and when it is necessary
            throw new NotImplementedException("EntitySerializer.ToXmlString()");
        }
    }
}
