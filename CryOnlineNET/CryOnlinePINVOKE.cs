using System;
using System.IO;
using System.Runtime.InteropServices;

// Token: 0x02000004 RID: 4
internal class CryOnlinePINVOKE
{
	// Token: 0x0600000E RID: 14
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_QUERY_DESC_LEN_get")]
	public static extern int SBaseQuery_QUERY_DESC_LEN_get();

	// Token: 0x0600000F RID: 15
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_QUERY_BUF_LEN_get")]
	public static extern int SBaseQuery_QUERY_BUF_LEN_get();

	// Token: 0x06000010 RID: 16
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_sId_set")]
	public static extern void SBaseQuery_sId_set(HandleRef jarg1, string jarg2);

	// Token: 0x06000011 RID: 17
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_sId_get")]
	public static extern string SBaseQuery_sId_get(HandleRef jarg1);

	// Token: 0x06000012 RID: 18
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_online_id_set")]
	public static extern void SBaseQuery_online_id_set(HandleRef jarg1, string jarg2);

	// Token: 0x06000013 RID: 19
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_online_id_get")]
	public static extern string SBaseQuery_online_id_get(HandleRef jarg1);

	// Token: 0x06000014 RID: 20
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_tag_set")]
	public static extern void SBaseQuery_tag_set(HandleRef jarg1, string jarg2);

	// Token: 0x06000015 RID: 21
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_tag_get")]
	public static extern string SBaseQuery_tag_get(HandleRef jarg1);

	// Token: 0x06000016 RID: 22
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_description_set")]
	public static extern void SBaseQuery_description_set(HandleRef jarg1, string jarg2);

	// Token: 0x06000017 RID: 23
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_description_get")]
	public static extern string SBaseQuery_description_get(HandleRef jarg1);

	// Token: 0x06000018 RID: 24
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_id_set")]
	public static extern void SBaseQuery_id_set(HandleRef jarg1, int jarg2);

	// Token: 0x06000019 RID: 25
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_id_get")]
	public static extern int SBaseQuery_id_get(HandleRef jarg1);

	// Token: 0x0600001A RID: 26
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_SBaseQuery")]
	public static extern IntPtr new_SBaseQuery();

