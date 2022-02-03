using System;
using System.Collections.Generic;
using System.Text;

namespace InsightRESTAPI.Model.CommonModel
{
    public class SqlCommandParameter
    {
        public SqlCommandParameter(string name, object value)
        {
            ParameterName = name;
            ParameterValue = value == null ? DBNull.Value : value;
        }

        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }


        public static SqlCommandParameter AddParameter(string parameterName, object parameterValue)
        {
            return new SqlCommandParameter(parameterName, parameterValue);
        }
    }

    public class SqlCommandParameters
    {
        public List<SqlCommandParameter> List { get; set; } = new List<SqlCommandParameter>();

        public void Add(string name, object value, bool nullable = false)
        {
            if (nullable || value != null)
            {
                List.Add(new SqlCommandParameter(name, value ?? DBNull.Value));
            }
        }

        public void AddAll(List<(string, object)> parameters)
        {
            parameters.ForEach(param => Add(param.Item1, param.Item2));
        }
    }
}
