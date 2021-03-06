﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ViSo.SharedEnums;

namespace GeneralExtensions
{
    public static class ObjectExstentions
    {
        public static bool TryToBool<T>(this T value)
        {
            try
            {
                if (value == null)
                {
                    return false;
                }

                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        public static int ToInt32<T>(this T value)
        {
            return Convert.ToInt32(value);
        }

        public static long ToInt64<T>(this T value)
        {
            return Convert.ToInt64(value);
        }

        public static decimal ToDecimal<T>(this T value)
        {
            StringBuilder resultText = new StringBuilder();

            bool hasDecimal = false;

            string valueString = value.ToString();

            string seperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            for (int x = valueString.Length - 1; x >= 0; --x)
            {
                char read = valueString[x];

                if (Char.IsNumber(read))
                {
                    resultText.Insert(0, read);

                    continue;
                }
                else if (read == '-' && x == 0)
                {
                    resultText.Insert(0, read);

                    continue;
                }
                else if (hasDecimal)
                {
                    continue;
                }

                resultText.Insert(0, seperator);

                hasDecimal = true;
            }

            return Convert.ToDecimal(resultText.ToString());
        }

        public static double ToDouble<T>(this T value)
        {
            return Convert.ToDouble(value);
        }

        public static string ParseToString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }

        public static DateTime TryToDate<T>(this T value)
        {
            try
            {
                if (value == null)
                {
                    return DateTime.Now;
                }

                return Convert.ToDateTime(value);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static T To<T>(this object item)
        {
            if (item == null)
            {
                return default(T);
            }

            return (T)item;
        }

        public static object CloneTo<T>(this T source, Type resultT)
        {
            Type sourceT = source.GetType();

            ConstructorInfo constructorInfo = resultT.GetConstructor(Type.EmptyTypes);

            object result = constructorInfo.Invoke(null);

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                try
                {
                    PropertyInfo resultP = resultT.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        continue;
                    }

                    resultP.SetValue(result, item.GetValue(source, null), null);
                }
                catch
                {
                    // Do Nothing
                }
            }

            return result;
        }

        public static T Clone<T>(this T source)
        {
            Type sourceT = source.GetType();

            Type resultT = source.GetType();

            ConstructorInfo constructorInfo = resultT.GetConstructor(Type.EmptyTypes);

            object result = constructorInfo.Invoke(null);

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                try
                {
                    PropertyInfo resultP = resultT.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        continue;
                    }

                    resultP.SetValue(result, item.GetValue(source, null), null);
                }
                catch
                {
                    // Do Nothing
                }
            }

            return (T)result;
        }

        public static byte[] ZipFile<T>(this T source)
        {
            byte[] result = null;

            using (MemoryStream queryStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(queryStream, CompressionMode.Compress))
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(zipStream, source);
                }

