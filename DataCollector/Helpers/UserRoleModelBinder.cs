using Microsoft.AspNetCore.Mvc.ModelBinding;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Helpers
{
    public class UserRoleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

                var value = valueProviderResult.FirstValue;

                if (Enum.TryParse(typeof(UserRole), value, true, out var result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }

}
