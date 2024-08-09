using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Response
{
    public partial class ResponseHandler
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ServerDate { get; set; } = DateTime.UtcNow;
        public Guid ReferenceNumber { get; set; } = Guid.NewGuid();

        public ResponseHandler(string message = null) 
        {
            if(string.IsNullOrEmpty(message))
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
                Message = message;
            }
        }
    }

    public partial class ResponseHandler<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ServerDate { get; set; } = DateTime.UtcNow;
        public Guid ReferenceNumber { get; set; } = Guid.NewGuid();

        public ResponseHandler(T data)
        {
            IsSuccess = true;
            Data = data;
            Message = "Success";
        }

        public ResponseHandler(bool isSuccess)
        {
            IsSuccess = isSuccess;
            Data = default;
            Message = isSuccess ? "Success" : "Error";
        }

        public ResponseHandler(string error) 
        {
            IsSuccess = true;
            Data = default;
            Message = error;
        }
    }
}
