using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IngestMatrixPlugin.Dto.Vo;
using IngestMatrixPlugin.Models.DB;

namespace IngestMatrixPlugin.cs
{
    public class MatrixProfile : Profile
    {
        public MatrixProfile()
        {
            #region DbpVirtualmatrixportstate Map MatrixVirtualPortInfo
            CreateMap<DbpVirtualmatrixportstate, MatrixVirtualPortInfo>()
                .ForMember(a => a.lVirtualInPort, (map) => map.MapFrom(b => b.Virtualinport))
                .ForMember(a => a.lVirtualOutPort, (map) => map.MapFrom(b => b.Virtualoutport))
                .ForMember(a => a.lState, (map) => map.MapFrom(b => b.State));
            CreateMap<MatrixVirtualPortInfo, DbpVirtualmatrixportstate>()
                .ForMember(a => a.Virtualinport, (map) => map.MapFrom(b => b.lVirtualInPort))
                .ForMember(a => a.Virtualoutport, (map) => map.MapFrom(b => b.lVirtualOutPort))
                .ForMember(a => a.State, (map) => map.MapFrom(b => b.lState))
                .ForMember(a => a.Matrixid, (map) => map.MapFrom(b => 1));
            #endregion
        }
    }
}
