namespace eu.iamia.Util.Extensions
{
    using System.Linq;

    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns true if all of the provided parameters has a value (!= null).
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool HasValue(this object value, params object[] args)
        {
            if (value == null) return false;
            return args != null && args.All(t => t != null); 
        }


        /// <summary>
        /// Returns true if the provided value is null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(this object value)
        {
            return value == null;
        }


        /// <summary>
        /// Returns true if all of the provided parameters are null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsNull(this object value, params object[] args)
        {
            if (value != null) { return false;}
            return args == null || args.All(t => t == null);
        }
    }
}
