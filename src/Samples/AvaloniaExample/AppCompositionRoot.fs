namespace AvaloniaExample

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open Elmish.Avalonia
open Microsoft.Extensions.DependencyInjection

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainView = Views.MainView()

    override this.RegisterServices(services) = 
        services.AddSingleton<FileService>(FileService(mainView))

    override this.RegisterViews() = 
        Map [
            VMKey.Create<ViewModels.MainViewModel>(), ViewRegistration.SingletonView(mainView)
            VMKey.Create<ViewModels.CounterViewModel>(), ViewRegistration.SingletonView<Views.CounterView>()
            VMKey.Create<ViewModels.AboutViewModel>(), ViewRegistration.SingletonView<Views.AboutView>()
            VMKey.Create<ViewModels.ChartViewModel>(), ViewRegistration.SingletonView<Views.ChartView>()
            VMKey.Create<ViewModels.FilePickerViewModel>(), ViewRegistration.SingletonView<Views.FilePickerView>()
        ]
        
