using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FZ4P.Commons.Helper
{
    public static class DataIOHelper
    {
        public static bool SerializeToXMLViewerFile<T>(this T toSerialize, string FileName) where T : class, new()
        {
            try
            {
                string fullPath = string.Empty;
                fullPath = FileExtensionChanged(FileName);
                string dir = Path.GetDirectoryName(fullPath);
                try { Directory.CreateDirectory(dir); }
                catch
                { return false; }
                string backFile = Path.ChangeExtension(fullPath, ".bak");
                if (File.Exists(backFile))
                    File.Delete(backFile);
                try { File.WriteAllText(backFile, toSerialize.SerializeToXMLWithCategory<T>()); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
                FileInfo info = new FileInfo(backFile);
                if (info.Length == 0)
                { return false; }

                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                File.Move(backFile, fullPath);
                return true;
            }
            catch { return false; }
        }

        public static bool SerializeToXMLViewerFile<T, U>(this T toSerialize, U toSerializeExtra, string FileName) where T : class, new()
        {
            try
            {
                string fullPath = string.Empty;
                fullPath = FileExtensionChanged(FileName);
                string dir = Path.GetDirectoryName(fullPath);
                try { Directory.CreateDirectory(dir); }
                catch
                { return false; }
                string backFile = Path.ChangeExtension(fullPath, ".bak");
                if (File.Exists(backFile))
                    File.Delete(backFile);
                try { File.WriteAllText(backFile, toSerialize.SerializeToXMLWithCategory<T,U>(toSerializeExtra)); }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
                FileInfo info = new FileInfo(backFile);
                if (info.Length == 0)
                { return false; }

                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                File.Move(backFile, fullPath);
                return true;
            }
            catch { return false; }
        }


        public static string SerializeToXMLWithCategory<T,U>(this T obj, U objExtra)
        {
            if (obj == null)
                return string.Empty;

            try
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    Indent = true
                };

                using (var ms = new MemoryStream())
                using (var xw = XmlWriter.Create(ms, settings))
                {
                    Type type = typeof(T);

                    xw.WriteStartDocument();
                    xw.WriteStartElement(type.Name);

                    PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var prop in props)
                    {
                        if (!prop.CanRead)
                            continue;

                        object value = prop.GetValue(obj, null);

                        xw.WriteStartElement(prop.Name);

                        var categoryAttr = prop.GetCustomAttribute<ConditionAttribute>();
                        if (categoryAttr != null)
                        {
                            xw.WriteAttributeString("Category", categoryAttr.Category ?? string.Empty);
                            xw.WriteAttributeString("ToDo1", categoryAttr.ToDo1 ?? string.Empty);
                            xw.WriteAttributeString("ToDo2", categoryAttr.ToDo2 ?? string.Empty);
                            xw.WriteAttributeString("Unit", categoryAttr.Unit ?? string.Empty);
                        }

                        WriteValue(xw, value);

                        xw.WriteEndElement();
                    }
                    // 추가 클래스 데이터
                    if (objExtra != null)
                    {
                        Type additionalType = objExtra.GetType();

                        xw.WriteStartElement(additionalType.Name);

                        PropertyInfo[] additionalProps = additionalType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                        foreach (var prop in additionalProps)
                        {
                            if (!prop.CanRead)
                                continue;

                            object value =
                                prop.GetValue(objExtra, null);

                            xw.WriteStartElement(prop.Name);

                            var categoryAttr = prop.GetCustomAttribute<ConditionAttribute>();

                            if (categoryAttr != null)
                            {
                                xw.WriteAttributeString("Category", categoryAttr.Category ?? string.Empty);
                                xw.WriteAttributeString("ToDo1", categoryAttr.ToDo1 ?? string.Empty);
                                xw.WriteAttributeString("ToDo2", categoryAttr.ToDo2 ?? string.Empty);
                                xw.WriteAttributeString("Unit", categoryAttr.Unit ?? string.Empty);
                            }

                            WriteValue(xw, value);

                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Flush();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SerializeToXMLWithCategory<T>(this T obj)
        {
            if (obj == null)
                return string.Empty;

            try
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    Indent = true
                };

                using (var ms = new MemoryStream())
                using (var xw = XmlWriter.Create(ms, settings))
                {
                    Type type = typeof(T);

                    xw.WriteStartDocument();
                    xw.WriteStartElement(type.Name);

                    PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var prop in props)
                    {
                        if (!prop.CanRead)
                            continue;

                        object value = prop.GetValue(obj, null);

                        xw.WriteStartElement(prop.Name);

                        var categoryAttr = prop.GetCustomAttribute<ConditionAttribute>();
                        if (categoryAttr != null)
                        { 
                            xw.WriteAttributeString("Category", categoryAttr.Category);
                            xw.WriteAttributeString("ToDo1", categoryAttr.ToDo1);
                            xw.WriteAttributeString("ToDo2", categoryAttr.ToDo2);
                            xw.WriteAttributeString("Unit", categoryAttr.Unit);
                        }

                        WriteValue(xw, value);

                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Flush();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void WriteValue(XmlWriter xw, object value)
        {
            if (value == null)
                return;

            Type valueType = value.GetType();

            if (IsSimpleType(valueType))
            {
                xw.WriteString(Convert.ToString(value));
                return;
            }

            if (value is IEnumerable enumerable && !(value is string))
            {
                foreach (object item in enumerable)
                {
                    if (item == null)
                        continue;

                    Type itemType = item.GetType();

                    string elementName = GetElementName(itemType);

                    xw.WriteStartElement(elementName);

                    if (IsSimpleType(itemType))
                    {
                        xw.WriteString(Convert.ToString(item));
                    }
                    else
                    {
                        WriteObjectProperties(xw, item);
                    }

                    xw.WriteEndElement();
                }

                return;
            }

            WriteObjectProperties(xw, value);
        }
        private static void WriteObjectProperties(XmlWriter xw, object obj)
        {
            if (obj == null)
                return;

            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!prop.CanRead)
                    continue;

                object value = prop.GetValue(obj, null);

                xw.WriteStartElement(prop.Name);

                var categoryAttr = prop.GetCustomAttribute<ConditionAttribute>();
                if (categoryAttr != null)
                {
                    xw.WriteAttributeString("Category", categoryAttr.Category);
                }

                WriteValue(xw, value);

                xw.WriteEndElement();
            }
        }
        private static bool IsSimpleType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(Guid);
        }
        private static string GetElementName(Type itemType)
        {
            itemType = Nullable.GetUnderlyingType(itemType) ?? itemType;

            if (itemType == typeof(string))
                return "string";

            if (itemType == typeof(int))
                return "int";

            if (itemType == typeof(double))
                return "double";

            if (itemType == typeof(float))
                return "float";

            if (itemType == typeof(bool))
                return "bool";

            return itemType.Name;
        }

        private static string FileExtensionChanged(string file)
        {
            string fileName = file;

            if (Path.GetExtension(fileName).ToLower() != ".rcpv")
            {
                fileName = Path.ChangeExtension(fileName, ".rcpv");
            }
            return fileName;
        }
    }
}
