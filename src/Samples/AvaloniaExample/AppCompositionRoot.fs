namespace AvaloniaExample

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open Elmish.Avalonia
open Microsoft.Extensions.DependencyInjection

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainView = Views.MainView()

    override this.RegisterServices() = 
        ServiceCollection()
            .AddSingleton<FileService>(FileService(mainView))
            // TODO: Auto-register by default in CompositionRoot
            // Transient VMs (singleton lifetime will be handled via RegisterViews)
            .AddTransient<ViewModels.MainViewModel>()
            .AddTransient<ViewModels.CounterViewModel>()
            .AddTransient<ViewModels.AboutViewModel>()
            .AddTransient<ViewModels.ChartViewModel>()
            .AddTransient<ViewModels.FilePickerViewModel>()


    override this.RegisterViews() = 
        Map [
            VMKey.Create<ViewModels.MainViewModel>(), ViewRegistration.SingletonView(mainView)
            VMKey.Create<ViewModels.CounterViewModel>(), ViewRegistration.SingletonView<Views.CounterView>()
            VMKey.Create<ViewModels.AboutViewModel>(), ViewRegistration.SingletonView<Views.AboutView>()
            VMKey.Create<ViewModels.ChartViewModel>(), ViewRegistration.SingletonView<Views.ChartView>()
            VMKey.Create<ViewModels.FilePickerViewModel>(), ViewRegistration.SingletonView<Views.FilePickerView>()
        ]
        
