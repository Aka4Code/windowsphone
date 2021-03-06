﻿using System;
using System.Windows;
using System.Windows.Media;
using mega;
using MegaApp.Classes;
using MegaApp.Enums;
using MegaApp.Models;
using MegaApp.Resources;
using MegaApp.Services;

namespace MegaApp.MegaApi
{
    class GetPricingRequestListener : BaseRequestListener
    {
        private readonly AccountDetailsViewModel _accountDetails;
        private readonly UpgradeAccountViewModel _upgradeAccount;

        public GetPricingRequestListener(AccountDetailsViewModel accountDetails, UpgradeAccountViewModel upgradeAccount)
        {
            _accountDetails = accountDetails;
            _upgradeAccount = upgradeAccount;
        }

        protected override string ProgressMessage
        {
            get { return ProgressMessages.GetAccountDetails; }
        }

        protected override bool ShowProgressMessage
        {
            get { return true; }
        }

        protected override string ErrorMessage
        {
            get { return AppMessages.GetAccountDetailsFailed; }
        }

        protected override string ErrorMessageTitle
        {
            get { return AppMessages.GetAccountDetailsFailed_Title; }
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

        protected override void OnSuccesAction(MegaSDK api, MRequest request)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _upgradeAccount.Products.Clear();
                _upgradeAccount.Plans.Clear();

                if (App.AppInformation.IsNewlyActivatedAccount)
                {
                    var freePlan = new ProductBase
                    {
                        AccountType = MAccountType.ACCOUNT_TYPE_FREE,
                        Name = AppResources.AccountTypeFree,
                        ProductColor = Color.FromArgb(255, 19, 224, 60),
                        ProductPathData = VisualResources.CrestFreeAccountPathData
                    };

                    _upgradeAccount.Plans.Add(freePlan);
                    App.AppInformation.IsNewlyActivatedAccount = false;
                }

                int numberOfProducts = request.getPricing().getNumProducts();

                for (int i = 0; i < numberOfProducts; i++)
                {
                    var accountType = (MAccountType) Enum.Parse(typeof (MAccountType),
                        request.getPricing().getProLevel(i).ToString());
                    
                    var product = new Product
                    {
                        AccountType = accountType,
                        Amount = request.getPricing().getAmount(i),
                        Currency = request.getPricing().getCurrency(i),
                        GbStorage = request.getPricing().getGBStorage(i),
                        GbTransfer = request.getPricing().getGBTransfer(i),
                        Months = request.getPricing().getMonths(i),
                        Handle = request.getPricing().getHandle(i)
                    };

                    switch (accountType)
                    {
                        case MAccountType.ACCOUNT_TYPE_FREE:
                            product.Name = AppResources.AccountTypeFree;
                            product.ProductPathData = VisualResources.CrestFreeAccountPathData;
                            break;
                        case MAccountType.ACCOUNT_TYPE_LITE:
                            product.Name = AppResources.AccountTypeLite;
                            product.ProductColor = Color.FromArgb(255, 255, 165, 0);
                            product.ProductPathData = VisualResources.CrestLiteAccountPathData;
                            product.IsNewOffer = true;

                            // If Centili payment method is active, and product is LITE monthly include it into the product
                            if (_upgradeAccount.CentiliPaymentMethodAvailable && product.Months == 1)
                            {
                                var centiliPaymentMethod = new PaymentMethod
                                {
                                    PaymentMethodType = MPaymentMethod.PAYMENT_METHOD_CENTILI,
                                    Name = String.Format("Centili - " + UiResources.PhoneBill + " (" + UiResources.Punctual.ToLower() + ")"),
                                    PaymentMethodPathData = VisualResources.PhoneBillingPathData
                                };
                                product.PaymentMethods.Add(centiliPaymentMethod);
                            }

                            // If Fortumo payment method is active, and product is LITE monthly include it into the product
                            if(_upgradeAccount.FortumoPaymentMethodAvailable && product.Months == 1)
                            {
                                var fortumoPaymentMethod = new PaymentMethod
                                {
                                    PaymentMethodType = MPaymentMethod.PAYMENT_METHOD_FORTUMO,
                                    Name = String.Format("Fortumo - " + UiResources.PhoneBill + " (" + UiResources.Punctual.ToLower() + ")"),
                                    PaymentMethodPathData = VisualResources.PhoneBillingPathData
                                };
                                product.PaymentMethods.Add(fortumoPaymentMethod);
                            }
                            break;
                        case MAccountType.ACCOUNT_TYPE_PROI:
                            product.Name = AppResources.AccountTypePro1;
                            product.ProductColor = Color.FromArgb(255, 217, 0, 7);
                            product.ProductPathData = VisualResources.CrestProIAccountPathData;                            
                            break;
                        case MAccountType.ACCOUNT_TYPE_PROII:
                            product.Name = AppResources.AccountTypePro2;
                            product.ProductColor = Color.FromArgb(255, 217, 0, 7);
                            product.ProductPathData = VisualResources.CrestProIIAccountPathData;                            
                            break;
                        case MAccountType.ACCOUNT_TYPE_PROIII:
                            product.Name = AppResources.AccountTypePro3;
                            product.ProductColor = Color.FromArgb(255, 217, 0, 7);
                            product.ProductPathData = VisualResources.CrestProIIIAccountPathData;                            
                            break;
                        default:
                            product.ProductColor = Color.FromArgb(255, 217, 0, 7);
                            break;
                    }

                    // If CC payment method is active, include it into the product
                    if(_upgradeAccount.CreditCardPaymentMethodAvailable)
                    {
                        var creditCardPaymentMethod = new PaymentMethod
                        {
                            PaymentMethodType = MPaymentMethod.PAYMENT_METHOD_CREDIT_CARD,
                            Name = String.Format(UiResources.CreditCard + " (" + UiResources.Recurring.ToLower() + ")"),
                            PaymentMethodPathData = VisualResources.CreditCardPathData
                        };
                        product.PaymentMethods.Add(creditCardPaymentMethod);
                    }

                    // If in-app payment method is active, include it into the product
                    if (_upgradeAccount.InAppPaymentMethodAvailable)
                    {
                        var inAppPaymentMethod = new PaymentMethod
                        {
                            PaymentMethodType = MPaymentMethod.PAYMENT_METHOD_WINDOWS_STORE,
                            Name = String.Format(UiResources.UI_InAppPurchase + " - " + UiResources.PhoneBill + " (" + UiResources.Punctual.ToLower() + ")"),
                            PaymentMethodPathData = VisualResources.PhoneBillingPathData
                        };
                        product.PaymentMethods.Add(inAppPaymentMethod);
                    }      

                    _upgradeAccount.Products.Add(product);

#if DEBUG
                    // Fill the Mocking IAP product listing with actual NEGA product id's
                    LicenseService.AddProductToMockIap(product);
#endif

                    // Plans show only the information off the annualy plans
                    if (request.getPricing().getMonths(i) == 12)
                    {
                        var plan = new ProductBase
                        {
                            AccountType = accountType,
                            Name = product.Name,
                            Amount = product.Amount,
                            Currency = product.Currency,
                            GbStorage = product.GbStorage,
                            GbTransfer = product.GbTransfer/12,
                            ProductPathData = product.ProductPathData,
                            ProductColor = product.ProductColor,
                            IsNewOffer = product.IsNewOffer
                        };

                        _upgradeAccount.Plans.Add(plan);
                       
                        // Check if the user has a product/plan already purchased and fill the structure to show it
                        if (accountType == _accountDetails.AccountType && request.getPricing().getMonths(i) == 12)
                        {
                            _upgradeAccount.ProductPurchased = product;
                            _upgradeAccount.ProductPurchased.GbTransfer = request.getPricing().getGBTransfer(i)/12;
                            _upgradeAccount.ProductPurchased.IsNewOffer = false;
                            _upgradeAccount.ProductPurchased.Purchased = true;
                        }
                    }
                }
            });
        }

        #endregion
    }
}
