using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace Domain.Common;
public class Result<T>
    {
        public T Data { get; set; }
        public List<ValidationResult> ValidationErrors { get; set; } = [];
        public int StatusCode { get; set; }
        public bool HasError { get { return ValidationErrors.Count != 0; } }
        public string Message { get; set; }
        public void AddError(string message, int statusCode)
        {
            ValidationErrors.Add(new ValidationResult(message));
            StatusCode = statusCode;
        }

        public string GetSingleErrorMessage()
            => ValidationErrors.Select(c => c.ErrorMessage).First();

        public string[] GetAllErrorMessages()
            => [.. ValidationErrors.Select(c => c.ErrorMessage)];
    }