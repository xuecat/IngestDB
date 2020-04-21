using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Dto.Response
{
    public class ResponseMessage
    {
		/// <summary>返回代码</summary>
		public int nCode { get; set; }
		/// <summary>返回消息</summary>
		public string message { get; set; }
		public ResponseMessage()
		{
			nCode = 1;          //1代表成功，0代表失败
			message = "OK";
		}
	}

	public class ResponseMessage<T> : ResponseMessage
	{
		/// <summary>返回数据</summary>
		public T extention;
	}
}
