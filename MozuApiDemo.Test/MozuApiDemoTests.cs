using System;
using Autofac;
using Mozu.Api;
using Mozu.Api.ToolKit.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Mozu.Api.Contracts.MZDB;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.Api.Resources.Platform.Entitylists;
using Mozu.Api.ToolKit.Handlers;
using Newtonsoft.Json.Linq;
using Mozu.Api.Resources.Commerce.Settings;

namespace MozuApiDemoTests.Test
{
    [TestClass]
    public class MozuApiDemoTests
    {
        private IApiContext _apiContext;
        private IAppSetting _appSetting;
        private IContainer _container;


        [TestInitialize]
        public void Initialize_Tests()
        {
            _container = new Bootstrapper().Bootstrap().Container;
            _appSetting = _container.Resolve<IAppSetting>();
            var tenantId = int.Parse(_appSetting.Settings["TenantId"].ToString());
            var siteId = int.Parse(_appSetting.Settings["SiteId"].ToString());

            var headers = new NameValueCollection
            {
                {"x-vol-tenant", tenantId},
                {"x-vol-site", siteId},
                {"x-vol-qwdqwdqwd", "qwdqwd" }
            };

            _apiContext = new ApiContext(headers);
            //_apiContext = new ApiContext(tenantId, siteId);
        }

        [TestMethod]
        public void Initialize_App()
        {
            var applicationResource = new ApplicationResource(_apiContext);
            var application = applicationResource.ThirdPartyGetApplicationAsync().Result;
            application.Initialized = true;
            applicationResource.ThirdPartyUpdateApplicationAsync(application).Wait();
        }

        [TestMethod]
        public void Exercise_8_Get_Tenant()
        {
            //create a new tenant resource
            var tenantResource = new Mozu.Api.Resources.Platform.TenantResource(_apiContext);

            //get an instance of the tenant
            var tenant = tenantResource.GetTenantAsync(_apiContext.TenantId).Result;

            Console.WriteLine($"Domain: {tenant.Domain}");
            Console.WriteLine($"Tenant Id: {tenant.Id}");
            Console.WriteLine("Tenant Name: " + tenant.Name);
             
            //loop through all master catalogs
            foreach (var masterCatalog in tenant.MasterCatalogs)
            {
                Console.WriteLine($"Master Catalog[{masterCatalog.Id}]: {masterCatalog.Name}");
                masterCatalog.Catalogs.ForEach(c => Console.WriteLine($"  Catalog[{c.Id}]: {c.Name}"));
            }

            //loop through sites
            foreach (var site in tenant.Sites)
            {
                Console.WriteLine($"Site Name: {site.Name}");
                Console.WriteLine("Site Id: " + site.Id);
            }
        }

        [TestMethod]
        public void AddEvent()
        {
            var eventItem = new EventItem
            {
                EntityId = "567",
                EventId = "def",
                Id = "PK123",
                Topic = "order.opened",
                QueuedDateTime = DateTime.Now,
                Status = EventStatus.Pending.ToString()
            };


            var listFullName = $"EventQueue@{_appSetting.Namespace}";

            try
            {
                var entityResource = new EntityResource(_apiContext);

                var entity = entityResource.GetEntityAsync(listFullName, eventItem.Id).Result;
                if (entity == null)
                {

                    var result = entityResource.InsertEntityAsync(JObject.FromObject(eventItem), listFullName).Result;
                }
            }

            catch (ApiException ex)
            {
                if (ex.ErrorCode.Trim() == "ITEM_ALREADY_EXISTS")
                {
                    throw new Exception("Item exists");
                }
                throw;
            }

        }
    }
}

