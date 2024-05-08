using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace DataCollector.Helpers
{
    public class PhotosModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var value = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();

            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var elementType = bindingContext.ModelType.GetElementType() ??
                bindingContext.ModelType.GetTypeInfo().GenericTypeArguments.FirstOrDefault();

            if (elementType == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var converter = TypeDescriptor.GetConverter(elementType);

            var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(Clean(x)))
                .ToArray();

            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            bindingContext.Model = typedValues;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }

        private static string Clean(string str)
        {
            return str.Trim('(', ')').Trim('[', ']').Trim();
        }
    }

}
