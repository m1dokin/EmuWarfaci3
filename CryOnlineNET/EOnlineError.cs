using System;

// Token: 0x0200000E RID: 14
public enum EOnlineError
{
	// Token: 0x04000024 RID: 36
	eOnlineError_NoError,
	// Token: 0x04000025 RID: 37
	eOnlineError_StreamError,
	// Token: 0x04000026 RID: 38
	eOnlineError_StreamVersionError,
	// Token: 0x04000027 RID: 39
	eOnlineError_StreamClosed,
	// Token: 0x04000028 RID: 40
	eOnlineError_ProxyAuthRequired,
	// Token: 0x04000029 RID: 41
	eOnlineError_ProxyAuthFailed,
	// Token: 0x0400002A RID: 42
	eOnlineError_ProxyNoSupportedAuth,
	// Token: 0x0400002B RID: 43
	eOnlineError_IoError,
	// Token: 0x0400002C RID: 44
	eOnlineError_ParseError,
	// Token: 0x0400002D RID: 45
	eOnlineError_ConnectionRefused,
	// Token: 0x0400002E RID: 46
	eOnlineError_DnsError,
	// Token: 0x0400002F RID: 47
	eOnlineError_OutOfMemory,
	// Token: 0x04000030 RID: 48
	eOnlineError_NoSupportedAuth,
	// Token: 0x04000031 RID: 49
	eOnlineError_TlsFailed,
	// Token: 0x04000032 RID: 50
	eOnlineError_TlsNotAvailable,
	// Token: 0x04000033 RID: 51
	eOnlineError_CompressionFailed,
	// Token: 0x04000034 RID: 52
	eOnlineError_AuthenticationFailed,
	// Token: 0x04000035 RID: 53
	eOnlineError_UserDisconnected,
	// Token: 0x04000036 RID: 54
	eOnlineError_NotConnected,
	// Token: 0x04000037 RID: 55
	eOnlineError_UnknownError,
	// Token: 0x04000038 RID: 56
	eOnlineError_ServiceNotFound = 404,
	// Token: 0x04000039 RID: 57
	eOnlineError_ServiceUnavailable = 503,
	// Token: 0x0400003A RID: 58
	eOnlineError_QoSLimitReached = 1006,
	// Token: 0x0400003B RID: 59
	eOnlineError_MaxOnlineUsersReached,
	// Token: 0x0400003C RID: 60
	eOnlineError_InvalidSession,
	// Token: 0x0400003D RID: 61
	eOnlineError_LastingServerFailure,
	// Token: 0x0400003E RID: 62
	eOnlineError_LostConnection,
	// Token: 0x0400003F RID: 63
	eOnlineError_QueryTimeout,
	// Token: 0x04000040 RID: 64
	eOnlineError_TryAgain,
	// Token: 0x04000041 RID: 65
	eOnlineError_ServerNotFound,
	// Token: 0x04000042 RID: 66
	eOnlineError_UnknownServer
}
