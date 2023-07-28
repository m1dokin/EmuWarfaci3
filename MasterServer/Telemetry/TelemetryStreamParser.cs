using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using StatsDataSource.ObjectModel;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x020007CB RID: 1995
	internal class TelemetryStreamParser : IDisposable
	{
		// Token: 0x060028D4 RID: 10452 RVA: 0x000B11F5 File Offset: 0x000AF5F5
		public TelemetryStreamParser(StatsRepository repo, TextReader reader)
		{
			this.m_repository = repo;
			this.m_reader = reader;
		}

		// Token: 0x060028D5 RID: 10453 RVA: 0x000B120C File Offset: 0x000AF60C
		public void Parse()
		{
			this.m_repository.StartModificationBatch(false);
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			while ((text = this.m_reader.ReadLine()) != null)
			{
				if (text.StartsWith("{"))
				{
					if (stringBuilder.Length > 0)
					{
						this.OnDataMessage(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
					}
					TelemetryStreamParser.OpCode opCode;
					Locator locator;
					this.ParseNavMessage(text, out opCode, out locator);
					if (opCode == TelemetryStreamParser.OpCode.eOC_Visit)
					{
						this.OnNavEnter(locator);
					}
					else
					{
						this.OnNavLeave(locator);
					}
				}
				else
				{
					stringBuilder.Append(text);
				}
			}
			this.m_repository.EndModificationBatch();
		}

		// Token: 0x060028D6 RID: 10454 RVA: 0x000B12B4 File Offset: 0x000AF6B4
		private void ParseNavMessage(string msg, out TelemetryStreamParser.OpCode opcode, out Locator locator)
		{
			int num = msg.IndexOf('}') + 1;
			string s = msg.Substring(1, num - 2);
			opcode = (TelemetryStreamParser.OpCode)int.Parse(s);
			int num2 = msg.IndexOf('[');
			int num3 = msg.IndexOf(']');
			string s2 = msg.Substring(num2, num3 - num2 + 1);
			locator = Locator.FromString(s2);
		}

		// Token: 0x060028D7 RID: 10455 RVA: 0x000B130C File Offset: 0x000AF70C
		public void Dispose()
		{
			this.m_reader.Close();
		}

		// Token: 0x060028D8 RID: 10456 RVA: 0x000B131C File Offset: 0x000AF71C
		private void OnNavEnter(Locator locator)
		{
			if (this.m_currentNode == null)
			{
				this.m_currentNode = this.m_repository.FindNode(locator);
				if (this.m_currentNode == null)
				{
					this.m_currentNode = this.m_repository.AddRootScope(locator);
				}
			}
			else
			{
				GameScope gameScope = (GameScope)this.m_currentNode;
				GameNode gameNode = gameScope.FindChild(locator);
				if (gameNode == null)
				{
					gameNode = gameScope.AddChild(locator);
				}
				this.m_currentNode = gameNode;
			}
		}

		// Token: 0x060028D9 RID: 10457 RVA: 0x000B1391 File Offset: 0x000AF791
		private void OnNavLeave(Locator locator)
		{
			this.m_currentNode = this.m_currentNode.Parent;
		}

		// Token: 0x060028DA RID: 10458 RVA: 0x000B13A4 File Offset: 0x000AF7A4
		private void OnDataMessage(string message)
		{
			using (StringReader stringReader = new StringReader(message))
			{
				using (XmlReader xmlReader = XmlReader.Create(stringReader))
				{
					int num = -1;
					while (xmlReader.Read())
					{
						if (num == -1 && xmlReader.NodeType == XmlNodeType.Element && !this.VisitXmlElement(xmlReader))
						{
							num = xmlReader.Depth;
						}
						if (xmlReader.IsEmptyElement || xmlReader.NodeType == XmlNodeType.EndElement)
						{
							if (num == -1)
							{
								this.LeaveXmlElement(xmlReader);
							}
							if (xmlReader.Depth == num)
							{
								num = -1;
							}
						}
					}
				}
			}
		}

		// Token: 0x060028DB RID: 10459 RVA: 0x000B146C File Offset: 0x000AF86C
		private bool VisitXmlElement(XmlReader xml)
		{
			if (xml.Depth == 0)
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Node;
				this.ExtractStatesFromAttributes(xml);
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Node && xml.Name == "timelines")
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Timelines;
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Node)
			{
				this.OnStateHierarchy(xml);
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Timelines)
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Timeline;
				this.OnEventGroup(xml);
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Timeline)
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Value;
				this.OnEventValue(xml);
			}
			return true;
		}

		// Token: 0x060028DC RID: 10460 RVA: 0x000B1514 File Offset: 0x000AF914
		private void LeaveXmlElement(XmlReader xml)
		{
			if (this.m_currentContext == TelemetryStreamParser.EContext.Node)
			{
				if (this.m_currentState != null)
				{
					this.m_currentState = this.m_currentState.Parent;
				}
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Timelines)
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Node;
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Timeline)
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Timelines;
				this.m_currentEvntGroup = null;
			}
			else if (this.m_currentContext == TelemetryStreamParser.EContext.Value)
			{
				this.m_currentContext = TelemetryStreamParser.EContext.Timeline;
			}
		}

		// Token: 0x060028DD RID: 10461 RVA: 0x000B1598 File Offset: 0x000AF998
		private void ExtractStatesFromAttributes(XmlReader xml)
		{
			for (int num = 0; num != xml.AttributeCount; num++)
			{
				xml.MoveToAttribute(num);
				StatDesc desc = this.m_repository.Registry.RegisterState(xml.Name);
				GameState gameState = this.m_currentNode.FindState(desc);
				this.AddOrSetState(this.m_currentNode, desc, xml.Value);
			}
			if (xml.AttributeCount != 0)
			{
				xml.MoveToElement();
			}
		}

		// Token: 0x060028DE RID: 10462 RVA: 0x000B1610 File Offset: 0x000AFA10
		private void OnStateHierarchy(XmlReader xml)
		{
			StatDesc desc = this.m_repository.Registry.RegisterState(xml.Name);
			this.m_currentState = ((this.m_currentState == null) ? this.AddOrSetState(this.m_currentNode, desc, null) : this.m_currentState.AddChild(desc, null));
			for (int num = 0; num != xml.AttributeCount; num++)
			{
				xml.MoveToAttribute(num);
				desc = this.m_repository.Registry.RegisterState(xml.Name);
				this.AddOrSetState(this.m_currentState, desc, xml.Value);
			}
			if (xml.AttributeCount != 0)
			{
				xml.MoveToElement();
			}
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x000B16C0 File Offset: 0x000AFAC0
		private void OnEventGroup(XmlReader xml)
		{
			string attribute = xml.GetAttribute(0);
			StatDesc desc = this.m_repository.Registry.RegisterEvent(attribute);
			this.m_currentEvntGroup = this.AddOrGetEventGroup(this.m_currentNode, desc);
		}

		// Token: 0x060028E0 RID: 10464 RVA: 0x000B16FC File Offset: 0x000AFAFC
		private void OnEventValue(XmlReader xml)
		{
			long time = long.Parse(xml.GetAttribute("time"));
			KeyValuePair<string, string>[] array;
			if (xml.AttributeCount <= 1 && !xml.IsEmptyElement)
			{
				while (xml.Read())
				{
					if (xml.NodeType == XmlNodeType.Element || !(xml.Name != "param"))
					{
						array = new KeyValuePair<string, string>[xml.AttributeCount];
						for (int num = 0; num != xml.AttributeCount; num++)
						{
							xml.MoveToAttribute(num);
							array[num] = new KeyValuePair<string, string>(xml.Name, xml.Value);
						}
						xml.MoveToElement();
						while (xml.Read())
						{
							if (xml.NodeType == XmlNodeType.EndElement || !(xml.Name != "val"))
							{
								goto IL_12C;
							}
						}
						throw new FormatException("Malformed telemetry XML");
					}
				}
				throw new FormatException("Malformed telemetry XML");
			}
			if (xml.AttributeCount > 1)
			{
				string attribute = xml.GetAttribute("prm");
				array = new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("prm", attribute)
				};
			}
			else
			{
				array = new KeyValuePair<string, string>[0];
			}
			IL_12C:
			this.m_currentEvntGroup.AddEvent(time, array);
		}

		// Token: 0x060028E1 RID: 10465 RVA: 0x000B1844 File Offset: 0x000AFC44
		private GameState AddOrSetState(GameNode node, StatDesc desc, string value)
		{
			GameState gameState = node.FindState(desc);
			if (gameState == null)
			{
				return node.AddState(desc, value);
			}
			gameState.Value = value;
			return gameState;
		}

		// Token: 0x060028E2 RID: 10466 RVA: 0x000B1870 File Offset: 0x000AFC70
		private GameState AddOrSetState(GameState state, StatDesc desc, string value)
		{
			GameState gameState = state.FindChild(desc.Name);
			if (gameState == null)
			{
				return state.AddChild(desc, value);
			}
			gameState.Value = value;
			return gameState;
		}

		// Token: 0x060028E3 RID: 10467 RVA: 0x000B18A4 File Offset: 0x000AFCA4
		private GameEventGroup AddOrGetEventGroup(GameNode node, StatDesc desc)
		{
			GameEventGroup gameEventGroup = node.FindEventGroup(desc);
			return (gameEventGroup != null) ? gameEventGroup : node.AddEventGroup(desc);
		}

		// Token: 0x040015B8 RID: 5560
		private StatsRepository m_repository;

		// Token: 0x040015B9 RID: 5561
		private TextReader m_reader;

		// Token: 0x040015BA RID: 5562
		private GameNode m_currentNode;

		// Token: 0x040015BB RID: 5563
		private TelemetryStreamParser.EContext m_currentContext;

		// Token: 0x040015BC RID: 5564
		private GameState m_currentState;

		// Token: 0x040015BD RID: 5565
		private GameEventGroup m_currentEvntGroup;

		// Token: 0x020007CC RID: 1996
		private enum OpCode
		{
			// Token: 0x040015BF RID: 5567
			eOC_Visit,
			// Token: 0x040015C0 RID: 5568
			eOC_Leave
		}

		// Token: 0x020007CD RID: 1997
		private enum EContext
		{
			// Token: 0x040015C2 RID: 5570
			Node,
			// Token: 0x040015C3 RID: 5571
			Timelines,
			// Token: 0x040015C4 RID: 5572
			Timeline,
			// Token: 0x040015C5 RID: 5573
			Value
		}
	}
}
