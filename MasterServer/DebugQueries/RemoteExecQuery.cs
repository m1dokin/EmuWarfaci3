using System;
using System.Text;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.DebugQueries
{
	// Token: 0x0200021F RID: 543
	[DebugQuery]
	[QueryAttributes(TagName = "remote_exec")]
	internal class RemoteExecQuery : BaseQuery
	{
		// Token: 0x06000BC2 RID: 3010 RVA: 0x0002CB40 File Offset: 0x0002AF40
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			string attribute = request.GetAttribute("command");
			bool flag;
			if (!bool.TryParse(request.GetAttribute("needs_response"), out flag))
			{
				flag = true;
			}
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(attribute));
			Log.Info<string, string>("Executing remote command '{0}' sent by {1}", @string, fromJid);
			RemoteExecQuery.RemoteExec remoteExec = new RemoteExecQuery.RemoteExec();
			string text = remoteExec.Execute(@string);
			response.SetAttribute("output", (!flag) ? string.Empty : text);
			return 0;
		}

		// Token: 0x02000220 RID: 544
		private class RemoteExec
		{
			// Token: 0x06000BC4 RID: 3012 RVA: 0x0002CBD0 File Offset: 0x0002AFD0
			public string Execute(string cmd)
			{
				Log.OnLogMessage += this.OnLogEvent;
				ConsoleCmdManager.ExecuteCmd(cmd);
				Log.OnLogMessage -= this.OnLogEvent;
				return this.m_messageBuilder.Replace(Environment.NewLine, ";nl").ToString();
			}

			// Token: 0x06000BC5 RID: 3013 RVA: 0x0002CC20 File Offset: 0x0002B020
			private void OnLogEvent(string line)
			{
				lock (this)
				{
					if (this.m_messageBuilder.Length > 0)
					{
						this.m_messageBuilder.AppendFormat("{0}{1}", Environment.NewLine, line);
					}
					else
					{
						this.m_messageBuilder.Append(line);
					}
				}
			}

			// Token: 0x04000579 RID: 1401
			private readonly StringBuilder m_messageBuilder = new StringBuilder();
		}
	}
}
