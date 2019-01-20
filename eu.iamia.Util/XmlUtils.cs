namespace eu.iamia.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public static class XmlUtils
    {
        public static String PrettyXml(this String source)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                var element = XElement.Parse(source);

                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NewLineOnAttributes = true
                };

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    element.Save(xmlWriter);
                }

                return stringBuilder.ToString();
            }
            catch (Exception)
            {

                return source;
            }
        }

        /// <summary>
        /// Returns Xml String representation of source with UTF-16 encoding.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String SerializeToXml(this object source)
        {
            String result;

            if (source == null) { return ""; }

            try
            {
                var serializer = new XmlSerializer(source.GetType());

                var settings = new XmlWriterSettings
                {
                    Encoding = new UnicodeEncoding(false, false),
                    Indent = true,
                    OmitXmlDeclaration = false
                };

                using (var stringWriter = new StringWriter())
                {
                    using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        serializer.Serialize(xmlWriter, source);
                        result = stringWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                result = String.Format("Unable to serialize object of type {0}, message: {1}", source.GetType().FullName, ex.Message);
            }

            return result;
        }


        private class StringWriteUtf8 : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }


        /// <summary>
        /// Retuns XML String representation of surce with UTF-8 encoding.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String SerializeToXmlUtf8(this object source)
        {
            String result;

            try
            {
                var serializer = new XmlSerializer(source.GetType());

                var settings = new XmlWriterSettings
                {
                    Encoding =  new UnicodeEncoding(false, false), // QQQ Encoding.UTF8
                    Indent = true,
                    OmitXmlDeclaration = false
                };

                using (var stringWriter = new StringWriteUtf8())
                {
                    using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        serializer.Serialize(xmlWriter, source);
                        result = stringWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                result = String.Format("Unable to serialize object of type {0}, message: {1}", source.GetType().FullName, ex.Message);
            }

            return result;
        }


        /// <summary>
        /// Replace newline \r\n with | (pipe).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String SingleLine(this String source)
        {
            return source.Replace("\r\n", "|");
        }


        /// <summary>
        /// XmlSerialize the specified list of strings.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static String Serialize(List<String> source)
        {
            var serializer = new XmlSerializer(typeof(List<String>));

            var settings = new XmlWriterSettings
            {
                Encoding = new UnicodeEncoding(false, false),
                Indent = false,
                OmitXmlDeclaration = true,
                NewLineHandling = NewLineHandling.None,
            };

            using (var stringWriter = new StringWriteUtf8())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, source ?? new List<String>(0));
                    var t1 = stringWriter.ToString();
                    var serialized = t1.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
                    return serialized;
                }
            }
        }


        /// <summary>
        /// Xml-Deserialize the specified xml-string representation of a List of strings.
        /// </summary>
        /// <param name="serialized"></param>
        public static List<String> Deserialize(String serialized)
        {
            var serializer = new XmlSerializer(typeof(List<String>));

            var settings = new XmlReaderSettings();

            using (var stringReader = new StringReader(serialized))
            {
                using (var xmlReader = XmlReader.Create(stringReader, settings))
                {
                    var t1 = serializer.Deserialize(xmlReader);
                    var list = t1 as List<String>;
                    return list;
                }
            }
        }


        public static bool TryParse<T>(string xmlSerializedObject, out T value) where T : class, new()
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlReaderSettings();

            using (var stringReader = new StringReader(xmlSerializedObject))
            {
                using (var xmlReader = XmlReader.Create(stringReader, settings))
                {
                    var t1 = serializer.Deserialize(xmlReader);
                    value = t1 as T;
                }
            }

            return value != null;
        }
    }


}
