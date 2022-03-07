using Manager.Application.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Manager.Api.Binders
{
    public class RevokeTokenModelBinder : IModelBinder
    { 
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentException(nameof(bindingContext));
            }

            var model = await JsonSerializer.DeserializeAsync<RevokeTokenDto>(bindingContext.HttpContext.Request.Body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            model.AccessToken = bindingContext
                .HttpContext
                .Request
                .Headers["Expired-token"]
                .ToString()
                .Remove(0, 7);

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}
