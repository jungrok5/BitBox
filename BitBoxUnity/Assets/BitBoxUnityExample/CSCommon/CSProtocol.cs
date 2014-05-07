using System.Collections;

namespace BitBoxExample.CSCommon
{
    public enum ProtocolBase : ushort
    {
        PROTOCOL_VERSION = 0x0000,

        PROTOCOL_BASE_CS = (PROTOCOL_VERSION + 0x1000),
    };

    public enum ProtocolID : ushort
    {
        PROTOCOL_CS_AU_START = ProtocolBase.PROTOCOL_BASE_CS,
        
        CS_ECHO_WEB_REQ,
        CS_ECHO_WEB_ACK,

        CS_ECHO_APP_REQ,
        CS_ECHO_APP_ACK,

        PROTOCOL_CS_AU_END,
    };
}