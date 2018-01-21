using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using HotsBpHelper.Api;
using HotsBpHelper.Api.Security;
using HotsBpHelper.Factories;
using Stylet;
using StyletIoC;
using HotsBpHelper.Pages;
using HotsBpHelper.Services;
using HotsBpHelper.Settings;
using HotsBpHelper.Utils;
using HotsBpHelper.Utils.ComboBoxItemUtil;
using WPFLocalizeExtension.Engine;

namespace HotsBpHelper
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<IRestApi>().To<RestApi>().InSingletonScope();
            builder.Bind<HeroItemUtil>().ToSelf().InSingletonScope();
            builder.Bind<MapItemUtil>().ToSelf().InSingletonScope();
            builder.Bind<ISecurityProvider>().To<SecurityProvider>().InSingletonScope();
            builder.Bind<IToastService>().To<ToastService>().InSingletonScope();

            RegisterViewModelFactories(builder);

            builder.Bind<IImageUtil>().To<ImageUtils>();
        }

        private static void RegisterViewModelFactories(IStyletIoCBuilder builder)
        {
            var vmFactoriesList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where typeof (IViewModelFactory).IsAssignableFrom(assemblyType)
                select assemblyType).ToArray();

            var method = typeof (IStyletIoCBuilder).GetMethod("Bind", new Type[] {});
            foreach (var vmFactory in vmFactoriesList)
            {
                var generic = method?.MakeGenericMethod(vmFactory);
                var bindTo = generic?.Invoke(builder, null) as IBindTo;
                bindTo?.ToAbstractFactory();
            }

            builder.Bind<ViewModelFactory>().ToSelf().InSingletonScope();
        }

        protected override void Configure()
        {
            
            App.AppPath = AppDomain.CurrentDomain.BaseDirectory;

            var args = Environment.GetCommandLineArgs();
			
            App.Debug = true;
            // TODO Remove
			
            if (args.Any(arg => arg.ToLower() == "/notcheckprocess"))
            {
                App.NotCheckProcess = true;
            }
            if (args.Any(arg => arg.ToLower() == "/errortest"))
            {
                ErrorView _errorView = new ErrorView(ViewModelBase.L("NoMatchResolution"), ViewModelBase.L("MSG_NoMatchResolution"), "https://www.bphots.com/articles/errors/test");
                _errorView.ShowDialog();
            }

            if (args.Any(arg => arg.ToLower() == "/cn"))
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("zh-CN");                
            }
            else if (args.Any(arg => arg.ToLower() == "/us"))
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en-US");
            }
            else if (args.Any(arg => arg.ToLower() == "/kr"))
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("ko-KR");
            }
            else if (args.Any(arg => arg.ToLower() == "/tw"))
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("zh-TW");
            }
            else if (args.Any(arg => arg.ToLower() == "/jp"))
            {
                LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("ja-JP");
            }
            else LocalizeDictionary.Instance.Culture = System.Globalization.CultureInfo.InstalledUICulture;
            switch (LocalizeDictionary.Instance.Culture.Name) {
                case "zh-CN":
                    App.Language = "zh-CN";
                    break;
                case "ko-KR":
                    App.Language = "ko-KR";
                    break;
                case "zh-TW":
                    App.Language = "zh-TW";
                    break;
                case "zh-CHS":
                    App.Language = "zh-CN";
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("zh-CN");
                    break;
                case "zh-HK":
                case "zh-CHT":
                    App.Language = "zh-TW";
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("zh-TW");
                    break;
                default:
                    App.Language = "en-US";
                    LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en-US"); 
                    break;
            }
			
            App.Language = "zh-CN";
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("zh-CN"); //TODO REMOVE


        }
    }
}
