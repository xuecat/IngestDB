using Sobey.Core.Log;
using System;
using System.Diagnostics;
using System.Text;

namespace IngestDBCore
{
	/// <summary>
	/// Summary description for BaseException.
	/// </summary>
	public class BaseException : System.ApplicationException
	{
        public int ErrorCode { get; set; }
        //����
        //�޴�����
        protected BaseException(string strMessage, System.Exception oInner, ILogger logger, int lerrcode) :base(strMessage,oInner)
		{
            ErrorCode = lerrcode;
			Dump(Format(strMessage, oInner), logger);
		}

		private void Dump(string str, ILogger logger)
		{
            if (logger != null)
            {
                logger.Error(str);
            }

            
        }
        //�д�����

		public static string Format(string msg, System.Exception inner)
		{
			StringBuilder newmsg = new StringBuilder();
			string errorstack = null;
			//��ȡ�����ջ
			errorstack = BuildErrorStack(inner);

			newmsg.Append("SobeyRecException Summary \n")
				//.Append("---------------------------\n")
				//.Append(DateTime.Now.ToShortDateString())
				//.Append(":")
			    //.Append(DateTime.Now.ToShortTimeString())
				.Append(" - ")
				.Append(msg)
				.Append("\n\n")
				.Append(errorstack);

			return newmsg.ToString();
		}

		private static string BuildErrorStack(System.Exception chain)
		{
			string error_stack = null;
			StringBuilder stack_builder = new StringBuilder();
			int num = 1;
			System.Exception inner = null;

			if(chain != null)
			{
				stack_builder.Append("Error Stack \n").Append("-------------------------\n");

				inner = chain;

				while(inner != null)
				{
					stack_builder.Append(num).Append(") ").Append(inner.Message).Append("\n");
					inner = inner.InnerException;
					num++;
				}

				stack_builder.Append("\n---------------------\n").Append("Call Stack\n");
				stack_builder.Append(chain.StackTrace);
				error_stack = stack_builder.ToString();
			}
			else
			{
//				error_stack = "exception was not chained";
				error_stack = "";
		//		error_stack+=Environment.StackTrace;
			}

			return error_stack;
		}
	}

	public class SobeyRecException: BaseException
	{
        
		private SobeyRecException(string strMessage, System.Exception oInner, ILogger logger, int lLogErrorCode):base(strMessage,oInner, logger, lLogErrorCode)
		{
		}

        /// <summary>
        /// ͨ���Զ����쳣�����׳��Զ����쳣
        /// </summary>
        /// <param name="lLogErrorCode">�Զ����쳣����</param>
        /// <remarks>
        /// Add by chenzhi 2013-06-04
        /// </remarks>
        //public static void ThrowSelf(int lLogErrorCode, ILogger logger)
        //{
        //    SobeyRecException ex = new SobeyRecException(GlobalDictionary.Instance.GetMessageByCode(lLogErrorCode), null, logger, lLogErrorCode);
        //    throw ex;
        //}

        /// <summary>
        /// ͨ���Զ����쳣���뼰�ϲ��쳣�׳��Զ����쳣
        /// </summary>
        /// <param name="lLogErrorCode">�Զ����쳣����</param>
        /// <param name="oInner">�ϲ��쳣</param>
        /// <remarks>
        /// Add by chenzhi 2013-06-18
        /// </remarks>
        public static void ThrowSelfNoParam(int lLogErrorCode, ILogger logger, System.Exception oInner)
        {
            string strMessage = GlobalDictionary.Instance.GetMessageByCode(lLogErrorCode);
            BuildErrInfo(oInner, ref strMessage);
            SobeyRecException ex = new SobeyRecException(strMessage, oInner, logger, lLogErrorCode);
            throw ex;
        }

        public static void ThrowSelfOneParam(int lLogErrorCode, ILogger logger, object p, System.Exception oInner)
        {
            string strMessage = GlobalDictionary.Instance.GetMessageByCode(lLogErrorCode);
            strMessage = string.Format(strMessage, p.ToString());
            BuildErrInfo(oInner, ref strMessage);
            SobeyRecException ex = new SobeyRecException(strMessage, oInner, logger, lLogErrorCode);
            throw ex;
        }

        public static void ThrowSelfTwoParam(int lLogErrorCode, ILogger logger, object p1, object p2, System.Exception oInner)
        {
            string strMessage = GlobalDictionary.Instance.GetMessageByCode(lLogErrorCode);
            strMessage = string.Format(strMessage, p1.ToString(), p2.ToString());
            BuildErrInfo(oInner, ref strMessage);
            SobeyRecException ex = new SobeyRecException(strMessage, oInner, logger, lLogErrorCode);
            throw ex;
        }

		private static void BuildErrInfo(Exception oInner, ref string strMessage)
		{
			System.Exception walker = oInner;
			while (walker!=null)
			{
				strMessage+=":";
				strMessage+=walker.Message;
				walker =  walker.InnerException;
			}
		}
	}
}
