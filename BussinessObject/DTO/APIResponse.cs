using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public static APIResponse<T> SuccessResult(T data, string? message = null)
        {
            return new APIResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static APIResponse<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new APIResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
