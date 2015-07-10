using Newtonsoft.Json;

namespace YATDL.Models
{
    public class ValidationErrorsModel : AjaxErrorInfo
    {
        [JsonProperty("validationErrors")]
        public ValidationError[] ValidationErrors { get; set; }


        public ValidationErrorsModel()
        {
            this.Error = "Ошибки валидации данных";
            this.ErrorDetails = "Исправьте значения и повторите сохранение";
        }
    }
}