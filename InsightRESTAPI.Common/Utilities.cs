using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InsightRESTAPI.Common
{
    public static class Utilities
    {
        private static IConfigurationRoot GetConfigurationRoot()
        {
            var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();
            //var serverSection = configuration.GetSection("Server").GetSection("Name").Value;
            return configuration;
        }
        private static IConfigurationRoot GetConfiguration()
        {
            return GetConfigurationRoot();
        }
        private static dynamic CheckPrimitiveType(Type propertyType)
        {
            bool isPrimitive;
            bool isDateTime;
            if (propertyType == typeof(int))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(int)
                };
            }
            else if (propertyType == typeof(int?))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(int?)
                };
            }
            else if (propertyType == typeof(double))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(double)
                };
            }
            else if (propertyType == typeof(double?))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(double?)
                };
            }
            else if (propertyType == typeof(float))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(float)
                };
            }
            else if (propertyType == typeof(float?))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(float?)
                };
            }
            else if (propertyType == typeof(decimal))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(decimal)
                };
            }
            else if (propertyType == typeof(decimal?))
            {
                isPrimitive = true;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(decimal?)
                };
            }
            else if (propertyType == typeof(DateTime))
            {
                isPrimitive = false;
                isDateTime = true;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(DateTime)
                };
            }
            else if (propertyType == typeof(DateTime?))
            {
                isPrimitive = false;
                isDateTime = true;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(DateTime?)
                };
            }
            else
            {
                isPrimitive = false;
                isDateTime = false;
                return new
                {
                    IsPrimitive = isPrimitive,
                    IsDateTime = isDateTime,
                    Type = typeof(Nullable)
                };
            }
        }
        private static bool IsDateTime(Type propertyType)
        {
            if (propertyType == typeof(DateTime))
            {
                return true;
            }
            else if (propertyType == typeof(DateTime?))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static string AddWhiteSpaceToString(string value, int counter)
        {

            if (value.Length >= counter)
            {
                return value;
            }
            else
            {
                int valueLength = counter - value.Length;
                int LoopTracker = 0;
                while (LoopTracker < valueLength)
                {
                    value += " ";
                    LoopTracker++;
                }
                return value;
            }
        }

        // Public Methods
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var s = Convert.ToBase64String(randomNumber);
                var randomNumberString = Regex.Replace(s, @"[$&+,:;=?@#|'<>/\\.^*()%!-]", "");
                return randomNumberString;
            }
        }
        public static DateTime GetDate()
        {
            return DateTime.Now;
        }
        public static string GetRequestResponseTime()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public static dynamic SerializeDynamicModel(dynamic value)
        {
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value));
        }
        public static string ResolveServerName(string serverNameFromDB)
        {
            return AddWhiteSpaceToString(serverNameFromDB, 16);
        }
        public static string BuildDynamicConnectionString(string conString, string dbConnection, string databaseName, string dbUsername, string dbPassword)
        {
            //string conString = "";
            //var configuration = GetConfiguration();
            //var data = configuration.GetSection("ConnectionStrings").GetChildren().Select(e => new
            //{
            //    e.Key,
            //    e.Value
            //})
            //.ToList();
            //conString = data.Where(e => e.Key == "DynamicConnection").Select(e => e.Value).FirstOrDefault().ToString();
            conString = conString.Replace("(DynamicServer)", dbConnection).ToString().Replace("(DynamicDB)", databaseName);
            if (!dbUsername.IsNullOrEmpty() && !dbPassword.IsNullOrEmpty())
            {
                conString = conString.Replace("(Credentials)", "User ID=" + dbUsername + ";password=" + dbPassword + "");
            }
            else
            {
                conString = conString.Replace("(Credentials)", "Integrated Security=SSPI");
            }
            return conString;
        }
        public static string SerializeObjectToXML<T>(T filter)
        {
            string xml = null;
            using (StringWriter sw = new StringWriter())
            {

                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, filter);
                try
                {
                    xml = sw.ToString();

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return xml;
        }
        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            try
            {
                var dataTable = new DataTable(typeof(T).Name);
                // Get all the properties
                var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                    // Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                foreach (object item in items)
                {
                    var values = new object[Props.Length - 1 + 1];
                    for (int i = 0; i <= Props.Length - 1; i++)
                        // inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    dataTable.Rows.Add(values);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public static async Task<string> DumpToJSON(string data, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").RemoveWhiteSpace().RemoveSpecialCharacter() + "__" + Guid.NewGuid().ToString().Replace("-", "_") + ".json";
            // Write the string array to a new file named "WriteLines.json".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, fileName)))
            {
                await outputFile.WriteAsync(data);
            }
            return fileName;
        }
        public static bool ReadDebugModeConfig()
        {
            try
            {
                var configuration = GetConfiguration();
                return Convert.ToBoolean(configuration.GetSection("IsDebugMode").Value);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string GenerateRandomCodeStringByByteSize(int _byte)
        {
            var randomNumber = new byte[_byte];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                string s = Convert.ToBase64String(randomNumber);
                s = Regex.Replace(s, @"[$&+,:;=?@#|'<>/\\.^*()%!-]", "");
                return s;
            }
        }
        public static string SerializeToXml(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType(), "");
            string result = string.Empty;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input);

                memStm.Position = 0;
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }
        public static dynamic ReadConfigByKey(string key, bool isList = false)
        {
            var configuration = GetConfiguration();
            if (!isList)
            {
                return configuration.GetSection(key).Value;
            }
            else
            {
                return configuration.GetSection(key).Get<List<string>>();
            }
        }

        /// <summary>
        /// Converts an object to a DataTable with columns in the exact order as the names in the columnNames list. 
        /// Use this to ensure changes to an object do not result in the wrong order of columns in the DataTable
        /// which will cause incorrect data/errors. 
        ///
        /// Column names must match the property names on provided items. The column will have the same type the 
        /// object properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnNames"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(List<string> columnNames, List<T> items)
        {
            try
            {
                var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var dataTable = new DataTable(typeof(T).Name);
                var propTypes = new Dictionary<string, Type>();

                foreach (PropertyInfo prop in props)
                {
                    propTypes[prop.Name] = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                }

                columnNames.ForEach(name => dataTable.Columns.Add(name, propTypes[name] ?? typeof(string)));

                foreach (T item in items)
                {
                    var row = dataTable.Rows.Add();
                    foreach (PropertyInfo prop in props)
                    {
                        row[prop.Name] = prop.GetValue(item, null) ?? DBNull.Value;
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        public static List<T> ConvertFromDataTable<T>(DataTable dt) where T : new()
        {
            var items = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                T obj = new T();
                foreach (DataColumn col in dt.Columns)
                {
                    try
                    {
                        var objProperty = obj.GetType().GetProperty(col.ColumnName);
                        if (objProperty == null) continue;

                        var value = row.Field<dynamic>(col.ColumnName);
                        if (value == null) continue;

                        var propType = objProperty.PropertyType.FullName;

                        if (propType.Contains("System.Double") && !(value is double))
                        {
                            value = value is string && value == "" ? 0 : Convert.ToDouble(value);
                        }
                        else if (propType.Contains("System.Int64") && !(value is Int64))
                        {
                            value = value is string && value == "" ? (Int64)0 : Convert.ToInt64(value);
                        }
                        else if (propType.Contains("System.Int32") && !(value is Int32))
                        {
                            value = value is string && value == "" ? 0 : Convert.ToInt32(value);
                        }
                        else if (propType.Contains("System.Boolean") && !(value is bool))
                        {
                            value = value is string && value == "" ? false : Convert.ToBoolean(value);
                        }
                        objProperty.SetValue(obj, value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                items.Add(obj);
            }

            return items;
        }
    }
}
