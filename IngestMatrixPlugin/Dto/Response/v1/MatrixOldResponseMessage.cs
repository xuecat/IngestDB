using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestMatrixPlugin.Dto.Response.v1
{
	/// <summary>
	/// V1 对象返回
	/// </summary>
    public class MatrixOldResponseMessage
    {
		/// <summary>返回代码</summary>
		/// <example>1</example>
		public int nCode { get; set; }
		/// <summary>返回消息</summary>
		/// <example>OK</example>
		public string message { get; set; }
		/// <summary>
		/// 构造函数
		/// </summary>
		public MatrixOldResponseMessage()
		{
			nCode = 1;          //1代表成功，0代表失败
			message = "OK";
		}
	}
	/// <summary>
	/// V1 泛型 对象返回
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MatrixOldResponseMessage<T> : MatrixOldResponseMessage
	{
		/// <summary>返回数据</summary>
		public T extention;
	}
}
