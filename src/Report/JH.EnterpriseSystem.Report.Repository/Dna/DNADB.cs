using System.Data;
using Microsoft.Data.SqlClient;
using JH.EnterpriseSystem.Shared.Kernel.Extensions;

namespace JH.EnterpriseSystem.Report.Repository.Dna
{
    public class DNADB
    {

        public enum MyEnum
        {
            RemoteString02,
            RemoteString03
        }

        #region SQL 連線

        //private const string RemoteString02 = "Data Source=192.168.0.2;Initial Catalog=DWReport;Persist Security Info=True;User ID=finsi;Password=Simis*2020";
        private const string RemoteString02 = "Data Source=dev-tw;Initial Catalog=DWReport;Persist Security Info=True;User ID=twuser;Password=tw@tw";
        private const string RemoteString30 = "Data Source=192.168.3.20;Initial Catalog=FormulationWeighingSystem;User ID=tw;Password=Jiahsin.123;TrustServerCertificate=true;MultipleActiveResultSets=True";

        public static SqlConnection Conn3(MyEnum ME = MyEnum.RemoteString03)
        {
            try
            {
                string RemoteString = string.Empty;
                switch (ME)
                {
                    case MyEnum.RemoteString02:
                        RemoteString = RemoteString02;
                        break;

                    default:
                        RemoteString = RemoteString30;
                        break;
                }
                SqlConnection sqlconn = new SqlConnection(RemoteString);
                return sqlconn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static SqlConnection Conn2(MyEnum ME = MyEnum.RemoteString02)
        {
            try
            {
                string RemoteString = string.Empty;
                switch (ME)
                {
                    case MyEnum.RemoteString02:
                        RemoteString = RemoteString02;
                        break;

                    default:
                        RemoteString = RemoteString30;
                        break;
                }
                SqlConnection sqlconn = new SqlConnection(RemoteString);
                return sqlconn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion SQL 連線

        #region 取得共用報表資料Type1

        /// <summary>
        /// 取得共用報表資料
        /// </summary>
        /// <param name="ReportName">報表名稱</param>
        /// <param name="Sday">查詢開始日期</param>
        /// <param name="Eday">查詢結束日期</param>
        /// <returns></returns>
        public DataTable GetCommonRPTT1(string ReportName, string Sday, string Eday)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Eday", SqlDbType = SqlDbType.Date, Value=  Eday },
                 new SqlParameter() { ParameterName = "@Sday", SqlDbType = SqlDbType.Date, Value=  Sday },
             };
            return GetDataTable(@"SELECT
                                         *
                                    FROM
                                         " + ReportName +
                                @" WHERE StockFlowDate>=@Sday AND
                                         StockFlowDate<=@Eday
                                ORDER BY StockFlowDate asc", sp);
        }

        /// <summary>
        /// 取得配方資料
        /// </summary>
        /// <param name="FormulaName"></param>
        /// <returns></returns>
        public static DataTable GetFormulaName(string FormulaName)
        {
            //cmd.CommandText = "select Top 10 Formula from vw_FormulaData where Formula LIKE ''+@SearchFormulaName+'%'";
            //cmd.Connection = con;
            //con.Open();
            //cmd.Parameters.AddWithValue("@SearchFormulaName", FormulaName);
            //SqlDataReader dr = cmd.ExecuteReader();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@SearchFormulaName", SqlDbType = SqlDbType.VarChar,Size=20, Value=  FormulaName },
             };
            return GetDataTable(@"select Top 10 Formula from vw_FormulaData where Formula LIKE '%'+@SearchFormulaName+'%'", sp, false);
        }

        //public static void SetMaterialOrder(List<OrderItem> orders)
        //{
        //    foreach (OrderItem item in orders)
        //    {
        //        List<SqlParameter> sp = new List<SqlParameter>()
        //     {
        //         new SqlParameter() { ParameterName = "@sort", SqlDbType = SqlDbType.SmallInt, Value=  item.Position },
        //         new SqlParameter() { ParameterName = "@gKey", SqlDbType = SqlDbType.VarChar, Size=6, Value=  item.Id },
        //      };
        //        ExecuteNonQuery(@"UPDATE
        //                                  MatrialSort
        //                              Set sort=@sort
        //                            where gKey=@gKey", sp, false);
        //    }
        //}

        public static void SetMaterialWeightRange(string Up, string Down, string Key)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
             {
                  new SqlParameter() { ParameterName = "@Wup", SqlDbType = SqlDbType.SmallInt, Value=  Up },
                 new SqlParameter() { ParameterName = "@WDown", SqlDbType = SqlDbType.SmallInt, Value= Down },
                 new SqlParameter() { ParameterName = "@gKey", SqlDbType = SqlDbType.VarChar, Size=6, Value=  Key },
              };
            ExecuteNonQuery(@"UPDATE
                                          MatrialSort
                                      Set Wup=@Wup,
                                          Wdown=@Wdown
                                    where gKey=@gKey", sp, false);
        }

        /// <summary>
        /// 取得配方清單
        /// </summary>
        /// <param name="CodePrefix">配方代號</param>
        /// <returns></returns>
        public static DataTable GetMaterial(string CodePrefix)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@CodePrefix", SqlDbType = SqlDbType.VarChar,Size=30, Value=  CodePrefix },
             };
            return GetDataTable(@"SELECT  *
                                    From 　
                                          vw_MaterialData
                                   Where  CodePrefix=@CodePrefix　
                                Order by  Sort asc,
                                          MaterialName asc", sp, false);
        }

        public static DataTable GetOrder(string Code, string CodePrefix,
                                               string Color, string MType,
                                               string Name, bool Datelimit)
        {
            try
            {
                //string sql = "SELECT * FROM vw_OrderStatus WHERE OrderNo = @OrderNo AND MaterialName = @MaterialName AND MaterialCode = @MaterialCode AND CustomColor = @CustomColor";
                string sql = "SELECT Code ," +
                    "                Name ," +
                    "                CodePrefix," +
                    "                Color," +
                    "                MType," +
                    "                Acount," +
                    "                BatchCount," +
                    "                cast(Acount as VARCHAR)+'/' +cast(BatchCount as VARCHAR) as Status " +
                    "           FROM vw_Oder " +
                    "          Where BatchCount<>Acount ";
                List<SqlParameter> sps = new List<SqlParameter>() { };
                if (!string.IsNullOrEmpty(Code))
                {
                    sql += " And Code LIKE '%@Code%'";
                    sps.Add(new SqlParameter() { ParameterName = "@Code", SqlDbType = SqlDbType.VarChar, Size = 20, Value = Code });
                }
                if (!string.IsNullOrEmpty(CodePrefix))
                {
                    sql += " And CodePrefix LIKE '%@CodePrefix%'";
                    sps.Add(new SqlParameter() { ParameterName = "@CodePrefix", SqlDbType = SqlDbType.NVarChar, Size = 25, Value = CodePrefix });
                }
                if (!string.IsNullOrEmpty(Color))
                {
                    sql += " And Color LIKE '%@Color%'";
                    sps.Add(new SqlParameter() { ParameterName = "@Color", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = Color });
                }
                if (!string.IsNullOrEmpty(MType))
                {
                    sql += " And MType =@MType";
                    sps.Add(new SqlParameter() { ParameterName = "@MType", SqlDbType = SqlDbType.NVarChar, Size = 2, Value = MType });
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    sql += " And Name LIKE '%@Name%'";
                    sps.Add(new SqlParameter() { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Size = 50, Value = Name });
                }
                if (Datelimit)
                {
                    sql += " And CreateDate>=DATEADD(DAY,-15, GETDATE())";
                }
                return GetDataTable(sql, sps, false);
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        /// <summary>
        /// 取得假日資料
        /// </summary>
        /// <param name="Sday">開始時間</param>
        /// <param name="Eday">結束時間</param>
        /// <returns></returns>
        public DataTable GetHoliday(string Sday, string Eday)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Eday", SqlDbType = SqlDbType.Date, Value=  Eday },
                 new SqlParameter() { ParameterName = "@Sday", SqlDbType = SqlDbType.Date, Value=  Sday },
             };
            return GetDataTable(@"select  CONVERT(varchar(10),SpecifiedDate,111) as Holiday
                                    From Holiday WHERE   SpecifiedDate>=@Sday and SpecifiedDate<=@Eday", sp, false);
        }

        #endregion 取得共用報表資料Type1

        #region 取得共用報表資料Type2

        /// <summary>
        /// 取得RPT25002報表資料
        /// </summary>
        /// <param name="QueryDay">查詢日期</param>
        /// <returns></returns>
        public DataTable GetCommonRPTT2(string ReportName, string QueryDay)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@QueryDay", SqlDbType = SqlDbType.Date, Value=  QueryDay }
             };
            return GetDataTable(@"SELECT
                                          FID,
                                          DataType,
                                          Replace(Replace(WorkTeamName,'BU1 ',''),'BU2 ','') as WorkTeamName,
                                          cast(isnull(TotalCount,0) AS int) as TotalCount
                                    FROM
                                          " + ReportName +
                                @" WHERE  StockFlowDate=@QueryDay
                                     AND  WorkTeamName <> N'BU1 Printing'
                                ORDER BY  WorkTeamName asc", sp);
        }

        public static DataTable GetCommonRPTT2_1(string ReportName, string QueryDay)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@QueryDay", SqlDbType = SqlDbType.Date, Value=  QueryDay }
             };
            return GetDataTable(@"SELECT
                                          FID,
                                          DataType,
                                          Replace(Replace(WorkTeamName,'BU1 ',''),'BU2 ','') as WorkTeamName,
                                          cast(isnull(TotalCount,0) AS int) as TotalCount
                                    FROM
                                          " + ReportName +
                                @" WHERE  StockFlowDate=@QueryDay
                                    AND   WorkTeamName = N'BU1 Printing'
                                ORDER BY  WorkTeamName asc", sp);
        }

