﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mega;

namespace ScheduledCameraUploadTaskAgent
{
    public class MegaGlobalListener : MGlobalListenerInterface
    {
        #region MGlobalListenerInterface

        public void onAccountUpdate(MegaSDK api)
        {
            // Do nothing
        }

        public void onContactRequestsUpdate(MegaSDK api, MContactRequestList requests)
        {
            // Do nothing
        }

        public void onNodesUpdate(MegaSDK api, MNodeList nodes)
        {
            // If the SDK has resumed the possible pending transfers
            if(nodes == null)
            {
                // If no pending transfers to resume start a new upload
                // Else it will start when finish the current transfer
                if(api.getTransfers(MTransferType.TYPE_UPLOAD).size() == 0)
                {
                    ScheduledAgent.Upload();
                }
            }
        }

        public void onReloadNeeded(MegaSDK api)
        {
            // Do nothing
        }

        public void onUsersUpdate(MegaSDK api, MUserList users)
        {
            // Do nothing
        }

        #endregion
    }
}
