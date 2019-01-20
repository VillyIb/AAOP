
// ReSharper disable TailRecursiveCall

namespace eu.iamia.Util.Extensions
{
    using System;

    public static class TypeExtension
    {
        /// <summary>
        /// Determines if a type is Enum.
        /// Nullable Enum types are considered Enum.
        /// </summary>
        /// <remarks>Must be tested before testing IsNummericType because Enum is also a NumericType</remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            var typecode = Type.GetTypeCode(type);

            if (type.IsEnum)
            {
                // Enum es extended from a nummeric type but are NOT to be treated as a NummericType.
                return true;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (typecode)
            {
                case TypeCode.Object:
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            return IsEnumType(Nullable.GetUnderlyingType(type));
                        }
                        return false;
                    }

                default:
                    {
                        return false;
                    }

            }
        }


        /// <summary>
        /// Determines if a type is numeric.
        /// Nullable numeric types are considerend numeric.
        /// </summary>
        /// <remarks>Boolean is not considered numeric.</remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNummericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            var typecode = Type.GetTypeCode(type);

            switch (typecode)
            {
                case TypeCode.Empty:
                    return false;

                case TypeCode.Object:
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            return IsNummericType(Nullable.GetUnderlyingType(type));
                        }
                        return false;
                    }

                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                    return false;

                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;

                case TypeCode.DateTime:
                case TypeCode.String:
                    return false;

                default:
                    return false;
            }
        }


        public static bool IsDateTime(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            var typecode = Type.GetTypeCode(type);

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (typecode)
            {
                case TypeCode.Object:
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            return IsDateTime(Nullable.GetUnderlyingType(type));
                        }
                        return false;
                    }

                case TypeCode.DateTime:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    }
}
