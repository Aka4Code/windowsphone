﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using mega;
using MegaApp.Classes;
using MegaApp.Enums;
using MegaApp.Resources;
using MegaApp.Services;

namespace MegaApp.MegaApi
{
    class GetUserDataRequestListener : BaseRequestListener
    {
        private readonly UserDataViewModel _userData;

        public GetUserDataRequestListener(UserDataViewModel userData)
        {
            _userData = userData;
        }

        protected override string ProgressMessage
        {
            get { return ProgressMessages.GetUserData; }
        }

        protected override bool ShowProgressMessage
        {
            get { return true; }
        }

        protected override string ErrorMessage
        {
            get { return AppMessages.GetUserDataFailed; }
        }

        protected override string ErrorMessageTitle
        {
            get { return AppMessages.GetUserDataFailed_Title; }
        }

        protected override bool ShowErrorMessage
        {
            get { return true; }
        }

        protected override string SuccessMessage
        {
            get { throw new NotImplementedException(); }
        }

        protected override string SuccessMessageTitle
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool ShowSuccesMessage
        {
            get { return false; }
        }

        protected override bool NavigateOnSucces
        {
            get { return false; }
        }

        protected override bool ActionOnSucces
        {
            get { return true; }
        }

        protected override Type NavigateToPage
        {
            get { throw new NotImplementedException(); }
        }

        protected override NavigationParameter NavigationParameter
        {
            get { throw new NotImplementedException(); }
        }

        #region Override Methods

        public override void onRequestFinish(MegaSDK api, MRequest request, MError e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ProgressService.ChangeProgressBarBackgroundColor((Color)Application.Current.Resources["PhoneChromeColor"]);
                ProgressService.SetProgressIndicator(false);
            });

            if (e.getErrorCode() == MErrorType.API_OK)
            {
                if (request.getType() == MRequestType.TYPE_GET_ATTR_USER)
                {
                    switch (request.getParamType())
                    {
                        case (int)MUserAttrType.USER_ATTR_FIRSTNAME:
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                _userData.Firstname = request.getText();
                                if (App.UserData != null)
                                    App.UserData.Firstname = _userData.Firstname;
                            });
                            break;

                        case (int)MUserAttrType.USER_ATTR_LASTNAME:
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                _userData.Lastname = request.getText();
                                if (App.UserData != null)
                                    App.UserData.Lastname = _userData.Lastname;
                            });
                            break;
                    }
                }                
            }
            else
            {
                if (request.getType() == MRequestType.TYPE_GET_ATTR_USER)
                {
                    if (request.getParamType() == (int)MUserAttrType.USER_ATTR_FIRSTNAME)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            _userData.Firstname = UiResources.MyAccount;
                            if (App.UserData != null)
                                App.UserData.Firstname = _userData.Firstname;
                        });                            
                    }
                }
            }
        }

        #endregion
    }
}
