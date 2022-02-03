using System;
using System.Collections.Generic;
using System.Text;

namespace InsightRESTAPI.Model.CommonModel
{
    public class ServiceResponse<TEntity> where TEntity : class
    {
        public TEntity data { get; set; }
        public List<string> message { get; set; }
        public bool success { get; set; }

        public static ServiceResponse<TEntity> RetreivedSuccessfully(TEntity data)
        {
            return new ServiceResponse<TEntity>
            {
                data = data,
                message = new List<string> { "Data retrieved successfully." },
                success = true
            };
        }

        public static ServiceResponse<TEntity> AddedSuccessfully(TEntity data)
        {
            return new ServiceResponse<TEntity>
            {
                data = data,
                message = new List<string> { "Resource added successfully." },
                success = true
            };
        }

        public static ServiceResponse<TEntity> UpdatedSuccessfully(TEntity data)
        {
            return new ServiceResponse<TEntity>
            {
                data = data,
                message = new List<string> { "Resource updated successfully." },
                success = true
            };
        }

        public static ServiceResponse<string> DeletedSuccessfully()
        {
            return new ServiceResponse<string>
            {
                data = null,
                message = new List<string> { "Resource deleted successfully." },
                success = true
            };
        }

        public static ServiceResponse<TEntity> NotFound()
        {
            return new ServiceResponse<TEntity>
            {
                data = null,
                message = new List<string> { "No record found." },
                success = false
            };
        }

        public static ServiceResponse<TEntity> Error(string message = null)
        {
            return new ServiceResponse<TEntity>
            {
                data = null,
                message = new List<string> { message ?? "There was a problem handling the request." },
                success = false
            };
        }

        public static ServiceResponse<TEntity> Success(string message = null, TEntity data = null)
        {
            return new ServiceResponse<TEntity>
            {
                data = data,
                message = new List<string> { message ?? "Request successful." },
                success = true
            };
        }
    }
}