        public static DataTable GetRPT25003(string Sday, string Eday)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Sday", SqlDbType = SqlDbType.Date, Value=  Sday },
                  new SqlParameter() { ParameterName = "@Eday", SqlDbType = SqlDbType.Date, Value=  Eday }
             };
            return GetDataTable(@"SELECT  Fid,
                                          StockFlowDate,
		                                  cast(ROUND(ProductionQty*100.0/(ProductionQty+DefectQty),1) AS DECIMAL(4,1))  AS RFT
                                   FROM
                                          (
	                                            SELECT　 Fid,
	　　　　                                             StockFlowDate,
	　　　　                                             sum(ProductionQty) AS ProductionQty,sum(DefectQty) as DefectQty
	                                             FROM 　 RPT25003
	                                            WHERE 　 StockFlowDate>=@Sday
	　　　　                                             AND
	　　　　                                             StockFlowDate<=@Eday
                                             GROUP BY    Fid,StockFlowDate
                                         ) x
                                             ORDER BY    StockFlowDate asc", sp);
        }

        public static DataTable GetRPT25004(string QueryDay)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@QueryDay", SqlDbType = SqlDbType.Date, Value=  QueryDay }
             };
            return GetDataTable(@"SELECT
                                          Fid,
                                           Replace(Replace(WorkTeamName,'BU1',''),'BU2','') as WorkTeamName ,
                                          cast(ROUND(ProductionQty*100.0/(ProductionQty+DefectQty),1) AS DECIMAL(4,1)) AS RFT
                                   FROM
                                          RPT25003
                                  WHERE
                                          StockFlowDate=@QueryDay
                                ORDER BY  WorkTeamName asc", sp);
        }

        #endregion 取得共用報表資料Type2

        #region 取得總表SQL

        //--GetDataSet
        public static DataTable GetRPT25013(string BU, string Sday, string Eday)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@bu", SqlDbType = SqlDbType.VarChar, Value =  BU },
                 new SqlParameter() { ParameterName = "@Sday", SqlDbType = SqlDbType.Date, Value=  Sday },
                 new SqlParameter() { ParameterName = "@Eday", SqlDbType = SqlDbType.Date, Value=  Eday },
             };
            return GetDataTable(@"SELECT DataDate, UpperTotal, StockFitTotal, SoleTotal, MatchPairTotal, PrintTotal, MatchRate
                                 FROM vw_RPT25013
                                WHERE  BU = @bu
                                  AND DataDate >= @Sday and DataDate <= @Eday
                                  AND DataDate not in (SELECT SpecifiedDate FROM Holiday WHERE SpecifiedDate >= @Sday and SpecifiedDate <= @Eday)
                             ORDER BY DataDate", sp);
        }

        public static DataSet GetRPT25014(string Target_date, string BU)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Target_date", SqlDbType = SqlDbType.VarChar, Value=  Target_date },
                 new SqlParameter() { ParameterName = "@Fid", SqlDbType = SqlDbType.VarChar, Value=  BU },
             };
            return GetDataSet("usp_GetRPT25014", sp, true);
        }

        public static DataSet GetCommonRPTTAll(string Sday, string Eday)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Eday", SqlDbType = SqlDbType.Date, Value=  Eday },
                 new SqlParameter() { ParameterName = "@Sday", SqlDbType = SqlDbType.Date, Value=  Sday },
             };

            return GetDataSet("usp_QryReportALLTemp", sp, true);
        }

        #endregion 取得總表SQL

        #region 登入INPUT系統

        public static DataTable GetAccount(string tAccount, string Functiontype)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Account", SqlDbType = SqlDbType.VarChar, Value=  tAccount },
                 new SqlParameter() { ParameterName = "@Functiontype", SqlDbType = SqlDbType.VarChar, Size=1 ,Value=  Functiontype },
                 // new SqlParameter() { ParameterName = "@Password", SqlDbType = SqlDbType.VarChar, Value=  tPassword }
             };
            //-- AND pwd=@Password
            return GetDataTable(@"SELECT * FROM vwt_AccountFunList WHERE acc=@Account and  Functiontype=@Functiontype", sp);
        }

        public static DataTable GetAccount(string tAccount)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Account", SqlDbType = SqlDbType.VarChar, Value=  tAccount },

                 // new SqlParameter() { ParameterName = "@Password", SqlDbType = SqlDbType.VarChar, Value=  tPassword }
             };
            //-- AND pwd=@Password
            //return GetDataTable(@"SELECT * FROM vwt_AccountFunList WHERE acc=@Account", sp);

            // 2025/11/18 新增Order By Functiontype
            return GetDataTable(@"SELECT * FROM vwt_AccountFunList WHERE acc=@Account ORDER BY Functiontype", sp);
        }

        #endregion 登入INPUT系統

        #region 生產線排程作業

        //--取得成型資料
        public static DataSet GetWorkShopTeam(string Target_date)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Target_date", SqlDbType = SqlDbType.VarChar, Value=  Target_date }
             };
            return GetDataSet("usp_GetWorkShopTeam", sp, true);
        }

        //--取得成型資料 - 組底、針車、印刷、截斷
        public static DataSet GetWorkShopTeam2(string Target_date, string tFType)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Target_date", SqlDbType = SqlDbType.VarChar, Value=  Target_date },
                 new SqlParameter() { ParameterName = "@FunctionType", SqlDbType = SqlDbType.Char, Value=  tFType }
             };
            //return GetDataSet("usp_GetWorkShopTeam2", sp, true);
            return GetDataSet("usp_GetWorkShopTeam2", sp, true); // 2025/11/13 Una - 測試機暫時使用
        }

        //--取得已設定目標資料

        public static DataTable GetDayTarget(string tTarget_date)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@Target_date", SqlDbType = SqlDbType.VarChar, Value=  tTarget_date }
             };
            return GetDataTable("SELECT * FROM DayTarget WHERE CONVERT(varchar(100), Target_date, 111)=CONVERT(varchar(100), @Target_date, 111)", sp, false);
        }

        //--取下個工作日
        public static DateTime GetNextWorkDay()
        {
            object obj = ExecuteScal("SELECT [dbo].[ufn_GetNextWorkDay]()", null, false);
            return (DateTime)obj;
        }

        //--取上個工作日
        public static DateTime GetPreWorkDay()
        {
            object obj = ExecuteScal("SELECT [dbo].[ufn_GetPreWorkDay]()", null, false);
            return (DateTime)obj;
        }

        //--儲存資料 - 成型
        public static string SetWorkShopTeam(string TeamID, string Target_date, string Production_date, string created_by
            , string TimeOnOff1, string TimeOnOff2, string TimeOnOff3, string TimeOnOff4, string TimeOnOff5, string TimeOnOff6
            , string TimeOnOff7, string TimeOnOff8, string TimeOnOff9, string TimeOnOff10, string TimeOnOff11, string TimeOnOff12
            , string TimeOnOff13, string TimeOnOff14, string TimeOnOff15, string TimeOnOff16, string TimeOnOff17, string TimeOnOff18
            , string TargetQty1, string TargetQty2, string TargetQty3, string TargetQty4, string TargetQty5, string TargetQty6, string TargetQty7
            , string TargetQty8, string TargetQty9, string TargetQty10, string TargetQty11, string TargetQty12, string TargetQty13
            , string TargetQty14, string TargetQty15, string TargetQty16, string TargetQty17, string TargetQty18)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@TeamID", SqlDbType = SqlDbType.VarChar, Value=  TeamID },
                new SqlParameter() { ParameterName = "@Target_date", SqlDbType = SqlDbType.VarChar, Value=  Target_date },
                new SqlParameter() { ParameterName = "@Production_date", SqlDbType = SqlDbType.VarChar, Value=  Production_date },
                new SqlParameter() { ParameterName = "@created_by", SqlDbType = SqlDbType.VarChar, Value=  created_by },

                new SqlParameter() { ParameterName = "@TimeOnOff1", SqlDbType = SqlDbType.Char, Value=  TimeOnOff1.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty1", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty1))? (object)DBNull.Value:TargetQty1 },

                new SqlParameter() { ParameterName = "@TimeOnOff2", SqlDbType = SqlDbType.Char, Value=  TimeOnOff2.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty2", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty2))? (object)DBNull.Value:TargetQty2 },

                new SqlParameter() { ParameterName = "@TimeOnOff3", SqlDbType = SqlDbType.Char, Value=  TimeOnOff3.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty3", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty3))? (object)DBNull.Value:TargetQty3 },

                new SqlParameter() { ParameterName = "@TimeOnOff4", SqlDbType = SqlDbType.Char, Value=  TimeOnOff4.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty4", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty4))? (object)DBNull.Value:TargetQty4 },

                new SqlParameter() { ParameterName = "@TimeOnOff5", SqlDbType = SqlDbType.Char, Value=  TimeOnOff5.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty5", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty5))? (object)DBNull.Value:TargetQty5 },

                new SqlParameter() { ParameterName = "@TimeOnOff6", SqlDbType = SqlDbType.Char, Value=  TimeOnOff6.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty6", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty6))? (object)DBNull.Value:TargetQty6 },

                new SqlParameter() { ParameterName = "@TimeOnOff7", SqlDbType = SqlDbType.Char, Value=  TimeOnOff7.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty7", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty7))? (object)DBNull.Value:TargetQty7 },

                new SqlParameter() { ParameterName = "@TimeOnOff8", SqlDbType = SqlDbType.Char, Value=  TimeOnOff8.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty8", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty8))? (object)DBNull.Value:TargetQty8 },

                new SqlParameter() { ParameterName = "@TimeOnOff9", SqlDbType = SqlDbType.Char, Value=  TimeOnOff9.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty9", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty9))? (object)DBNull.Value:TargetQty9 },

                new SqlParameter() { ParameterName = "@TimeOnOff10", SqlDbType = SqlDbType.Char, Value=  TimeOnOff10.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty10", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty10))? (object)DBNull.Value:TargetQty10 },

                new SqlParameter() { ParameterName = "@TimeOnOff11", SqlDbType = SqlDbType.Char, Value=  TimeOnOff11.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty11", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty11))? (object)DBNull.Value:TargetQty11 },

                new SqlParameter() { ParameterName = "@TimeOnOff12", SqlDbType = SqlDbType.Char, Value=  TimeOnOff12.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty12", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty12))? (object)DBNull.Value:TargetQty12 },

                new SqlParameter() { ParameterName = "@TimeOnOff13", SqlDbType = SqlDbType.Char, Value=  TimeOnOff13.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty13", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty13))? (object)DBNull.Value:TargetQty13 },

                new SqlParameter() { ParameterName = "@TimeOnOff14", SqlDbType = SqlDbType.Char, Value=  TimeOnOff14.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty14", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty14))? (object)DBNull.Value:TargetQty14 },

                new SqlParameter() { ParameterName = "@TimeOnOff15", SqlDbType = SqlDbType.Char, Value=  TimeOnOff15.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty15", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty15))? (object)DBNull.Value:TargetQty15 },

                new SqlParameter() { ParameterName = "@TimeOnOff16", SqlDbType = SqlDbType.Char, Value=  TimeOnOff16.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty16", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty16))? (object)DBNull.Value:TargetQty16 },

                new SqlParameter() { ParameterName = "@TimeOnOff17", SqlDbType = SqlDbType.Char, Value=  TimeOnOff17.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty17", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty17))? (object)DBNull.Value:TargetQty17 },

                new SqlParameter() { ParameterName = "@TimeOnOff18", SqlDbType = SqlDbType.Char, Value=  TimeOnOff18.Trim() },
                new SqlParameter() { ParameterName = "@TargetQty18", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetQty18))? (object)DBNull.Value:TargetQty18}
             };

            object obj = ExecuteScal("usp_SetWorkShopTeam", sp, true);

            return obj.ToString();
        }

        //--儲存資料 - 組底、針車、印刷、截斷
        public static string SetWorkShopTeam2(string TeamID, string Target_date, string created_by, string TargetTotal)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@TeamID", SqlDbType = SqlDbType.VarChar, Value=  TeamID },
                new SqlParameter() { ParameterName = "@Target_date", SqlDbType = SqlDbType.VarChar, Value=  Target_date },
                new SqlParameter() { ParameterName = "@created_by", SqlDbType = SqlDbType.VarChar, Value=  created_by },
                new SqlParameter() { ParameterName = "@TargetTotal", SqlDbType = SqlDbType.Int, Value=  (string.IsNullOrEmpty(TargetTotal))? (object)DBNull.Value:TargetTotal}
             };

            object obj = ExecuteScal("usp_SetWorkShopTeam2", sp, true);

            return obj.ToString();
        }

        #endregion 生產線排程作業

        #region 取得SQL資料函數

        #region 取得資料

        public static DataTable GetDataTable(string Sql, SqlCommand cmd, string TableName)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    //using (SqlCommand cmd = new SqlCommand())
                    //{
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = Sql;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable(TableName))
                        {
                            da.Fill(dt);
                            return dt;
                        }
                    }
                    //}
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        #endregion 取得資料

        #region 跑SQL指令

        public static void ExecuteNonQuery(string Sql, SqlCommand cmd)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    conn.Open();
                    //  using (SqlCommand cmd = new SqlCommand())
                    //   {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = Sql;
                    cmd.ExecuteNonQuery();
                    //       }
                    conn.Close();
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        public static void ExecuteNonQuery(string Sql, List<SqlParameter> SPs, bool isSp = false)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        if (isSp)
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        else
                            cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = Sql;
                        if (SPs != null)
                            cmd.Parameters.AddRange(SPs.ToArray());
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        #endregion 跑SQL指令

        #region 回存執行單一結果

        /// <summary>
        ///  回存執行單一結果
        /// </summary>
        /// <param name="Sql">SQL 命令</param>
        /// <param name="SPs">參數</param>
        /// <param name="isSp">是否預存程序</param>
        /// <returns></returns>
        public static object ExecuteScal(string Sql, List<SqlParameter> SPs, bool isSp = false)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        if (isSp)
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        else
                            cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = Sql;
                        if (SPs != null)
                            cmd.Parameters.AddRange(SPs.ToArray());
                        conn.Open();
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        #endregion 回存執行單一結果

        #region SQL命令執行並回傳參數

        /// <summary>
        /// SQL命令執行並回傳參數
        /// </summary>
        /// <param name="Sql">執行SQL命令</param>
        /// <param name="SPs">參數</param>
        /// <param name="OutputName">Output參數名稱</param>
        /// <param name="isSp">是否是預存程序</param>
        /// <returns></returns>
        public static string ExecuteNonQuery(string Sql, List<SqlParameter> SPs, string OutputName, bool isSp = false)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        if (isSp)
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        else
                            cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = Sql;
                        if (SPs != null)
                            cmd.Parameters.AddRange(SPs.ToArray());
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return cmd.Parameters["@" + OutputName].Value.ToString();
                    }
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        #endregion SQL命令執行並回傳參數

        #region 取得DataSet資料

        //取得DataSet
        /// <summary>
        /// 取得DataTable
        /// </summary>
        /// <param name="Sql">SQL命令</param>
        /// <param name="SPs">參數集</param>
        /// <param name="isSp">是否預存程序</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string Sql, List<SqlParameter> SPs, bool isSp = false)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        if (isSp)
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        else
                            cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = Sql;
                        if (SPs != null)
                            cmd.Parameters.AddRange(SPs.ToArray());
                        return cmd.FillDataSet("temp");
                    }
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        #endregion 取得DataSet資料

        #region 取得DataTable

        /// <summary>
        /// 取得DataTable
        /// </summary>
        /// <param name="Sql">SQL命令</param>
        /// <param name="SPs">參數集</param>
        /// <param name="isSp">是否預存程序</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string Sql, List<SqlParameter> SPs, bool isSp = false)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        if (isSp)
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        else
                            cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = Sql;
                        if (SPs != null)
                            cmd.Parameters.AddRange(SPs.ToArray());
                        DataTable dt = cmd.FillDataSet("temp").Tables[0];
                        dt.TableName = Guid.NewGuid().ToString("N").Substring(0, 5);
                        return dt;
                    }
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        #endregion 取得DataTable

        #endregion 取得SQL資料函數

        #region 2025/11/12 HR報表

        //公司層
        public  DataTable GetHR_CompanyDaily(
            string queryDay,
            string orgGroupId = null,
            string parentBuId = null,
            string divisionId = null,
            string departmentId = null,
            string sectionId = null,
            string teamId = null)
        {
            var sp = new List<SqlParameter>
            {
                new SqlParameter("@QueryDay", SqlDbType.Date) { Value = queryDay }
            };

            string whereConditions = "r.AttendanceDate = @QueryDay \n";
            string selectCodeColumn = "r.OrgGroupId";
            string selectNameColumn = "r.OrgGroupId";
            string groupByNameColumn = "r.OrgGroupId";

            // 定義組織層級及其對應的欄位資訊
            var orgLevels = new List<(string paramValue, string codeColumn, string nameColumn, string paramName)>
            {
                (orgGroupId,      "OrgGroupId",     "OrgGroupId",      "@OrgGroupId"),
                (parentBuId,      "ParentBuId",     "ParentBuId",      "@ParentBuId"),
                (divisionId,      "DivisionId",     "DivisionName",    "@DivisionId"),
                (departmentId,    "DepartmentId",   "DepartmentName",  "@DepartmentId"),
                (sectionId,       "SectionId",      "SectionName",     "@SectionId"),
                (teamId,          "TeamId",         "TeamName",        "@TeamId")
            };

            // 找出最小的傳入層級
            (string paramValue, string codeColumn, string nameColumn, string paramName) deepestLevel = default;

            foreach (var level in orgLevels)
            {
                if (!string.IsNullOrWhiteSpace(level.paramValue))
                {
                    // 加入 WHERE 條件
                    sp.Add(new SqlParameter(level.paramName, SqlDbType.VarChar) { Value = level.paramValue });
                    whereConditions += $" AND r.{level.codeColumn} = {level.paramName} \n";

                    // 更新最細層級
                    deepestLevel = level;
                }
            }

            // 根據最小層級設定 SELECT 和 GROUP BY 欄位
            if (deepestLevel.paramValue != null)
            {
                int index = orgLevels.IndexOf(deepestLevel);

                // 下一級就是 index + 1
                if (index + 1 < orgLevels.Count)
                {
                    var nextLevel = orgLevels[index + 1];
                    selectCodeColumn = $"r.{nextLevel.codeColumn}";

                    // 決定名稱欄位是 OrgRef 還是 RPT25015
                    if (nextLevel.nameColumn.EndsWith("Name"))
                    {
                        selectNameColumn = $"rorg.{nextLevel.nameColumn}";
                        groupByNameColumn = $"r.{nextLevel.codeColumn}, rorg.{nextLevel.nameColumn}";
                    }
                    else
                    {
                        // 使用 r (RPT25015) 的 ID 欄位作為名稱 (因為沒有對應 Name)
                        selectNameColumn = $"r.{nextLevel.codeColumn}";
                        groupByNameColumn = $"r.{nextLevel.codeColumn}";
                    }
                }
                else
                {
                    // 如果傳入最深層級 TeamId，則不再分組，只顯示當前層級的資料總計
                    // 讓 SELECT/GROUP BY 維持在目前的 TeamId
                    selectCodeColumn = $"r.{deepestLevel.codeColumn}";
                    selectNameColumn = $"rorg.{deepestLevel.nameColumn}";
                    groupByNameColumn = $"r.{deepestLevel.codeColumn}, rorg.{deepestLevel.nameColumn}";
                }
            }

            string sql = $@"
                    SELECT
                        r.AttendanceDate,
                        {selectCodeColumn} AS UnitCode,
                        {selectNameColumn} AS UnitName,
                        SUM(r.ExpectedCnt) AS ShouldCount,
                        SUM(r.ActualCnt) AS ActualCount
                    FROM dbo.vw_RPT25015 r
                    LEFT JOIN dbo.vw_RPT25015_OrgRef rorg
                        ON r.DBName = rorg.DBName
                        AND r.ParentBuId = rorg.ParentBuId
                        AND r.DivisionId = rorg.DivisionId
                        AND r.DepartmentId = rorg.DepartmentId
                        AND r.SectionId = rorg.SectionId
                        AND r.TeamId = rorg.TeamId
                    WHERE
                        {whereConditions}
                    GROUP BY
                        r.AttendanceDate, {groupByNameColumn}
                ";

            return GetDataTable(sql, sp);
        }

        //2025/10/30 Darnell

        public static int SetRPT25016(
            string attendanceDate,
            string id,
            string editorId,
            string rowNo,
            string divisionName,
            string caseCount,
            string remark
        )
        {
            int successCount = 0;

            // Debug 訊息
            System.Diagnostics.Debug.WriteLine($"AttendanceDate = {attendanceDate}");
            System.Diagnostics.Debug.WriteLine($"ID = {id}, EditorId = {editorId}, RowNo = {rowNo}, CaseCount = {caseCount}, Remark = {remark}");

            // 建立 SQL 參數清單
            List<SqlParameter> sp = new List<SqlParameter>()
            {
        new SqlParameter() { ParameterName = "@AttendanceDate", SqlDbType = SqlDbType.Date, Value = attendanceDate },
        new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, Value = int.Parse(id) },
        new SqlParameter() { ParameterName = "@EditorId", SqlDbType = SqlDbType.VarChar, Value = editorId },
        new SqlParameter() { ParameterName = "@RowNo", SqlDbType = SqlDbType.VarChar, Value = rowNo },
        new SqlParameter() { ParameterName = "@DivisionName", SqlDbType = SqlDbType.NVarChar, Value = string.IsNullOrEmpty(divisionName) ? string.Empty : divisionName },
        new SqlParameter() { ParameterName = "@CaseCount", SqlDbType = SqlDbType.Int, Value = int.Parse(caseCount) },
        new SqlParameter() { ParameterName = "@Remark", SqlDbType = SqlDbType.NVarChar, Value = remark }
    };

            // 執行 SQL
            int row = GetExecuteNonQuery(@"
        UPDATE RPT25016
        SET
            CaseCount = @CaseCount,
            Remark = @Remark,
            EditorId = @EditorId,
            EditedDate = GETDATE()
        WHERE AttendanceDate = @AttendanceDate AND ID = @ID;

        INSERT INTO RPT25016_Log
            (
                AttendanceDate,
                RowNo,
                DivisionName,
                CaseCount,
                Remark,
                EditorId,
                EditedDate
            )
        VALUES
            (
            @AttendanceDate,
            @RowNo,
            @DivisionName,
            @CaseCount,
            @Remark,
            @EditorId,
            GETDATE()
            );
    ", sp);

            successCount += row;

            return successCount;
        }

        // 更新RPT25016的Total欄位
        public static int SetRPT25016ToTal(
            string attendanceDate,
            string editorId
        )
        {
            int successCount = 0;

            // 建立 SQL 參數清單
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@AttendanceDate", SqlDbType = SqlDbType.Date, Value = attendanceDate },
                new SqlParameter() { ParameterName = "@EditorId", SqlDbType = SqlDbType.VarChar, Value = editorId }
            };

            // 執行 SQL
            int row = GetExecuteNonQuery(@"
            DECLARE @TotalCount int;
            SELECT @TotalCount = SUM(CaseCount) FROM RPT25016 WHERE AttendanceDate = @AttendanceDate AND RowNo <> 'TOTAL'
            UPDATE RPT25016
            SET
                CaseCount = @TotalCount,
                EditorId = @EditorId,
                EditedDate = GETDATE()
            WHERE AttendanceDate = @AttendanceDate AND RowNo = 'TOTAL';
    ", sp);

            successCount += row;

            return successCount;
        }

        public static int GetExecuteNonQuery(string Sql, List<SqlParameter> SPs, bool isSp = false)
        {
            try
            {
                using (SqlConnection conn = Conn2())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        int row = 0;
                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        if (isSp)
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        else
                            cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = Sql;
                        if (SPs != null)
                            cmd.Parameters.AddRange(SPs.ToArray());
                        // 移除原本的Table方式，改由執行並回傳筆數
                        //DataTable dt = cmd.FillDataSet("temp").Tables[0];
                        //dt.TableName = Guid.NewGuid().ToString("N").Substring(0, 5);
                        conn.Open(); // 手動開啟連線
                        row = cmd.ExecuteNonQuery(); // 回傳執行筆數
                        conn.Close(); // 手動關閉連線
                        return row;
                    }
                }
            }
            catch (System.Exception oex)
            {
                throw new System.Exception(oex.Message);
            }
        }

        public static DataTable GetRPT25016(string queryDay)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = "@QueryDay", SqlDbType = SqlDbType.Date, Value =  queryDay },
             };
            return GetDataTable(@"
                SELECT * FROM dbo.RPT25016
                Where AttendanceDate = @QueryDay
                Order By ID", sp);
        }

        // 2025/11/13 Una - For DnaReaprt的HR異常表類別

        // 新增類別 接收要Update的異常資料
        public class HrExceptionData
        {
            public string AttendanceDate { get; set; } // 打卡日期
            public List<HrExceptionTableRow> TableRowData { get; set; } // 異常列資料
        }

        public class HrExceptionTableRow
        {
            public string ID { get; set; }
            public string EditorId { get; set; }
            public string RowNo { get; set; }
            public string DivisionName { get; set; }
            public string CaseCount { get; set; } // 案件數量，在numCaseCount轉成數字
            public string Remark { get; set; }

            public string IsChange { get; set; }

            public int numCaseCount
            {
                get
                {
                    if (int.TryParse(CaseCount, out int value))
                    { // 轉換成功則改變value值為轉換後結果
                        return value;
                    }
                    return value; // 轉換失敗則回傳預設值0
                }
            }

            public int numID
            {
                get
                {
                    if (int.TryParse(ID, out int value))
                    { // 轉換成功則改變value值為轉換後結果
                        return value;
                    }
                    return value; // 轉換失敗則回傳預設值0
                }
            }
        }

        #endregion 2025/11/12 HR報表
    }
}
