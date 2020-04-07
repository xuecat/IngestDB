﻿using IngestDBCore;
using IngestDBCore.Interface;
using IngestDevicePlugin.Controllers;
using IngestDevicePlugin.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static IngestDBCore.DeviceInternals;

namespace IngestTaskInterfacePlugin
{
    public class IngestDeviceInterfaceImplement : IIngestDeviceInterface
    {
        public async Task<ResponseMessage> GetDeviceCallBack(DeviceInternals examineResponse)
        {
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var reqService = scope.ServiceProvider.GetRequiredService<DeviceController>();

                switch (examineResponse.funtype)
                {
                    case FunctionType.ChannelInfoBySrc:
                        return await reqService.ChannelsByProgrammeId(examineResponse.SrcId);
                    default:
                        break;
                }
                //var response = await scope.ServiceProvider.GetRequiredService<GlobalController>()
                //    .SubmitGlobalCallback();

                //return Mapper.Map<ResponseMessage>(response);
            }

            return null;
        }
        
        public async Task<ResponseMessage> SubmitDeviceCallBack(DeviceInternals examineResponse)
        {
            throw new NotImplementedException();
        }
        
    }
}
