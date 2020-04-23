using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
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
                        Type = "string",
                        Default = new OpenApiString("ingest_admin")
                    }
                });
                var apiVersion = methodInfo.DeclaringType.GetCustomAttributes(true).Where(a => a is ApiVersionAttribute).Select(a => a as ApiVersionAttribute).FirstOrDefault();
                var param = operation.Parameters.SingleOrDefault(a => a.Name == "version" && a.In == ParameterLocation.Path);
                if (param!=null)
                {
                    param.Schema.Default = new OpenApiString(apiVersion.Versions[0].ToString());
                }
                
            }
        }
    }
}
