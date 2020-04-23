using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IngestDBCore.Basic
{
    public class DefaultValueOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                var parameters = descriptor.Parameters.Where(a => a is ControllerParameterDescriptor).Select(a => a as ControllerParameterDescriptor);
                foreach (var parameter in parameters)
                {
                    var defaultValue = parameter.ParameterInfo.GetCustomAttributes(true).FirstOrDefault(a => a is DefaultValueAttribute) as DefaultValueAttribute;
                    var param = operation.Parameters.FirstOrDefault(a => a.Name == parameter.Name);
                    if (param != null && defaultValue != null)
                    {
                        param.Schema.Default = OpenApiAnyFactory.CreateFor(param.Schema, defaultValue.Value);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DefaultValueAttribute : Attribute
    {
        //public string Name { get; set; }
        public object Value { get; set; }
        public DefaultValueAttribute(object value)
        {
            this.Value = value;
        }
    }
}
