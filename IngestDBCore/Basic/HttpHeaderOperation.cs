using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IngestDBCore.Basic
{
   
    public class HttpHeaderOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }
            context.ApiDescription.TryGetMethodInfo(out var methodInfo);

            if (methodInfo.DeclaringType.IsDefined(typeof(ApiVersionAttribute), true))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "sobeyhive-ingest-signature",
                    In = ParameterLocation.Header,
                    Description = "sobeyhive-ingest-signature",
                    Required = true,
                    Schema = new OpenApiSchema // Parameter variable format 
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}
