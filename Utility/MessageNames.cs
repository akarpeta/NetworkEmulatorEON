using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class MessageNames
    {
        //Wysyłane gdy chcemy zestawić połączenie
        public const string CALL_REQUEST = "CALLREQUEST";

        public const string ADD_CONNECTION = "ADDCONNECTION";

        public const string HANDLEAVAIBLESLOTREQUEST = "HANDLEAVAIBLESLOTREQUEST";

        public const string PEER_COORDINATION_RESPONSE = "PEERCOORDINATIONRESPONSE";

        //Wysyłane do CPCC, czyli pojawia się to w nasłuchiwaniu
        public const string CALL_INDICATION = "CALLINDICATION";

        //OTRZYMYWANE PRZEZ CPCC
        public const string CALL_CONFIRMED = "CALLCONFIRMED";

        public const string CALL_CONFIRMED_SEND = "CALLCONFIRMEDSEND";
        //OTRZYMYWANE PRZEZ NCC OD CPCC
        public const string CALL_CONFIRMED_FROM_CPCC = "CALLCONFIRMEDFROMCPCC";
        //OTRZYMYWANE PREZ NCC OD NCC_OTHER
        public const string CALL_CONFIRMED_FROM_NCC = "CALLCONFIRMEDFROMNCC";

        public const string ALLOCATE = "ALLOCATE";
        //
        public const string CALL_COORDINATION = "CALLCOORDINATION";

        //
        public const string CONNECTION_REQUEST = "CONNECTIONREQUEST";

        //
        public const string CONNECTION_CONFIRMED = "CONNECTIONCONFIRMED";

        //
        public const string LINK_CONNECTION_DEALLOCATION = "LINKCONNECTIONDEALLOCATION";
       
        public const string LINK_ALLOCATION_RESPONSE = "LINKALOCATIONRESPONSE";

        //
        public const string LINK_CONNECTION_REQUEST = "LINKCONNECTIONREQUEST";

        //
        public const string ROUTE_TABLE_QUERY = "ROUTETABLEQUERY";

        public const string ROUTE_TABLE_QUERY_RSP = "ROUTETABLEQUERYRSP";

        public const string HANDLEAVAIBLESLOT_RESPONSE = "HANDLEAVAIBLESLOT_RESPONSE";
        //
        public const string PEER_COORDINATION = "PEERCOORDINATION";

        public const string LOCAL_TOPOLOGY = "LOCALTOPOLOGY";

        //RC-RC
        public const string NETWORK_TOPOLOGY = "NETWORKTOPOLOGY";

        public const string NETWORK_TOPOLOGY_RESPONSE = "NETWORKTOPOLOGYRESPONSE";

        //RC-RC
        public const string ROUTE_PATH = "ROUTEPATH";

        //RC-RC w odpowiedzi na ROUTE_PATH
        public const string ROUTED_PATH = "ROUTEDPATH";

        public const string FAILED_ROUTE_TABLE_QUERY = "FAILEDROUTETABLEQUERY";

        public const string CALL_TEARDOWN = "CALLTEARDOWN";

        public const string CALL_TEARDOWN_CONFIRMATION = "CALLTEARDOWNCONFIRMED";

        public const string CONNECTION_TEARDOWN = "CONNECTIONTEARDOWN";

        public const string CONNECTION_TEARDOWN_CONFIRMED = "CONNECTIONTEARDOWNCONFIRMED";

        public const string GET_PATH = "GETPATH";

        public const string SNP_RELEASE = "SNPRELEASE";

        public const string DELETE_RECORD = "DELETERECORD";

        public const string ADD_RECORD = "ADDRECORD";

        public const string CONNECTION_TEARDOWN_PEER = "CONNECTION_TEARDOWN_PEER";
        //Uszkodzone łącze
        public const string KILL_LINK = "KILL_LINK";
    }
}

