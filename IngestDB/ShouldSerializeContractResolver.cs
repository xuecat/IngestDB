using IngestDBCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IngestDB
{

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;
        public ShouldSerializeContractResolver()
        {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        protected override JsonContract CreateContract(Type type)
        {
            if (type.BaseType == typeof(ResponseMessage) || type.FullName.IndexOf("Dto.Response")>0)
                return _camelCaseContractResolver.ResolveContract(type);

            return _defaultContractSerializer.ResolveContract(type);

        }

    }
}
