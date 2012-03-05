//using System;
//using System.Net;
//using System.Web.Mvc;
//using Umbraco.Cms.Web;
//using Umbraco.Framework;
//using Umbraco.Framework.Context;
//using Umbraco.Framework.Diagnostics;
//using Umbraco.Framework.Persistence;
//using Umbraco.Framework.Tasks;
//using Umbraco.Hive;
//using Umbraco.Hive.Tasks;

//namespace Umbraco.Pugpig.Core.Installers
//{
    
//    [Task("FA00109A-242F-439C-94E9-A248663BC4E2", TaskTriggers.PostPackageInstall, ContinueOnFailure = false)]
//    public class PugpigInstallTask : HiveProviderInstallTask
//    {                                            
//        public PugpigInstallTask(Cms.Web.Context.ConfigurationTaskContext configurationTaskContext) : base(configurationTaskContext.ApplicationContext.FrameworkContext, configurationTaskContext.ApplicationContext.Hive)
//        {


//        }

//        public override void Execute(TaskExecutionContext context)
//        {
            
//            //HttpWebRequest wreq;
//            //HttpWebResponse wresp = null;

//            //try
//            //{
//            //    LogHelper.Error<PugpigInstallTask>(string.Format("Running the pugpig install task"), new Exception());
//            //    var controller = context.EventSource as Controller;
//            //    var urlHelper = new UrlHelper(controller.ControllerContext.RequestContext);
//            //  //  var url = urlHelper.;
//            //    LogHelper.Error<PugpigInstallTask>(
//            //           string.Format(String.Format("The url was {0}", url)), new Exception());
//            //    //http://umbracodebug.local/umbraco/pugpig/InitContentSurface/InstallPugpigData
//            //    wreq = (HttpWebRequest)WebRequest.Create(url);
//            //    wreq.KeepAlive = true;
//            //    wreq.Method = "GET";
//            //    wresp = (HttpWebResponse)wreq.GetResponse();
//            //}
//            //catch (WebException)
//            //{
//            //    LogHelper.Error<PugpigInstallTask>(
//            //           string.Format("Error calling the url to create content"), new Exception());
//            //}
//            //finally
//            //{
//            //    if (wresp != null)
//            //        wresp.Close();
//            //}
//        }


//        public override bool NeedsInstallOrUpgrade
//        {
//            get { return true; }
//        }

//        public override int GetInstalledVersion()
//        {
//            return 0;
//        }

//        public override void InstallOrUpgrade()
//        {
//        }
//    }
//}