	// Token: 0x0600001B RID: 27
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_SetSID")]
	public static extern void SBaseQuery_SetSID(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600001C RID: 28
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_SetOnlineID")]
	public static extern void SBaseQuery_SetOnlineID(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600001D RID: 29
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_SetTag")]
	public static extern void SBaseQuery_SetTag(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600001E RID: 30
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SBaseQuery_SetDescription")]
	public static extern void SBaseQuery_SetDescription(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600001F RID: 31
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_SBaseQuery")]
	public static extern void delete_SBaseQuery(HandleRef jarg1);

	// Token: 0x06000020 RID: 32
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SOnlineQuery_type_set")]
	public static extern void SOnlineQuery_type_set(HandleRef jarg1, int jarg2);

	// Token: 0x06000021 RID: 33
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SOnlineQuery_type_get")]
	public static extern int SOnlineQuery_type_get(HandleRef jarg1);

	// Token: 0x06000022 RID: 34
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_SOnlineQuery")]
	public static extern IntPtr new_SOnlineQuery();

	// Token: 0x06000023 RID: 35
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_SOnlineQuery")]
	public static extern void delete_SOnlineQuery(HandleRef jarg1);

	// Token: 0x06000024 RID: 36
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryError_online_error_set")]
	public static extern void SQueryError_online_error_set(HandleRef jarg1, int jarg2);

	// Token: 0x06000025 RID: 37
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryError_online_error_get")]
	public static extern int SQueryError_online_error_get(HandleRef jarg1);

	// Token: 0x06000026 RID: 38
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryError_custom_code_set")]
	public static extern void SQueryError_custom_code_set(HandleRef jarg1, int jarg2);

	// Token: 0x06000027 RID: 39
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryError_custom_code_get")]
	public static extern int SQueryError_custom_code_get(HandleRef jarg1);

	// Token: 0x06000028 RID: 40
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_SQueryError")]
	public static extern IntPtr new_SQueryError();

	// Token: 0x06000029 RID: 41
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_SQueryError")]
	public static extern void delete_SQueryError(HandleRef jarg1);

	// Token: 0x0600002A RID: 42
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_NAME_LEN_get")]
	public static extern int SQueryStats_NAME_LEN_get();

	// Token: 0x0600002B RID: 43
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_Query_set")]
	public static extern void SQueryStats_Query_set(HandleRef jarg1, string jarg2);

	// Token: 0x0600002C RID: 44
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_Query_get")]
	public static extern string SQueryStats_Query_get(HandleRef jarg1);

	// Token: 0x0600002D RID: 45
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_ResponseTime_set")]
	public static extern void SQueryStats_ResponseTime_set(HandleRef jarg1, ulong jarg2);

	// Token: 0x0600002E RID: 46
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_ResponseTime_get")]
	public static extern ulong SQueryStats_ResponseTime_get(HandleRef jarg1);

	// Token: 0x0600002F RID: 47
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_InboundDataSize_set")]
	public static extern void SQueryStats_InboundDataSize_set(HandleRef jarg1, uint jarg2);

	// Token: 0x06000030 RID: 48
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_InboundDataSize_get")]
	public static extern uint SQueryStats_InboundDataSize_get(HandleRef jarg1);

	// Token: 0x06000031 RID: 49
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_InboundCompressedSize_set")]
	public static extern void SQueryStats_InboundCompressedSize_set(HandleRef jarg1, uint jarg2);

	// Token: 0x06000032 RID: 50
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_InboundCompressedSize_get")]
	public static extern uint SQueryStats_InboundCompressedSize_get(HandleRef jarg1);

	// Token: 0x06000033 RID: 51
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_OutboundDataSize_set")]
	public static extern void SQueryStats_OutboundDataSize_set(HandleRef jarg1, uint jarg2);

	// Token: 0x06000034 RID: 52
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_OutboundDataSize_get")]
	public static extern uint SQueryStats_OutboundDataSize_get(HandleRef jarg1);

	// Token: 0x06000035 RID: 53
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_OutboundCompressedSize_set")]
	public static extern void SQueryStats_OutboundCompressedSize_set(HandleRef jarg1, uint jarg2);

	// Token: 0x06000036 RID: 54
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_OutboundCompressedSize_get")]
	public static extern uint SQueryStats_OutboundCompressedSize_get(HandleRef jarg1);

	// Token: 0x06000037 RID: 55
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_Request_set")]
	public static extern void SQueryStats_Request_set(HandleRef jarg1, bool jarg2);

	// Token: 0x06000038 RID: 56
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_Request_get")]
	public static extern bool SQueryStats_Request_get(HandleRef jarg1);

	// Token: 0x06000039 RID: 57
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_Failed_set")]
	public static extern void SQueryStats_Failed_set(HandleRef jarg1, bool jarg2);

	// Token: 0x0600003A RID: 58
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryStats_Failed_get")]
	public static extern bool SQueryStats_Failed_get(HandleRef jarg1);

	// Token: 0x0600003B RID: 59
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_SQueryStats")]
	public static extern IntPtr new_SQueryStats();

	// Token: 0x0600003C RID: 60
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_SQueryStats")]
	public static extern void delete_SQueryStats(HandleRef jarg1);

	// Token: 0x0600003D RID: 61
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IXmlNodeHandler_GetTag")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IXmlNodeHandler_GetTag(HandleRef jarg1);

	// Token: 0x0600003E RID: 62
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IXmlNodeHandler_GetContent")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IXmlNodeHandler_GetContent(HandleRef jarg1);

	// Token: 0x0600003F RID: 63
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IXmlNodeHandler_GetNumAttributes")]
	public static extern int IXmlNodeHandler_GetNumAttributes(HandleRef jarg1);

	// Token: 0x06000040 RID: 64
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IXmlNodeHandler_GetAttributeByIndex")]
	public static extern bool IXmlNodeHandler_GetAttributeByIndex(HandleRef jarg1, int jarg2, HandleRef jarg3, HandleRef jarg4);

	// Token: 0x06000041 RID: 65
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IXmlNodeHandler_GetChildCount")]
	public static extern int IXmlNodeHandler_GetChildCount(HandleRef jarg1);

	// Token: 0x06000042 RID: 66
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IXmlNodeHandler_GetChild")]
	public static extern IntPtr IXmlNodeHandler_GetChild(HandleRef jarg1, int jarg2);

	// Token: 0x06000043 RID: 67
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IXmlNodeHandler")]
	public static extern void delete_IXmlNodeHandler(HandleRef jarg1);

	// Token: 0x06000044 RID: 68
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineBandwidthStats")]
	public static extern void delete_IOnlineBandwidthStats(HandleRef jarg1);

	// Token: 0x06000045 RID: 69
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineBandwidthStats_GetTotalBytesSent")]
	public static extern int IOnlineBandwidthStats_GetTotalBytesSent(HandleRef jarg1);

	// Token: 0x06000046 RID: 70
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineBandwidthStats_GetTotalBytesRecv")]
	public static extern int IOnlineBandwidthStats_GetTotalBytesRecv(HandleRef jarg1);

	// Token: 0x06000047 RID: 71
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineBandwidthStats")]
	public static extern IntPtr new_IOnlineBandwidthStats();

	// Token: 0x06000048 RID: 72
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineBandwidthStats_director_connect")]
	public static extern void IOnlineBandwidthStats_director_connect(HandleRef jarg1, IOnlineBandwidthStats.SwigDelegateIOnlineBandwidthStats_0 delegate0, IOnlineBandwidthStats.SwigDelegateIOnlineBandwidthStats_1 delegate1);

	// Token: 0x06000049 RID: 73
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineQueryStatsListener")]
	public static extern void delete_IOnlineQueryStatsListener(HandleRef jarg1);

	// Token: 0x0600004A RID: 74
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryStatsListener_OnQueryStats")]
	public static extern void IOnlineQueryStatsListener_OnQueryStats(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x0600004B RID: 75
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineQueryStatsListener")]
	public static extern IntPtr new_IOnlineQueryStatsListener();

	// Token: 0x0600004C RID: 76
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryStatsListener_director_connect")]
	public static extern void IOnlineQueryStatsListener_director_connect(HandleRef jarg1, IOnlineQueryStatsListener.SwigDelegateIOnlineQueryStatsListener_0 delegate0);

	// Token: 0x0600004D RID: 77
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineConnectionListener")]
	public static extern void delete_IOnlineConnectionListener(HandleRef jarg1);

	// Token: 0x0600004E RID: 78
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnectionListener_OnConnectionAvailable")]
	public static extern void IOnlineConnectionListener_OnConnectionAvailable(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x0600004F RID: 79
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnectionListener_OnConnectionLost")]
	public static extern void IOnlineConnectionListener_OnConnectionLost(HandleRef jarg1, HandleRef jarg2, int jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg4);

	// Token: 0x06000050 RID: 80
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnectionListener_OnConnectionTick")]
	public static extern void IOnlineConnectionListener_OnConnectionTick(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x06000051 RID: 81
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnectionListener_OnPresence")]
	public static extern void IOnlineConnectionListener_OnPresence(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, int jarg4);

	// Token: 0x06000052 RID: 82
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnectionListener_OnUserStatus")]
	public static extern void IOnlineConnectionListener_OnUserStatus(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg4, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg5);

	// Token: 0x06000053 RID: 83
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineConnectionListener")]
	public static extern IntPtr new_IOnlineConnectionListener();

	// Token: 0x06000054 RID: 84
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnectionListener_director_connect")]
	public static extern void IOnlineConnectionListener_director_connect(HandleRef jarg1, IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_0 delegate0, IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_1 delegate1, IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_2 delegate2, IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_3 delegate3, IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_4 delegate4);

	// Token: 0x06000055 RID: 85
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineQueryBinder")]
	public static extern void delete_IOnlineQueryBinder(HandleRef jarg1);

	// Token: 0x06000056 RID: 86
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_Tag")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineQueryBinder_Tag(HandleRef jarg1);

	// Token: 0x06000057 RID: 87
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_GetCompressionType")]
	public static extern int IOnlineQueryBinder_GetCompressionType(HandleRef jarg1);

	// Token: 0x06000058 RID: 88
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_GetReceiverId")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineQueryBinder_GetReceiverId(HandleRef jarg1);

	// Token: 0x06000059 RID: 89
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_OnQueryCompleted")]
	public static extern void IOnlineQueryBinder_OnQueryCompleted(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg4);

	// Token: 0x0600005A RID: 90
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_OnRequest")]
	public static extern void IOnlineQueryBinder_OnRequest(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg4);

	// Token: 0x0600005B RID: 91
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_OnQueryError")]
	public static extern void IOnlineQueryBinder_OnQueryError(HandleRef jarg1, HandleRef jarg2, HandleRef jarg3);

	// Token: 0x0600005C RID: 92
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineQueryBinder")]
	public static extern IntPtr new_IOnlineQueryBinder();

	// Token: 0x0600005D RID: 93
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineQueryBinder_director_connect")]
	public static extern void IOnlineQueryBinder_director_connect(HandleRef jarg1, IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_0 delegate0, IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_1 delegate1, IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_2 delegate2, IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_3 delegate3, IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_4 delegate4, IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_5 delegate5);

	// Token: 0x0600005E RID: 94
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineConfiguration")]
	public static extern void delete_IOnlineConfiguration(HandleRef jarg1);

	// Token: 0x0600005F RID: 95
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetDomain")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetDomain(HandleRef jarg1);

	// Token: 0x06000060 RID: 96
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetDomain")]
	public static extern void IOnlineConfiguration_SetDomain(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x06000061 RID: 97
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetServer")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetServer(HandleRef jarg1);

	// Token: 0x06000062 RID: 98
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetServer")]
	public static extern void IOnlineConfiguration_SetServer(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x06000063 RID: 99
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetServerPort")]
	public static extern int IOnlineConfiguration_GetServerPort(HandleRef jarg1);

	// Token: 0x06000064 RID: 100
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetServerPort")]
	public static extern void IOnlineConfiguration_SetServerPort(HandleRef jarg1, int jarg2);

	// Token: 0x06000065 RID: 101
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetHost")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetHost(HandleRef jarg1);

	// Token: 0x06000066 RID: 102
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetHost")]
	public static extern void IOnlineConfiguration_SetHost(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x06000067 RID: 103
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetResource")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetResource(HandleRef jarg1);

	// Token: 0x06000068 RID: 104
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetResource")]
	public static extern void IOnlineConfiguration_SetResource(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x06000069 RID: 105
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetPassword")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetPassword(HandleRef jarg1);

	// Token: 0x0600006A RID: 106
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetPassword")]
	public static extern void IOnlineConfiguration_SetPassword(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600006B RID: 107
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetOnlineId")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetOnlineId(HandleRef jarg1);

	// Token: 0x0600006C RID: 108
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetOnlineId")]
	public static extern void IOnlineConfiguration_SetOnlineId(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600006D RID: 109
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetFSProxy")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetFSProxy(HandleRef jarg1);

	// Token: 0x0600006E RID: 110
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetFSProxy")]
	public static extern void IOnlineConfiguration_SetFSProxy(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600006F RID: 111
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetTLSPolicy")]
	public static extern int IOnlineConfiguration_GetTLSPolicy(HandleRef jarg1);

	// Token: 0x06000070 RID: 112
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetTLSPolicy")]
	public static extern void IOnlineConfiguration_SetTLSPolicy(HandleRef jarg1, int jarg2);

	// Token: 0x06000071 RID: 113
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetFSProxyPort")]
	public static extern int IOnlineConfiguration_GetFSProxyPort(HandleRef jarg1);

	// Token: 0x06000072 RID: 114
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetFSProxyPort")]
	public static extern void IOnlineConfiguration_SetFSProxyPort(HandleRef jarg1, int jarg2);

	// Token: 0x06000073 RID: 115
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetDefaultCompression")]
	public static extern void IOnlineConfiguration_SetDefaultCompression(HandleRef jarg1, int jarg2);

	// Token: 0x06000074 RID: 116
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetDefaultCompression")]
	public static extern int IOnlineConfiguration_GetDefaultCompression(HandleRef jarg1);

	// Token: 0x06000075 RID: 117
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetSendDelay")]
	public static extern void IOnlineConfiguration_SetSendDelay(HandleRef jarg1, int jarg2);

	// Token: 0x06000076 RID: 118
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetSendDelay")]
	public static extern int IOnlineConfiguration_GetSendDelay(HandleRef jarg1);

	// Token: 0x06000077 RID: 119
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetTcpReceiveBufferSize")]
	public static extern void IOnlineConfiguration_SetTcpReceiveBufferSize(HandleRef jarg1, int jarg2);

	// Token: 0x06000078 RID: 120
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetTcpReceiveBufferSize")]
	public static extern int IOnlineConfiguration_GetTcpReceiveBufferSize(HandleRef jarg1);

	// Token: 0x06000079 RID: 121
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetFullOnlineId")]
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public static extern string IOnlineConfiguration_GetFullOnlineId(HandleRef jarg1);

	// Token: 0x0600007A RID: 122
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetOnlineVerbose")]
	public static extern void IOnlineConfiguration_SetOnlineVerbose(HandleRef jarg1, int jarg2);

	// Token: 0x0600007B RID: 123
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_IsOnlineVerbose")]
	public static extern bool IOnlineConfiguration_IsOnlineVerbose(HandleRef jarg1);

	// Token: 0x0600007C RID: 124
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_SetThreadMode")]
	public static extern void IOnlineConfiguration_SetThreadMode(HandleRef jarg1, int jarg2);

	// Token: 0x0600007D RID: 125
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConfiguration_GetThreadMode")]
	public static extern int IOnlineConfiguration_GetThreadMode(HandleRef jarg1);

	// Token: 0x0600007E RID: 126
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineChatRoomListener")]
	public static extern void delete_IOnlineChatRoomListener(HandleRef jarg1);

	// Token: 0x0600007F RID: 127
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoomListener_OnChatRoomMessage")]
	public static extern void IOnlineChatRoomListener_OnChatRoomMessage(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, bool jarg4);

	// Token: 0x06000080 RID: 128
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoomListener_OnChatRoomPresence")]
	public static extern void IOnlineChatRoomListener_OnChatRoomPresence(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, int jarg3);

	// Token: 0x06000081 RID: 129
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoomListener_OnChatRoomParticipant")]
	public static extern void IOnlineChatRoomListener_OnChatRoomParticipant(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, bool jarg3);

	// Token: 0x06000082 RID: 130
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineChatRoomListener")]
	public static extern IntPtr new_IOnlineChatRoomListener();

	// Token: 0x06000083 RID: 131
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoomListener_director_connect")]
	public static extern void IOnlineChatRoomListener_director_connect(HandleRef jarg1, IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_0 delegate0, IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_1 delegate1, IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_2 delegate2);

	// Token: 0x06000084 RID: 132
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineChatRoom")]
	public static extern void delete_IOnlineChatRoom(HandleRef jarg1);

	// Token: 0x06000085 RID: 133
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_RegisterListener")]
	public static extern bool IOnlineChatRoom_RegisterListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x06000086 RID: 134
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_UnregisterListener")]
	public static extern bool IOnlineChatRoom_UnregisterListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x06000087 RID: 135
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_IsListenersEmpty")]
	public static extern bool IOnlineChatRoom_IsListenersEmpty(HandleRef jarg1);

	// Token: 0x06000088 RID: 136
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_Join")]
	public static extern void IOnlineChatRoom_Join(HandleRef jarg1);

	// Token: 0x06000089 RID: 137
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_Leave")]
	public static extern void IOnlineChatRoom_Leave(HandleRef jarg1);

	// Token: 0x0600008A RID: 138
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_EnumerateParticipants")]
	public static extern void IOnlineChatRoom_EnumerateParticipants(HandleRef jarg1);

	// Token: 0x0600008B RID: 139
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_Send")]
	public static extern void IOnlineChatRoom_Send(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600008C RID: 140
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatRoom_SendPrivate")]
	public static extern void IOnlineChatRoom_SendPrivate(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x0600008D RID: 141
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineChatListener")]
	public static extern void delete_IOnlineChatListener(HandleRef jarg1);

	// Token: 0x0600008E RID: 142
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatListener_OnChatRoomDiscovered")]
	public static extern void IOnlineChatListener_OnChatRoomDiscovered(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x0600008F RID: 143
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineChatListener")]
	public static extern IntPtr new_IOnlineChatListener();

	// Token: 0x06000090 RID: 144
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChatListener_director_connect")]
	public static extern void IOnlineChatListener_director_connect(HandleRef jarg1, IOnlineChatListener.SwigDelegateIOnlineChatListener_0 delegate0);

	// Token: 0x06000091 RID: 145
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChat_RegisterListener")]
	public static extern bool IOnlineChat_RegisterListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x06000092 RID: 146
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChat_UnregisterListener")]
	public static extern bool IOnlineChat_UnregisterListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x06000093 RID: 147
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChat_DiscoverChatRooms")]
	public static extern void IOnlineChat_DiscoverChatRooms(HandleRef jarg1);

	// Token: 0x06000094 RID: 148
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChat_GetChatRoom__SWIG_0")]
	public static extern IntPtr IOnlineChat_GetChatRoom__SWIG_0(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x06000095 RID: 149
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineChat_GetChatRoom__SWIG_1")]
	public static extern IntPtr IOnlineChat_GetChatRoom__SWIG_1(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x06000096 RID: 150
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineChat")]
	public static extern void delete_IOnlineChat(HandleRef jarg1);

	// Token: 0x06000097 RID: 151
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineDataProtect")]
	public static extern void delete_IOnlineDataProtect(HandleRef jarg1);

	// Token: 0x06000098 RID: 152
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineDataProtect_protectData")]
	public static extern int IOnlineDataProtect_protectData(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, uint jarg4, string jarg5, HandleRef jarg6);

	// Token: 0x06000099 RID: 153
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineDataProtect_unprotectData")]
	public static extern int IOnlineDataProtect_unprotectData(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, uint jarg3, HandleRef jarg4, string jarg5, HandleRef jarg6);

	// Token: 0x0600009A RID: 154
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineDataProtect")]
	public static extern IntPtr new_IOnlineDataProtect();

	// Token: 0x0600009B RID: 155
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineDataProtect_director_connect")]
	public static extern void IOnlineDataProtect_director_connect(HandleRef jarg1, IOnlineDataProtect.SwigDelegateIOnlineDataProtect_0 delegate0, IOnlineDataProtect.SwigDelegateIOnlineDataProtect_1 delegate1);

	// Token: 0x0600009C RID: 156
	[DllImport("CryOnline.so", EntryPoint = "CSharp_ICertificateValidator_Query")]
	public static extern bool ICertificateValidator_Query(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x0600009D RID: 157
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_ICertificateValidator")]
	public static extern void delete_ICertificateValidator(HandleRef jarg1);

	// Token: 0x0600009E RID: 158
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Connect__SWIG_0")]
	public static extern bool IOnlineConnection_Connect__SWIG_0(HandleRef jarg1, int jarg2);

	// Token: 0x0600009F RID: 159
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Connect__SWIG_1")]
	public static extern bool IOnlineConnection_Connect__SWIG_1(HandleRef jarg1);

	// Token: 0x060000A0 RID: 160
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Query__SWIG_0")]
	public static extern int IOnlineConnection_Query__SWIG_0(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x060000A1 RID: 161
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Query__SWIG_1")]
	public static extern int IOnlineConnection_Query__SWIG_1(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x060000A2 RID: 162
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Query__SWIG_2")]
	public static extern int IOnlineConnection_Query__SWIG_2(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, HandleRef jarg4, int jarg5);

	// Token: 0x060000A3 RID: 163
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Query__SWIG_3")]
	public static extern int IOnlineConnection_Query__SWIG_3(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, HandleRef jarg4, int jarg5, int jarg6);

	// Token: 0x060000A4 RID: 164
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Query__SWIG_4")]
	public static extern int IOnlineConnection_Query__SWIG_4(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, HandleRef jarg4, int jarg5);

	// Token: 0x060000A5 RID: 165
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_Response")]
	public static extern void IOnlineConnection_Response(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, int jarg4);

	// Token: 0x060000A6 RID: 166
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_ResponseError")]
	public static extern void IOnlineConnection_ResponseError(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x060000A7 RID: 167
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_ScheduleFailedQuery")]
	public static extern void IOnlineConnection_ScheduleFailedQuery(HandleRef jarg1, int jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, HandleRef jarg4);

	// Token: 0x060000A8 RID: 168
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_GetConnectionState")]
	public static extern int IOnlineConnection_GetConnectionState(HandleRef jarg1);

	// Token: 0x060000A9 RID: 169
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_GetConfiguration")]
	public static extern IntPtr IOnlineConnection_GetConfiguration(HandleRef jarg1);

	// Token: 0x060000AA RID: 170
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_GetChat")]
	public static extern IntPtr IOnlineConnection_GetChat(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x060000AB RID: 171
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_RegisterQueryStatsListener")]
	public static extern void IOnlineConnection_RegisterQueryStatsListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000AC RID: 172
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_UnregisterQueryStatsListener")]
	public static extern void IOnlineConnection_UnregisterQueryStatsListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000AD RID: 173
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_GetBandwidthStatistics")]
	public static extern IntPtr IOnlineConnection_GetBandwidthStatistics(HandleRef jarg1);

	// Token: 0x060000AE RID: 174
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_RegisterDataProtect")]
	public static extern void IOnlineConnection_RegisterDataProtect(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000AF RID: 175
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_UnregisterDataProtect")]
	public static extern void IOnlineConnection_UnregisterDataProtect(HandleRef jarg1);

	// Token: 0x060000B0 RID: 176
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_SendProtectionData")]
	public static extern void IOnlineConnection_SendProtectionData(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3, uint jarg4);

	// Token: 0x060000B1 RID: 177
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineConnection_SetCertificateValidator")]
	public static extern void IOnlineConnection_SetCertificateValidator(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000B2 RID: 178
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineConnection")]
	public static extern void delete_IOnlineConnection(HandleRef jarg1);

	// Token: 0x060000B3 RID: 179
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnlineLog")]
	public static extern void delete_IOnlineLog(HandleRef jarg1);

	// Token: 0x060000B4 RID: 180
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineLog_OnLogMessage")]
	public static extern void IOnlineLog_OnLogMessage(HandleRef jarg1, int jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x060000B5 RID: 181
	[DllImport("CryOnline.so", EntryPoint = "CSharp_new_IOnlineLog")]
	public static extern IntPtr new_IOnlineLog();

	// Token: 0x060000B6 RID: 182
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnlineLog_director_connect")]
	public static extern void IOnlineLog_director_connect(HandleRef jarg1, IOnlineLog.SwigDelegateIOnlineLog_0 delegate0);

	// Token: 0x060000B7 RID: 183
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_Tick")]
	public static extern void IOnline_Tick(HandleRef jarg1);

	// Token: 0x060000B8 RID: 184
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_Shutdown")]
	public static extern void IOnline_Shutdown(HandleRef jarg1);

	// Token: 0x060000B9 RID: 185
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_CreateConnection")]
	public static extern IntPtr IOnline_CreateConnection(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x060000BA RID: 186
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_ReleaseConnection")]
	public static extern void IOnline_ReleaseConnection(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x060000BB RID: 187
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_RegisterLog")]
	public static extern void IOnline_RegisterLog(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000BC RID: 188
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_UnregisterLog")]
	public static extern void IOnline_UnregisterLog(HandleRef jarg1);

	// Token: 0x060000BD RID: 189
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_GetLog")]
	public static extern IntPtr IOnline_GetLog(HandleRef jarg1);

	// Token: 0x060000BE RID: 190
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_GetConfiguration")]
	public static extern IntPtr IOnline_GetConfiguration(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x060000BF RID: 191
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_GetConnection")]
	public static extern IntPtr IOnline_GetConnection(HandleRef jarg1, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg2);

	// Token: 0x060000C0 RID: 192
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_RegisterConnectionListener")]
	public static extern bool IOnline_RegisterConnectionListener(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x060000C1 RID: 193
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_UnregisterConnectionListener")]
	public static extern bool IOnline_UnregisterConnectionListener(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000C2 RID: 194
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_RegisterQueryBinder")]
	public static extern void IOnline_RegisterQueryBinder(HandleRef jarg1, HandleRef jarg2, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string jarg3);

	// Token: 0x060000C3 RID: 195
	[DllImport("CryOnline.so", EntryPoint = "CSharp_IOnline_UnregisterQueryBinder")]
	public static extern void IOnline_UnregisterQueryBinder(HandleRef jarg1, HandleRef jarg2);

	// Token: 0x060000C4 RID: 196
	[DllImport("CryOnline.so", EntryPoint = "CSharp_delete_IOnline")]
	public static extern void delete_IOnline(HandleRef jarg1);

	// Token: 0x060000C5 RID: 197
	[DllImport("CryOnline.so", EntryPoint = "CSharp_CryOnlineInit")]
	public static extern IntPtr CryOnlineInit();

	// Token: 0x060000C6 RID: 198
	[DllImport("CryOnline.so", EntryPoint = "CSharp_CryOnlineShutdown")]
	public static extern void CryOnlineShutdown();

	// Token: 0x060000C7 RID: 199
	[DllImport("CryOnline.so", EntryPoint = "CSharp_CryOnlineGetInstance")]
	public static extern IntPtr CryOnlineGetInstance();

	// Token: 0x060000C8 RID: 200
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SOnlineQueryUpcast")]
	public static extern IntPtr SOnlineQueryUpcast(IntPtr objectRef);

	// Token: 0x060000C9 RID: 201
	[DllImport("CryOnline.so", EntryPoint = "CSharp_SQueryErrorUpcast")]
	public static extern IntPtr SQueryErrorUpcast(IntPtr objectRef);

	// Token: 0x04000002 RID: 2
	protected static CryOnlinePINVOKE.SWIGExceptionHelper swigExceptionHelper = new CryOnlinePINVOKE.SWIGExceptionHelper();

	// Token: 0x04000003 RID: 3
	protected static CryOnlinePINVOKE.SWIGStringHelper swigStringHelper = new CryOnlinePINVOKE.SWIGStringHelper();

	// Token: 0x02000005 RID: 5
	protected class SWIGExceptionHelper
	{
		// Token: 0x060000CB RID: 203 RVA: 0x000021B4 File Offset: 0x000003B4
		static SWIGExceptionHelper()
		{
			CryOnlinePINVOKE.SWIGExceptionHelper.SWIGRegisterExceptionCallbacks_CryOnline(CryOnlinePINVOKE.SWIGExceptionHelper.applicationDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.arithmeticDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.divideByZeroDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.indexOutOfRangeDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.invalidCastDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.invalidOperationDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ioDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.nullReferenceDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.outOfMemoryDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.overflowDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.systemDelegate);
			CryOnlinePINVOKE.SWIGExceptionHelper.SWIGRegisterExceptionCallbacksArgument_CryOnline(CryOnlinePINVOKE.SWIGExceptionHelper.argumentDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.argumentNullDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.argumentOutOfRangeDelegate);
		}

		// Token: 0x060000CD RID: 205
		[DllImport("CryOnline.so")]
		public static extern void SWIGRegisterExceptionCallbacks_CryOnline(CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate applicationDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate arithmeticDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate divideByZeroDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate indexOutOfRangeDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate invalidCastDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate invalidOperationDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate ioDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate nullReferenceDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate outOfMemoryDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate overflowDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate systemExceptionDelegate);

		// Token: 0x060000CE RID: 206
		[DllImport("CryOnline.so", EntryPoint = "SWIGRegisterExceptionArgumentCallbacks_CryOnline")]
		public static extern void SWIGRegisterExceptionCallbacksArgument_CryOnline(CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate argumentDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate argumentNullDelegate, CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate argumentOutOfRangeDelegate);

		// Token: 0x060000CF RID: 207 RVA: 0x00002307 File Offset: 0x00000507
		private static void SetPendingApplicationException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new ApplicationException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00002319 File Offset: 0x00000519
		private static void SetPendingArithmeticException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new ArithmeticException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000232B File Offset: 0x0000052B
		private static void SetPendingDivideByZeroException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new DivideByZeroException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000233D File Offset: 0x0000053D
		private static void SetPendingIndexOutOfRangeException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new IndexOutOfRangeException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000234F File Offset: 0x0000054F
		private static void SetPendingInvalidCastException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new InvalidCastException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00002361 File Offset: 0x00000561
		private static void SetPendingInvalidOperationException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new InvalidOperationException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00002373 File Offset: 0x00000573
		private static void SetPendingIOException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new IOException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00002385 File Offset: 0x00000585
		private static void SetPendingNullReferenceException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new NullReferenceException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00002397 File Offset: 0x00000597
		private static void SetPendingOutOfMemoryException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new OutOfMemoryException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000023A9 File Offset: 0x000005A9
		private static void SetPendingOverflowException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new OverflowException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000023BB File Offset: 0x000005BB
		private static void SetPendingSystemException(string message)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new SystemException(message, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000023CD File Offset: 0x000005CD
		private static void SetPendingArgumentException(string message, string paramName)
		{
			CryOnlinePINVOKE.SWIGPendingException.Set(new ArgumentException(message, paramName, CryOnlinePINVOKE.SWIGPendingException.Retrieve()));
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000023E0 File Offset: 0x000005E0
		private static void SetPendingArgumentNullException(string message, string paramName)
		{
			Exception ex = CryOnlinePINVOKE.SWIGPendingException.Retrieve();
			if (ex != null)
			{
				message = message + " Inner Exception: " + ex.Message;
			}
			CryOnlinePINVOKE.SWIGPendingException.Set(new ArgumentNullException(paramName, message));
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00002418 File Offset: 0x00000618
		private static void SetPendingArgumentOutOfRangeException(string message, string paramName)
		{
			Exception ex = CryOnlinePINVOKE.SWIGPendingException.Retrieve();
			if (ex != null)
			{
				message = message + " Inner Exception: " + ex.Message;
			}
			CryOnlinePINVOKE.SWIGPendingException.Set(new ArgumentOutOfRangeException(paramName, message));
		}

		// Token: 0x04000004 RID: 4
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate applicationDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingApplicationException);

		// Token: 0x04000005 RID: 5
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate arithmeticDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingArithmeticException);

		// Token: 0x04000006 RID: 6
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate divideByZeroDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingDivideByZeroException);

		// Token: 0x04000007 RID: 7
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate indexOutOfRangeDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingIndexOutOfRangeException);

		// Token: 0x04000008 RID: 8
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate invalidCastDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingInvalidCastException);

		// Token: 0x04000009 RID: 9
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate invalidOperationDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingInvalidOperationException);

		// Token: 0x0400000A RID: 10
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate ioDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingIOException);

		// Token: 0x0400000B RID: 11
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate nullReferenceDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingNullReferenceException);

		// Token: 0x0400000C RID: 12
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate outOfMemoryDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingOutOfMemoryException);

		// Token: 0x0400000D RID: 13
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate overflowDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingOverflowException);

		// Token: 0x0400000E RID: 14
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate systemDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingSystemException);

		// Token: 0x0400000F RID: 15
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate argumentDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingArgumentException);

		// Token: 0x04000010 RID: 16
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate argumentNullDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingArgumentNullException);

		// Token: 0x04000011 RID: 17
		private static CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate argumentOutOfRangeDelegate = new CryOnlinePINVOKE.SWIGExceptionHelper.ExceptionArgumentDelegate(CryOnlinePINVOKE.SWIGExceptionHelper.SetPendingArgumentOutOfRangeException);

		// Token: 0x02000006 RID: 6
		// (Invoke) Token: 0x060000DE RID: 222
		public delegate void ExceptionDelegate(string message);

		// Token: 0x02000007 RID: 7
		// (Invoke) Token: 0x060000E2 RID: 226
		public delegate void ExceptionArgumentDelegate(string message, string paramName);
	}

	// Token: 0x02000008 RID: 8
	public class SWIGPendingException
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00002458 File Offset: 0x00000658
		public static bool Pending
		{
			get
			{
				bool result = false;
				if (CryOnlinePINVOKE.SWIGPendingException.numExceptionsPending > 0 && CryOnlinePINVOKE.SWIGPendingException.pendingException != null)
				{
					result = true;
				}
				return result;
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00002480 File Offset: 0x00000680
		public static void Set(Exception e)
		{
			if (CryOnlinePINVOKE.SWIGPendingException.pendingException != null)
			{
				throw new ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + CryOnlinePINVOKE.SWIGPendingException.pendingException.ToString() + ")", e);
			}
			CryOnlinePINVOKE.SWIGPendingException.pendingException = e;
			object typeFromHandle = typeof(CryOnlinePINVOKE);
			lock (typeFromHandle)
			{
				CryOnlinePINVOKE.SWIGPendingException.numExceptionsPending++;
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00002500 File Offset: 0x00000700
		public static Exception Retrieve()
		{
			Exception result = null;
			if (CryOnlinePINVOKE.SWIGPendingException.numExceptionsPending > 0 && CryOnlinePINVOKE.SWIGPendingException.pendingException != null)
			{
				result = CryOnlinePINVOKE.SWIGPendingException.pendingException;
				CryOnlinePINVOKE.SWIGPendingException.pendingException = null;
				object typeFromHandle = typeof(CryOnlinePINVOKE);
				lock (typeFromHandle)
				{
					CryOnlinePINVOKE.SWIGPendingException.numExceptionsPending--;
				}
			}
			return result;
		}

		// Token: 0x04000012 RID: 18
		[ThreadStatic]
		private static Exception pendingException;

		// Token: 0x04000013 RID: 19
		private static int numExceptionsPending;
	}

	// Token: 0x02000009 RID: 9
	protected class SWIGStringHelper
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00002576 File Offset: 0x00000776
		static SWIGStringHelper()
		{
			CryOnlinePINVOKE.SWIGStringHelper.SWIGRegisterStringCallback_CryOnline(CryOnlinePINVOKE.SWIGStringHelper.stringDelegate);
		}

		// Token: 0x060000EC RID: 236
		[DllImport("CryOnline.so")]
		public static extern void SWIGRegisterStringCallback_CryOnline(CryOnlinePINVOKE.SWIGStringHelper.SWIGStringDelegate stringDelegate);

		// Token: 0x060000ED RID: 237 RVA: 0x0000259B File Offset: 0x0000079B
		private static string CreateString(string cString)
		{
			return cString;
		}

		// Token: 0x04000014 RID: 20
		private static CryOnlinePINVOKE.SWIGStringHelper.SWIGStringDelegate stringDelegate = new CryOnlinePINVOKE.SWIGStringHelper.SWIGStringDelegate(CryOnlinePINVOKE.SWIGStringHelper.CreateString);

		// Token: 0x0200000A RID: 10
		// (Invoke) Token: 0x060000EF RID: 239
		public delegate string SWIGStringDelegate(string message);
	}
}
