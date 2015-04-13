﻿using System;

namespace MegaApp.Models
{
    class LoginAndCreateAccountViewModelContainer
    {
        public LoginViewModel _loginViewModel { get; set; }
        public CreateAccountViewModel _createAccountViewModel { get; set; }

        public LoginAndCreateAccountViewModelContainer()
        {
            _loginViewModel = new LoginViewModel(App.MegaSdk);
            _createAccountViewModel = new CreateAccountViewModel(App.MegaSdk);
        }
    }
}