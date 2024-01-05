using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Consts
{
    public static class VLConstants
    {
        public const string TaskNotificationQueue = "VL_TaskNotificationQueue";
        // Other constants
        public const int WebpageLoaderLocalPort = 5010;
        public const int TaskQueueManagerLocalPort = 5020;
        public const string ApiKeyHeaderName = "X-API-KEY";
        public const string ServiceInternalSecretKey = "some secret string";

        public const string ENV_VAR_NAME_VLUserPanelApiUrl = "USER_PANEL_API_URI";
        public const string ENV_VAR_NAME_RabbitMQHost = "RABBIT_MQ_HOST";
        public const string ENV_VAR_NAME_DbConnection = "DB_CONNECTION";
    }
}
