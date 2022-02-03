using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace InsightRESTAPI.Model.CommonModel
{
    public class SqlCommandOutputParameter
    {
        public string ParameterName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public int Size { get; set; }

        public SqlCommandOutputParameter(string parameterName, SqlDbType sqlDbType, int size = 0)
        {
            ParameterName = parameterName;
            SqlDbType = sqlDbType;
            Size = size;
        }

        public static SqlCommandOutputParameter AddOutputParameter(string parameterName, SqlDbType sqlDbType, int size = 0)
        {
            return new SqlCommandOutputParameter(parameterName, sqlDbType, size);
        }
    }

    public class SqlCommandOutputParameters
    {
        public List<SqlCommandOutputParameter> List { get; set; } = new List<SqlCommandOutputParameter>();
        public void Add(string parameterName, SqlDbType sqlDbType, int size = 0)
        {
            List.Add(new SqlCommandOutputParameter(parameterName, sqlDbType, size));
        }
        public void AddAll(List<(string, SqlDbType, int)> parameters)
        {
            parameters.ForEach(param => Add(param.Item1, param.Item2, param.Item3));
        }
    }
}