                result = queryStream.ToArray();
            }

            return result ?? (new byte[] { });
        }

        public static T CopyTo<T>(this T source, T result)
        {
            var sourceT = source.GetType();

            var resultT = result.GetType();

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                if (item.CanRead && !item.CanWrite)
                {
                    continue;
                }

                try
                {
                    var resultP = resultT.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        continue;
                    }

                    resultP.SetValue(result, item.GetValue(source, null), null);
                }
                catch
                {
                    // Do Nothing
                }
            }

            return result;
        }

        public static T TryCast<T>(object o)
        {
            return (T)o;
        }

        public static object CopyToObject<T>(this T source, object result)
        {
            var sourceT = source.GetType();

            var resultT = result.GetType();

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                if (item.CanRead && !item.CanWrite)
                {
                    continue;
                }

                try
                {
                    var resultP = resultT.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        continue;
                    }

                    resultP.SetValue(result, item.GetValue(source, null), null);
                }
                catch
                {
                    // Do Nothing
                }
            }

            return result;
        }

        public static List<object> CopyToObject<T>(this List<T> source, Type resultType)
        {
            List<object> result = new List<object>();

            foreach (T item in source)
            {
                var instance = Activator.CreateInstance(resultType);

                result.Add(item.CopyToObject(instance));
            }

            return result;
        }

        public static object[] CopyToObject<T>(this T[] source, Type resultType)
        {
            List<object> result = new List<object>();

            foreach (T item in source)
            {
                var instance = Activator.CreateInstance(resultType);

                result.Add(item.CopyToObject(instance));
            }

            return result.ToArray();
        }

        public static void SetPropertyValue<T>(this T source, string name, object value) //, bool converter = true)
        {
            Type obj = source.GetType();

            PropertyInfo info = obj.GetProperty(name);

            if (info == null)
            {
                return;
            }

            try
            {
                //Type propertyType = info.PropertyType;

                //Type target = propertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;

                //value = Convert.ChangeType(value, target);

                info.SetValue(source, value, null);
            }
            catch
            {
                // DO NOTHING: This may be because the property was not initialized and is NULL
                throw;
            }
        }

        public static object GetPropertyValue<T>(this T source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type obj = source.GetType();

            PropertyInfo info = obj.GetProperty(name);

            if (info == null)
            {
                return null;
            }

            try
            {
                return info.GetValue(source, null);
            }
            catch
            {
                return null;
            }
        }

        public static object InvokeMethod<T>(this T source, object instance, string method, object[] args, bool throwIfnoMethod = true)
        {
            Type instanceType = instance.GetType();

            if (!args.HasElements())
            {
                MethodInfo inf = instanceType.GetMethod(method);

                if (inf == null)
                {
                    if (throwIfnoMethod)
                    {
                        throw new MissingMethodException($"{method} not found");
                    }

                    return null;
                }

                return inf.Invoke(instance, new object[] { });
            }

            Type[] argumentTypes = new Type[args.Length];

            for (int x = 0; x < args.Count(); ++x)
            {
                if (args[x] == null)
                {
                    argumentTypes[x] = null;

                    continue;
                }

                argumentTypes[x] = args[x].GetType();
            }

            MethodInfo spesificInfo = instanceType.GetMethod(method, argumentTypes);

            if (spesificInfo == null)
            {
                if (throwIfnoMethod)
                {
                    throw new MissingMethodException($"{method} not found");
                }

                return null;
            }

            return spesificInfo.Invoke(instance, args);
        }

        public static object InvokeMethod<T>(this T source, string assembly, string method, object[] args, bool throwIfnoMethod = true)
        {
            Type objectType = Type.GetType(assembly);

            if (objectType == null)
            {
                throw new ApplicationException($"Method {method} or Object not found");
            }

            ConstructorInfo contructor = objectType.GetConstructor(Type.EmptyTypes);

            object controller = contructor.Invoke(null);

            if (!args.HasElements())
            {
                MethodInfo inf = objectType.GetMethod(method);

                if (inf == null)
                {
                    if (throwIfnoMethod)
                    {
                        throw new MissingMethodException($"{method} not found");
                    }

                    return null;
                }

                return inf.Invoke(controller, new object[] { });
            }

            Type[] argumentTypes = new Type[args.Length];

            for (int x = 0; x < args.Count(); ++x)
            {
                argumentTypes[x] = args[x].GetType();
            }

            MethodInfo spesificInfo = objectType.GetMethod(method, argumentTypes);

            if (spesificInfo == null)
            {
                if (throwIfnoMethod)
                {
                    throw new MissingMethodException($"{method} not found");
                }

                return null;
            }

            return spesificInfo.Invoke(controller, args);
        }

        public static bool ContainsValue<T>(this T source, string value, DataComparisonEnum compareType, bool removeSpaces = false)
        {
            if (value.IsNullEmptyOrWhiteSpace() || source == null)
            {
                return false;
            }

            var sourceT = source.GetType();

            value = value.ToLower();

            if (removeSpaces)
            {
                value = value.Replace(" ", string.Empty);
            }

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                if (!item.CanRead)
                {
                    continue;
                }

                try
                {
                    string objVal = item.GetValue(source, null).ParseToString().ToLower();

                    if (objVal.IsNullEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    if (removeSpaces)
                    {
                        objVal = objVal.Replace(" ", string.Empty);
                    }

                    switch (compareType)
                    {
                        case DataComparisonEnum.None:

                            string[] valSplit = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string valItem in valSplit)
                            {
                                if (objVal.Contains(valItem))
                                {
                                    return true;
                                }
                            }

                            break;

                        case DataComparisonEnum.Contains:

                            if (objVal.Contains(value))
                            {
                                return true;
                            }

                            break;

                        case DataComparisonEnum.StartsWith:

                            if (objVal.StartsWith(value))
                            {
                                return true;
                            }

                            break;

                        case DataComparisonEnum.EndsWith:

                            if (objVal.EndsWith(value))
                            {
                                return true;
                            }

                            break;

                        case DataComparisonEnum.Exact:

                            if (objVal == value)
                            {
                                return true;
                            }

                            break;
                    }

                }
                catch
                {
                    // Do Nothing
                }
            }

            return false;
        }
    }
}
