using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Application.Helpers
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public static ServiceResult<T> Success(T Data ,string Message = "Success") 
            => new ServiceResult<T> {IsSuccess=true, Data = Data , Message = Message };

        public static ServiceResult<T> Failure(string Message,int StatusCode=400)
            => new ServiceResult<T> {IsSuccess=false,Message= Message,StatusCode=StatusCode}; 
    }
}
