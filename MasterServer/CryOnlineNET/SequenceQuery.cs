using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A1 RID: 417
	internal abstract class SequenceQuery : BaseQuery
	{
		// Token: 0x060007CB RID: 1995 RVA: 0x0001DC19 File Offset: 0x0001C019
		public SequenceQuery(ISequenceQueryCache queryCacheService)
		{
			this.m_queryCacheService = queryCacheService;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x0001DC28 File Offset: 0x0001C028
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SequenceQuery"))
			{
				if (!request.HasAttribute("size"))
				{
					Log.Error<string, string>("SequenceQuery {0} has not attribute 'size', sent from {1}", base.Tag, fromJid);
					result = -1;
				}
				else if (!request.HasAttribute("received"))
				{
					Log.Error<string, string>("SequenceQuery {0} has not attribute 'received', sent from {1}", base.Tag, fromJid);
					result = -1;
				}
				else
				{
					string attribute = request.GetAttribute("size");
					string attribute2 = request.GetAttribute("received");
					int num = int.Parse(attribute2);
					int num2 = int.Parse(attribute);
					int num3 = 0;
					bool flag = false;
					if (request.HasAttribute("token"))
					{
						string attribute3 = request.GetAttribute("token");
						num3 = int.Parse(attribute3);
					}
					if (request.HasAttribute("cancelled"))
					{
						string attribute4 = request.GetAttribute("cancelled");
						flag = (int.Parse(attribute4) != 0);
					}
					if (flag)
					{
						if (num3 != 0)
						{
							this.m_queryCacheService.FreeData(num3, this.GetTokenName(fromJid));
						}
						response.SetAttribute("left", "0");
						response.SetAttribute("token", "0");
						Log.Info<string, string>("SequenceQuery {0} was cancelled from {1}", base.Tag, fromJid);
						result = 0;
					}
					else if (request.HasAttribute("hash") && this.GetTokenHash(fromJid) == request.GetAttribute("hash"))
					{
						Log.Verbose("SequenceQuery {0} finished since {1} has local data with the same cache", new object[]
						{
							base.Tag,
							fromJid
						});
						result = 0;
					}
					else
					{
						List<XmlElement> list;
						num3 = this.m_queryCacheService.GetData(num3, this.GetTokenName(fromJid), out list);
						if (num3 == 0)
						{
							num3 = this.CreateSequenceData(request, out list, fromJid);
						}
						if (num3 == 0)
						{
							Log.Warning("Failed to generate sequence data");
							result = -1;
						}
						else
						{
							int num4 = Math.Max(0, list.Count - (num + num2));
							response.SetAttribute("left", num4.ToString());
							response.SetAttribute("token", num3.ToString());
							if (num < list.Count)
							{
								int num5 = num;
								while (num5 < num + num2 && num5 < list.Count)
								{
									response.AppendChild(response.OwnerDocument.ImportNode(list[num5], true));
									num5++;
								}
							}
							if (num4 == 0)
							{
								this.m_queryCacheService.FreeData(num3, this.GetTokenName(fromJid));
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060007CD RID: 1997
		protected abstract int CreateSequenceData(XmlElement request, out List<XmlElement> sequenceData, string fromJid);

		// Token: 0x060007CE RID: 1998
		protected abstract string GetTokenName(string fromJid);

		// Token: 0x060007CF RID: 1999 RVA: 0x0001DEE4 File Offset: 0x0001C2E4
		protected virtual string GetTokenHash(string fromJid)
		{
			return string.Empty;
		}

		// Token: 0x04000497 RID: 1175
		protected readonly ISequenceQueryCache m_queryCacheService;
	}
}
