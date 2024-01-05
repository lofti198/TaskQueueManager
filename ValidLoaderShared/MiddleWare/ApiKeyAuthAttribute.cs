
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using ValidLoaderShared.Consts;
using ValidLoaderShared.DataContracts;

namespace ValidLoaderShared.MiddleWare
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Resolve IHttpClientFactory from the services container
            var httpClientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();

            // Rest of your existing logic to validate the API key

            if (!context.HttpContext.Request.Headers.TryGetValue(VLConstants.ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!await IsValidApiKey(httpClientFactory, potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        private async Task<bool> IsValidApiKey(IHttpClientFactory httpClientFactory, string apiKey)
        {
            using (var client = httpClientFactory.CreateClient())
            {
                try
                {
                    var userPanelApiUrl = Environment.GetEnvironmentVariable(VLConstants.ENV_VAR_NAME_VLUserPanelApiUrl)
                                          ?? throw new InvalidOperationException("The User Panel API URL is not set in the environment variables.");

                    var request = new HttpRequestMessage(HttpMethod.Post, userPanelApiUrl);
                    request.Headers.Add(VLConstants.ApiKeyHeaderName, VLConstants.ServiceInternalSecretKey);
                    request.Content = new StringContent(JsonSerializer.Serialize(new { ApiKey = apiKey }), System.Text.Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode(); // This will throw HttpRequestException for non-success codes

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var validationResponse = JsonSerializer.Deserialize<ApiKeyValidationResponse>(responseContent);

                    return validationResponse?.IsValid ?? false;
                }
                catch (HttpRequestException ex)
                {
                    // You can log the exception here if needed
                    // LogException(ex);

                    // Rethrow the exception to let the client know there was a problem with the request
                    throw new Exception($"There was an error processing the request. {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions if necessary
                    throw new Exception($"An unexpected error occurred. {ex.Message}");
                }
            }
        }



    }
    //public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    //{
    //    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    //    {
    //        if (!context.HttpContext.Request.Headers.TryGetValue(VLConstants.ApiKeyHeaderName, out var potentialApiKey))
    //        {
    //            context.Result = new UnauthorizedResult();
    //            return;
    //        }

    //        // var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    //        var apiKey = "key12345";// configuration.GetValue<string>("ApiKey");

    //        if (!apiKey.Equals(potentialApiKey))
    //        {
    //            context.Result = new UnauthorizedResult();
    //            return;
    //        }

    //        await next();
    //    }
    //}

}
