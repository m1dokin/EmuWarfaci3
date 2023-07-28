using System;
using Enyim.Caching;
using NLog;

namespace MasterServer.Core
{
	// Token: 0x0200011F RID: 287
	public class MasterServerLoggerFactory : ILogFactory
	{
		// Token: 0x0600048A RID: 1162 RVA: 0x00013DB8 File Offset: 0x000121B8
		ILog ILogFactory.GetLogger(string name)
		{
			return MasterServerLoggerFactory.MasterServerLogger.Instance;
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00013DBF File Offset: 0x000121BF
		ILog ILogFactory.GetLogger(Type type)
		{
			return MasterServerLoggerFactory.MasterServerLogger.Instance;
		}

		// Token: 0x02000120 RID: 288
		private class MasterServerLogger : ILog
		{
			// Token: 0x0600048C RID: 1164 RVA: 0x00013DC6 File Offset: 0x000121C6
			private MasterServerLogger()
			{
			}

			// Token: 0x17000094 RID: 148
			// (get) Token: 0x0600048D RID: 1165 RVA: 0x00013DCE File Offset: 0x000121CE
			public bool IsDebugEnabled
			{
				get
				{
					return MasterServerLoggerFactory.MasterServerLogger.Logger.IsDebugEnabled;
				}
			}

			// Token: 0x17000095 RID: 149
			// (get) Token: 0x0600048E RID: 1166 RVA: 0x00013DDA File Offset: 0x000121DA
			public bool IsErrorEnabled
			{
				get
				{
					return MasterServerLoggerFactory.MasterServerLogger.Logger.IsErrorEnabled;
				}
			}

			// Token: 0x17000096 RID: 150
			// (get) Token: 0x0600048F RID: 1167 RVA: 0x00013DE6 File Offset: 0x000121E6
			public bool IsFatalEnabled
			{
				get
				{
					return MasterServerLoggerFactory.MasterServerLogger.Logger.IsFatalEnabled;
				}
			}

			// Token: 0x17000097 RID: 151
			// (get) Token: 0x06000490 RID: 1168 RVA: 0x00013DF2 File Offset: 0x000121F2
			public bool IsInfoEnabled
			{
				get
				{
					return MasterServerLoggerFactory.MasterServerLogger.Logger.IsInfoEnabled;
				}
			}

			// Token: 0x17000098 RID: 152
			// (get) Token: 0x06000491 RID: 1169 RVA: 0x00013DFE File Offset: 0x000121FE
			public bool IsWarnEnabled
			{
				get
				{
					return MasterServerLoggerFactory.MasterServerLogger.Logger.IsWarnEnabled;
				}
			}

			// Token: 0x06000492 RID: 1170 RVA: 0x00013E0A File Offset: 0x0001220A
			public void Debug(object message)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(message);
			}

			// Token: 0x06000493 RID: 1171 RVA: 0x00013E17 File Offset: 0x00012217
			public void Debug(object message, Exception exception)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(exception, message.ToString(), new object[0]);
			}

			// Token: 0x06000494 RID: 1172 RVA: 0x00013E30 File Offset: 0x00012230
			public void DebugFormat(string format, object arg0)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(format, arg0);
			}

