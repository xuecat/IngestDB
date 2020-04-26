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
    public class RemoveVersionFromParameter : IOperationFilter
    {
       
        void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters != null && operation.Parameters.Count > 0)
            {
                var versionParameter = operation.Parameters.Single(p => p.Name == "version");
                operation.Parameters.Remove(versionParameter);
            }
            
        }
    }
    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
       
        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            
            var f = swaggerDoc.Paths
                .ToDictionary(
                    path => path.Key.Replace("v{version}", swaggerDoc.Info.Version),
                    path => path.Value
                );
            swaggerDoc.Paths.Clear();
            foreach (var item in f)
            {
                swaggerDoc.Paths.Add(item.Key, item.Value);
            }
            int aa = 4;
        }
    }

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
                    operation.Parameters.Remove(param);
                    //param.Schema.Default = new OpenApiString(apiVersion.Versions[0].ToString());
                }
                
            }
        }
    }
}
