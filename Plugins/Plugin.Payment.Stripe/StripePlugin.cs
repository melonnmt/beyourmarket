﻿using BeYourMarket.Core;
using BeYourMarket.Core.Plugins;
using BeYourMarket.Model.Enum;
using BeYourMarket.Service;
using Plugin.Payment.Stripe.Migrations;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using BeYourMarket.Core.Extensions;

namespace Plugin.Payment.Stripe
{
    public class StripePlugin : WidgetBasePlugin
    {
        public const string SettingStripeApiKey = "StripeApiKey";
        public const string SettingStripePublishableKey = "StripePublishableKey";
        public const string SettingStripeClientID = "StripeClientID";

        private readonly ISettingDictionaryService _settingDictionaryService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;        

        public enum Enum_StripeConnectStatus
        {
            None = 0,
            Authorized
        }

        public StripePlugin(
            ISettingDictionaryService settingDictionaryService,
            IUnitOfWorkAsync unitOfWorkAsync)
        {
            _settingDictionaryService = settingDictionaryService;
            _unitOfWorkAsync = unitOfWorkAsync;

            AddRoute(WidgetZone.Payment, new RouteValueDictionary
                {
                    { "action", "Payment" }, 
                    { "controller", "PaymentStripe" }, 
                    { "namespaces", "Plugin.Payment.Stripe.Controllers"},
                    { "area", null},
                    { "widgetZone", WidgetZone.Payment}
                });

            AddRoute(WidgetZone.PaymentSetting, new RouteValueDictionary
                {
                    { "action", "PaymentSetting" }, 
                    { "controller", "PaymentStripe" }, 
                    { "namespaces", "Plugin.Payment.Stripe.Controllers"},
                    { "area", null},
                    { "widgetZone", WidgetZone.PaymentSetting}
                });

            AddRoute(WidgetZone.Transaction, new RouteValueDictionary
                {
                    { "action", "Transaction" }, 
                    { "controller", "PaymentStripe" }, 
                    { "namespaces", "Plugin.Payment.Stripe.Controllers"},
                    { "area", null},
                    { "widgetZone", WidgetZone.Transaction}
                });

            AddRoute(WidgetZone.TransactionOverview, new RouteValueDictionary
                {
                    { "action", "TransactionOverview" }, 
                    { "controller", "PaymentStripe" }, 
                    { "namespaces", "Plugin.Payment.Stripe.Controllers"},
                    { "area", null},
                    { "widgetZone", WidgetZone.TransactionOverview}
                });

            AddRoute(WidgetZone.Configuration, new RouteValueDictionary {                 
                { "action", "Configure" }, 
                { "controller", "PaymentStripe" }, 
                { "namespaces", "Plugin.Payment.Stripe.Controllers" }, 
                { "area", null } 
            });
        }

        public override Type GetControllerType()
        {
            return typeof(Plugin.Payment.Stripe.Controllers.PaymentStripeController);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            // Initialize database
            System.Data.Entity.Database.SetInitializer(new StripeDatabaseInitializer());

            // initialize and create database
            using (var context = new Plugin.Payment.Stripe.Data.StripeContext())
            {
                context.Database.Initialize(true);
                context.SaveChanges();
            }

            // Add settings
            _settingDictionaryService.Insert(new BeYourMarket.Model.Models.SettingDictionary()
            {
                Name = SettingStripeApiKey,
                Value = "sk_test_kUNQFEh3YLbEFEa38tbeMJLV",
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added,
                SettingID = CacheHelper.Settings.ID
            });

            _settingDictionaryService.Insert(new BeYourMarket.Model.Models.SettingDictionary()
            {
                Name = SettingStripePublishableKey,
                Value = "pk_test_EfbP8SfcALEJ8Jk2JxtSxmqe",
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added,
                SettingID = CacheHelper.Settings.ID
            });

            _settingDictionaryService.Insert(new BeYourMarket.Model.Models.SettingDictionary()
            {
                Name = SettingStripeClientID,
                Value = "ca_6Rh18px61rjCEZIav5ItunZ1mKD8YjvU",
                Created = DateTime.Now,
                LastUpdated = DateTime.Now,
                ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added,
                SettingID = CacheHelper.Settings.ID
            });

            _unitOfWorkAsync.SaveChanges();

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            // Remove settings
            var settings = _settingDictionaryService.Query(
                x => x.Name == SettingStripeApiKey ||
                     x.Name == SettingStripePublishableKey ||
                     x.Name == SettingStripeClientID).Select();

            foreach (var setting in settings)
            {
                _settingDictionaryService.Delete(setting);
            }

            _unitOfWorkAsync.SaveChanges();

            var context = new Plugin.Payment.Stripe.Data.StripeContext();
            context.DeletePluginData<Plugin.Payment.Stripe.Data.StripeContext>();

            base.Uninstall();
        }
    }
}