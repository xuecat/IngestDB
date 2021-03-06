﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.Basic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IngestAuthentication : ActionFilterAttribute
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!IsValidRequestAsync(context.HttpContext.Request))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                //actionContext.Response = context.Request.CreateErrorResponse(
                //    HttpStatusCode.Forbidden,
                //    "The ingest request is invalid(no security)"
                //);
            }

            //base.OnAuthorization(context);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsValidRequestAsync(context.HttpContext.Request))
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public static string UnBase64String(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }
        private bool IsValidRequestAsync(HttpRequest request)
        {
            var headerExists = request.Headers["sobeyhive-ingest-signature"];
                //"sobeyhive-ingest-signature", out signature);
            if (Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(headerExists) || headerExists.Count <=0) return false;

            if (headerExists.First() == "ingest_admin")
            {
                return true;
            }

            string data = UnBase64String(headerExists.First());
            string[] requesta = data.Split(';');

            if (requesta[0] == "ingest_server")
            {
                return true;

                DateTime date = DateTime.Parse(requesta[1]);
                var span = DateTime.Now - date;
                if (span.TotalMinutes > 3)
                {
                    return false;
                }
                else
                    return true;
            }
            if (requesta[0] == "ingest_web")
            {
                DateTime date = DateTime.Parse(requesta[1]);
                var span = DateTime.Now - date;
                if (span.TotalMinutes > 3)
                {
                    return false;
                }
                else
                    return true;
            }
            else if (requesta[0] == "ingest_client")
            {
                DateTime date = DateTime.Parse(requesta[1]);
                var span = DateTime.Now - date;
                if (span.TotalMinutes > 5)
                {
                    return false;
                }
                else
                    return true;
            }
            return false;
        }
    }
}
