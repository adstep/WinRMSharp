using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using WinRMSharp.Contracts;

namespace WinRMSharp.Utils
{
    internal static class Xml
    {
        private static readonly Dictionary<string, string> NamespaceToPrefix = new()
        {
            { Namespace.XML_SCHEMA, "xsd" },
            { Namespace.XML_SCHEMA_INSTANCE, "xsi" },
            { Namespace.SOAP_ENVELOPE, "env" },
            { Namespace.ADDRESSING, "a" },
            { Namespace.CIM_BINDING, "b" },
            { Namespace.ENUMERATION, "n" },
            { Namespace.TRANSFER, "x" },
            { Namespace.WSMID, "wsmid" },
            { Namespace.WSMAN_0, "w" },
            { Namespace.WSMAN_1, "p" },
            { Namespace.WSMAN_SHELL, "rsp" },
            { Namespace.WSMAN_CONFIG, "cfg"}
        };

        public static string Serialize<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Extract namespaces actually used from the type AND instance
            HashSet<string> usedNamespaces = ExtractNamespacesFromInstance(obj);

            // Always include the xsi and xsd namespaces first
            usedNamespaces = new HashSet<string>(new[]
            {
                Namespace.XML_SCHEMA_INSTANCE,
                Namespace.XML_SCHEMA
            }.Concat(usedNamespaces));

            // Build XmlSerializerNamespaces with desired prefixes
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            foreach (string nsUri in usedNamespaces)
            {
                // Check if the namespace URI exists in the dictionary
                if (!NamespaceToPrefix.TryGetValue(nsUri, out string? prefix))
                {
                    throw new InvalidOperationException($"Unknown namespace URI encountered: {nsUri}");
                }

                namespaces.Add(prefix, nsUri);
            }

            // Serialize with filtered namespaces
            return Serialize(obj, namespaces);
        }

        private static string Serialize<T>(T obj, XmlSerializerNamespaces namespaces)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, obj, namespaces);
            return writer.ToString();
        }

        private static HashSet<string> ExtractNamespacesFromInstance(object obj)
        {
            HashSet<string> namespaces = new HashSet<string>();

            // Get the actual runtime type of the object
            Type type = obj.GetType();

            // Check for XmlRoot attribute
            XmlRootAttribute? xmlRootAttr = type
                .GetCustomAttributes(typeof(XmlRootAttribute), true)
                .FirstOrDefault() as XmlRootAttribute;
            if (xmlRootAttr?.Namespace != null)
            {
                namespaces.Add(xmlRootAttr.Namespace);
            }

            // Check for XmlType attribute
            XmlTypeAttribute? xmlTypeAttr = type
                .GetCustomAttributes(typeof(XmlTypeAttribute), true)
                .FirstOrDefault() as XmlTypeAttribute;
            if (xmlTypeAttr?.Namespace != null)
            {
                namespaces.Add(xmlTypeAttr.Namespace);
            }

            // Process properties
            foreach (PropertyInfo property in type.GetProperties())
            {
                // Get the property value
                object? propertyValue = property.GetValue(obj);

                // Skip if property value is null - don't include its namespaces
                if (propertyValue == null)
                    continue;

                // Check for XmlElement attribute
                XmlElementAttribute? xmlElementAttr = property
                    .GetCustomAttributes(typeof(XmlElementAttribute), true)
                    .FirstOrDefault() as XmlElementAttribute;
                if (xmlElementAttr?.Namespace != null)
                {
                    namespaces.Add(xmlElementAttr.Namespace);
                }

                // Check for XmlAttribute attribute
                XmlAttributeAttribute? xmlAttributeAttr = property
                    .GetCustomAttributes(typeof(XmlAttributeAttribute), true)
                    .FirstOrDefault() as XmlAttributeAttribute;
                if (xmlAttributeAttr?.Namespace != null)
                {
                    namespaces.Add(xmlAttributeAttr.Namespace);
                }

                // Recursively check property types that are complex objects
                Type propertyType = property.PropertyType;
                if (propertyType != typeof(string)
                    && !propertyType.IsPrimitive
                    && !propertyType.IsEnum
                    && propertyType.Namespace != "System"
                    && !propertyType.IsArray
                    && !propertyType.IsGenericType)
                {
                    HashSet<string> nestedNamespaces = ExtractNamespacesFromInstance(propertyValue);
                    foreach (string ns in nestedNamespaces)
                    {
                        namespaces.Add(ns);
                    }
                }

                // Handle arrays and collections
                if (propertyType.IsArray)
                {
                    Type elementType = propertyType.GetElementType()!;
                    Array array = (Array)propertyValue;

                    // If array is not empty and elements are complex types
                    if (array.Length > 0
                        && elementType != typeof(string)
                        && !elementType.IsPrimitive
                        && !elementType.IsEnum
                        && elementType.Namespace != "System")
                    {
                        foreach (object item in array)
                        {
                            if (item != null)
                            {
                                HashSet<string> nestedNamespaces = ExtractNamespacesFromInstance(item);
                                foreach (string ns in nestedNamespaces)
                                {
                                    namespaces.Add(ns);
                                }
                            }
                        }
                    }
                }
                else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type elementType = propertyType.GetGenericArguments()[0];
                    IEnumerable collection = (IEnumerable)propertyValue;

                    // If collection has elements and they are complex types
                    if (elementType != typeof(string)
                        && !elementType.IsPrimitive
                        && !elementType.IsEnum
                        && elementType.Namespace != "System")
                    {
                        foreach (object item in collection)
                        {
                            if (item != null)
                            {
                                HashSet<string> nestedNamespaces = ExtractNamespacesFromInstance(item);
                                foreach (string ns in nestedNamespaces)
                                {
                                    namespaces.Add(ns);
                                }
                            }
                        }
                    }
                }
            }

            return namespaces;
        }

        public static XDocument Parse(this string text)
        {
            return XDocument.Parse(text);
        }

        public static string? Format(this string text)
        {
            try
            {
                XDocument doc = XDocument.Parse(text);
                return doc.ToString();
            }
            catch (Exception)
            {
                return text;
            }
        }
    }
}
