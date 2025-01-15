using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private String PropertyName { get; set; }
        private new String ErrorMessage { get; set; }
        private Object DesiredValue { get; set; }

        public RequiredIfAttribute(String propertyName, Object desiredValue, String errorMessage)
        {
            this.PropertyName = propertyName;
            this.DesiredValue = desiredValue;
            this.ErrorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            Object instance = context.ObjectInstance;
            Type type = instance.GetType();
            Object propertyvalue = type.GetProperty(PropertyName).GetValue(instance, null);

            if (DesiredValue.Equals("IsNotNull") && propertyvalue != null && value == null)
                return new ValidationResult(ErrorMessage);

            if (propertyvalue != null)
            {
                if (propertyvalue.ToString() == DesiredValue.ToString() && value == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
    public class NUmberGreaterThanOrEqualAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        public NUmberGreaterThanOrEqualAttribute(string comparisonProperty) { _comparisonProperty = comparisonProperty; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success; //new ValidationResult("Invalid entry");
            ErrorMessage = ErrorMessageString;

            if (value.GetType() == typeof(IComparable)) throw new ArgumentException("value has not implemented IComparable interface");
            var currentValue = (IComparable)Convert.ToDouble(value);

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null) throw new ArgumentException("Comparison property with this name not found");

            var comparisonValue = Convert.ToDouble(property.GetValue(validationContext.ObjectInstance));

            if (!ReferenceEquals(currentValue.GetType(), comparisonValue.GetType()))
                throw new ArgumentException("The types of the fields to compare are not the same.");

            return currentValue.CompareTo(comparisonValue) >= 0 ? ValidationResult.Success : new ValidationResult(ErrorMessage);

        }
    }
    #region Date
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DateGreaterThanOrEqualAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        public DateGreaterThanOrEqualAttribute(string comparisonProperty) { _comparisonProperty = comparisonProperty; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success; //new ValidationResult("Invalid entry");
            ErrorMessage = ErrorMessageString;

            if (value.GetType() == typeof(IComparable)) throw new ArgumentException("value has not implemented IComparable interface");
            var currentValue = (IComparable)Convert.ToDateTime(value);

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null) throw new ArgumentException("Comparison property with this name not found");

            var comparisonValue = Convert.ToDateTime(property.GetValue(validationContext.ObjectInstance));
            if (!ReferenceEquals(currentValue.GetType(), comparisonValue.GetType()))
                throw new ArgumentException("The types of the fields to compare are not the same.");

            return currentValue.CompareTo(comparisonValue) >= 0 ? ValidationResult.Success : new ValidationResult(ErrorMessage);

        }
    }
    #endregion
}
