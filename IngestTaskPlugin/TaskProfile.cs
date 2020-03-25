using AutoMapper;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskMetadataResponse, DbpTaskMetadata>();
        }
    }
}
