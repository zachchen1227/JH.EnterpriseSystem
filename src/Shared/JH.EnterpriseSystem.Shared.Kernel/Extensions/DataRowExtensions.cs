
using System.Data;
using Microsoft.Data.SqlClient;

namespace JH.EnterpriseSystem.Shared.Kernel.Extensions
{
    #region Extension 類別
    /// <summary>
    /// Extension 類別
    /// </summary>
    public static class DatabaseExtensions
    {

        /// <summary>
        /// SqlDataAdapter塞入資料
        /// </summary>
        /// <param name="da"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable FillDataTable(this SqlDataAdapter da, DataTable dt)
        {
            da.Fill(dt);
            return dt;
        }
        #region 具名的DataSet塞入資料
        /// <summary>
        /// 具名的DataTable塞入資料
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="TableName">TableName</param>
        /// <returns></returns>
        public static DataSet FillDataSet(this SqlCommand cmd, string DsName)
        {
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
            catch
            {
                throw;
            }

        }
        #endregion
        #region 具名的DataTable塞入資料
        /// <summary>
        /// 具名的DataTable塞入資料
        /// </summary>
        /// <param name="cmd">SqlCommand</param>
        /// <param name="TableName">TableName</param>
        /// <returns></returns>
        public static DataTable FillDataTable(this SqlCommand cmd, string TableName)
        {
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable(TableName))
                    {
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch
            {
                throw;
            }

        }


        #region 轉型成double
        /// <summary>
        /// 轉型成double
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static double ToDouble(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                {
                    return 0;
                }
                return Convert.ToDouble(o.ToString());
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉型成int
        /// <summary>
        /// 轉型成int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int ToInt(this object o, int? Spec = null)
        {
            try
            {
                if (o.IsNullOrEmpty())
                {
                    if (Spec == null)
                        return 0;
                    else
                        return (int)Spec;
                }
                return Convert.ToInt32(o.ToString());
            }
            catch
            {
                throw;
            }
        }
        public static short ToInt16(this object o, int? Spec = null)
        {
            try
            {
                if (o.IsNullOrEmpty())
                {
                    if (Spec == null)
                        return 0;
                    else
                        return (short)Spec;
                }
                return Convert.ToInt16(o.ToString());
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉型Bool轉Strng
        /// <summary>
        /// 轉型Bool轉Strng
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string BoolToYN(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                    return "N";
                return "Y";
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉型成 bool
        /// <summary>
        /// 轉型成int
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool Tobool(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                    return false;
                return Convert.ToBoolean(o.ToString());
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 判斷NullOrEmpty
        /// <summary>
        /// 判斷NullOrEmpty
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object o)
        {
            try
            {
                // return  string.IsNullOrEmpty(o.ToString());  
                return (o == null || o.ToString().Length == 0 || Convert.IsDBNull(o));

            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉字串順便去掉空白
        /// <summary>
        /// 轉字串順便去掉空白
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToStringTrim(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                    return "";
                return o.ToString().Trim();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉換成yyyy/MM/dd HH:mm 格式
        /// <summary>
        /// 轉換成yyyy/MM/dd HH:mm 格式
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string toyyyyMMddHHmm(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                    return "";
                return ((DateTime)o).ToString("yyyy/MM/dd HH:mm");
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉換成yyyy/MM/dd HH:mm 格式
        /// <summary>
        /// 轉換成yyyy/MM/dd HH:mm 格式
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string toyyMMddHHmm(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                    return "";
                return ((DateTime)o).ToString("yyMMddHHmm");
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉換成yyyy/MM/dd格式
        public static string toyyyyMMdd(this object o)
        {
            try
            {
                if (o.IsNullOrEmpty())
                    return "";
                return ((DateTime)o).ToString("yyyy/MM/dd");
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region 轉型成DateTime
        /// <summary>
        /// 轉型成DateTime
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object o)
        {
            try
            {
                if (o == null || o.ToString() == "" || o.IsNullOrEmpty())
                    return DateTime.MinValue;
                else
                    return Convert.ToDateTime(o.ToString());
            }
            catch
            {
                throw;
            }
        }

        #endregion
        public static string TrimEnd(this object source, string value)
        {
            if (!source.ToString().EndsWith(value))
                return source.ToString();

            return source.ToString().Remove(source.ToString().LastIndexOf(value));
        }
        public static bool IsNumeric(this object Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;
            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;
            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }
        public static bool IsDate(this Object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime dt = DateTime.Parse(strDate);
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

    }
    #endregion
}
#endregion