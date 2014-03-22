using System;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Pulsar.Pipeline
{
    public static class JsonHelper
    {
        #region Static methods

        /// <summary>
        /// Checks if a json text match a schema
        /// </summary>
        /// <param name="text">Json text to validate</param>
        /// <param name="schema">Schema to use for validation</param>
        /// <param name="error">Error message</param>
        /// <returns>Returns true if the text is valid otherwise false</returns>
        public static bool IsValid(string text, string schema, out string error)
        {
            JObject jData = JObject.Parse(text);
            JsonSchema jSchema = JsonSchema.Parse(schema);
            IList<string> validationMsg;
            bool result = jData.IsValid(jSchema, out validationMsg);

            error = null;
            if (result) 
                return true;

            int length = 0;
            for (int i = 0; i < validationMsg.Count; i++)
                length += validationMsg[i].Length + Environment.NewLine.Length;

            StringBuilder strBuilder = new StringBuilder(length);
            for (int i = 0; i < validationMsg.Count; i++)
                strBuilder.AppendLine(validationMsg[i]);

            error = strBuilder.ToString();

            return false;
        }

        /// <summary>
        /// Gets the string value of a json property
        /// </summary>
        /// <param name="jObject">Json object</param>
        /// <param name="property">Name of the property</param>
        /// <returns>Returns the value of the property if found otherwise an empty string</returns>
        public static string GetString(JObject jObject, string property)
        {
            JToken token = jObject[property];
            if (token == null)
                return string.Empty;

            return (string)token;
        }

        public static string[] GetStringAsArray(JObject jObject, string property)
        {
            JToken token = jObject[property];
            string[] result;
            JArray jArray = token as JArray;
            if (jArray != null)
            {
                result = new string[jArray.Count];
                for (int i = 0; i < jArray.Count; i++)
                    result[i] = (string) jArray[i];
            }
            else
                result = new[] {(string) token};

            return result;
        }

        public static object ConvertToObject(JToken token)
        {
            if(token == null)
                throw new ArgumentNullException("token");

            JTokenType type = token.Type;
            if ((type == JTokenType.Comment) || (type == JTokenType.None) || (type == JTokenType.Null) ||
                    (type == JTokenType.Undefined))
                return null;

            object value = null;
            if (type == JTokenType.Object)
                value = ConvertObjectToMap((JObject)token);

            if (type == JTokenType.Array)
                value = GetArrayContents((JArray)token);

            return value ?? token.Value<string>();
        }

        public static List<object> GetArrayContents(JArray jArray)
        {
            if(jArray == null)
                throw new ArgumentNullException("jArray");

            List<object> result = new List<object>();
            for (int i = 0; i < jArray.Count; i++)
            {
                object value = ConvertToObject(jArray[i]);
                if(value == null)
                    continue;
                
                result.Add(value);
            }

            return result;
        }

        public static Dictionary<string, object> ConvertObjectToMap(JObject jObject)
        {
            if(jObject == null)
                throw new ArgumentNullException("jObject");

            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (JProperty property in jObject.Properties())
            {
                JToken token = property.Value;
                object value = ConvertToObject(token);
                if(value == null)
                    continue;

                result.Add(property.Name, value);
            }

            return result;
        }

        #endregion
    }
}
