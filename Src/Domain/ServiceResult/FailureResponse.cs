using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApiClean.Domain.ServiceResult
{
    [Serializable]
    public class FailureResponse
    {
        [Required]
        [Description("Error message")]
        public string Message { get; set; } = string.Empty;

        [Required]
        [Description("Error code")]
        public int ErrorCode { get; set; }

        [Required]
        [Description("Error text code")]
        public string ErrorCodeDescription { get; set; }
    }
}
