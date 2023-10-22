using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Services.Helpers
{
    public class ValidationHelper
    {
        internal static void ValidateModels(object obj)
        {
            ValidationContext context = new(obj);
            List<ValidationResult> validationResult = new();

            bool isValid = Validator.TryValidateObject(obj, context, validationResult, true);
            if (!isValid) throw new ArgumentException(validationResult.FirstOrDefault()?.ErrorMessage);
        }
    }
}