			// Token: 0x06000495 RID: 1173 RVA: 0x00013E3E File Offset: 0x0001223E
			public void DebugFormat(string format, object arg0, object arg1)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(format, arg0, arg1);
			}

			// Token: 0x06000496 RID: 1174 RVA: 0x00013E4D File Offset: 0x0001224D
			public void DebugFormat(string format, object arg0, object arg1, object arg2)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(format, arg0, arg1, arg2);
			}

			// Token: 0x06000497 RID: 1175 RVA: 0x00013E5E File Offset: 0x0001225E
			public void DebugFormat(string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(format, args);
			}

			// Token: 0x06000498 RID: 1176 RVA: 0x00013E6C File Offset: 0x0001226C
			public void DebugFormat(IFormatProvider provider, string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Debug(provider, format, args);
			}

			// Token: 0x06000499 RID: 1177 RVA: 0x00013E7B File Offset: 0x0001227B
			public void Info(object message)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(message);
			}

			// Token: 0x0600049A RID: 1178 RVA: 0x00013E88 File Offset: 0x00012288
			public void Info(object message, Exception exception)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(exception, message.ToString(), new object[0]);
			}

			// Token: 0x0600049B RID: 1179 RVA: 0x00013EA1 File Offset: 0x000122A1
			public void InfoFormat(string format, object arg0)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(format, arg0);
			}

			// Token: 0x0600049C RID: 1180 RVA: 0x00013EAF File Offset: 0x000122AF
			public void InfoFormat(string format, object arg0, object arg1)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(format, arg0, arg1);
			}

			// Token: 0x0600049D RID: 1181 RVA: 0x00013EBE File Offset: 0x000122BE
			public void InfoFormat(string format, object arg0, object arg1, object arg2)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(format, arg0, arg1, arg2);
			}

			// Token: 0x0600049E RID: 1182 RVA: 0x00013ECF File Offset: 0x000122CF
			public void InfoFormat(string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(format, args);
			}

			// Token: 0x0600049F RID: 1183 RVA: 0x00013EDD File Offset: 0x000122DD
			public void InfoFormat(IFormatProvider provider, string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Info(provider, format, args);
			}

			// Token: 0x060004A0 RID: 1184 RVA: 0x00013EEC File Offset: 0x000122EC
			public void Warn(object message)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(message);
			}

			// Token: 0x060004A1 RID: 1185 RVA: 0x00013EF9 File Offset: 0x000122F9
			public void Warn(object message, Exception exception)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(exception, message.ToString(), new object[0]);
			}

			// Token: 0x060004A2 RID: 1186 RVA: 0x00013F12 File Offset: 0x00012312
			public void WarnFormat(string format, object arg0)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(format, arg0);
			}

			// Token: 0x060004A3 RID: 1187 RVA: 0x00013F20 File Offset: 0x00012320
			public void WarnFormat(string format, object arg0, object arg1)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(format, arg0, arg1);
			}

			// Token: 0x060004A4 RID: 1188 RVA: 0x00013F2F File Offset: 0x0001232F
			public void WarnFormat(string format, object arg0, object arg1, object arg2)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(format, arg0, arg1, arg2);
			}

			// Token: 0x060004A5 RID: 1189 RVA: 0x00013F40 File Offset: 0x00012340
			public void WarnFormat(string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(format, args);
			}

			// Token: 0x060004A6 RID: 1190 RVA: 0x00013F4E File Offset: 0x0001234E
			public void WarnFormat(IFormatProvider provider, string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Warn(provider, format, args);
			}

			// Token: 0x060004A7 RID: 1191 RVA: 0x00013F5D File Offset: 0x0001235D
			public void Error(object message)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(message);
			}

			// Token: 0x060004A8 RID: 1192 RVA: 0x00013F6A File Offset: 0x0001236A
			public void Error(object message, Exception exception)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(exception, message.ToString(), new object[0]);
			}

			// Token: 0x060004A9 RID: 1193 RVA: 0x00013F83 File Offset: 0x00012383
			public void ErrorFormat(string format, object arg0)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(format, arg0);
			}

			// Token: 0x060004AA RID: 1194 RVA: 0x00013F91 File Offset: 0x00012391
			public void ErrorFormat(string format, object arg0, object arg1)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(format, arg0, arg1);
			}

			// Token: 0x060004AB RID: 1195 RVA: 0x00013FA0 File Offset: 0x000123A0
			public void ErrorFormat(string format, object arg0, object arg1, object arg2)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(format, arg0, arg1, arg2);
			}

			// Token: 0x060004AC RID: 1196 RVA: 0x00013FB1 File Offset: 0x000123B1
			public void ErrorFormat(string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(format, args);
			}

			// Token: 0x060004AD RID: 1197 RVA: 0x00013FBF File Offset: 0x000123BF
			public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Error(provider, format, args);
			}

			// Token: 0x060004AE RID: 1198 RVA: 0x00013FCE File Offset: 0x000123CE
			public void Fatal(object message)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(message);
			}

			// Token: 0x060004AF RID: 1199 RVA: 0x00013FDB File Offset: 0x000123DB
			public void Fatal(object message, Exception exception)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(exception, message.ToString(), new object[0]);
			}

			// Token: 0x060004B0 RID: 1200 RVA: 0x00013FF4 File Offset: 0x000123F4
			public void FatalFormat(string format, object arg0)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(format, arg0);
			}

			// Token: 0x060004B1 RID: 1201 RVA: 0x00014002 File Offset: 0x00012402
			public void FatalFormat(string format, object arg0, object arg1)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(format, arg0, arg1);
			}

			// Token: 0x060004B2 RID: 1202 RVA: 0x00014011 File Offset: 0x00012411
			public void FatalFormat(string format, object arg0, object arg1, object arg2)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(format, arg0, arg1, arg2);
			}

			// Token: 0x060004B3 RID: 1203 RVA: 0x00014022 File Offset: 0x00012422
			public void FatalFormat(string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(format, args);
			}

			// Token: 0x060004B4 RID: 1204 RVA: 0x00014030 File Offset: 0x00012430
			public void FatalFormat(IFormatProvider provider, string format, params object[] args)
			{
				MasterServerLoggerFactory.MasterServerLogger.Logger.Fatal(provider, format, args);
			}

			// Token: 0x040001FE RID: 510
			internal static readonly ILog Instance = new MasterServerLoggerFactory.MasterServerLogger();

			// Token: 0x040001FF RID: 511
			private static readonly ILogger Logger = NLog.LogManager.GetCurrentClassLogger();
		}
	}
}
