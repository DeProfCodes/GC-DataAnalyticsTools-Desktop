using Autofac;
using Autofac.Features.ResolveAnything;
using Data_Analytics_Tools.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Data_Analytics_Tools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var builder = new ContainerBuilder();
            //allow the Autofac container resolve unknown types
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            //register the MyDataService class as the IDataService interface in the DI container
            builder.RegisterType<BusinessLogicData>().As<IBusinessLogicData>().SingleInstance();
            Autofac.IContainer container = builder.Build();
        }
    }
}
