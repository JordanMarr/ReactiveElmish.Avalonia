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
            VM.Create<ViewModels.MainViewModel>(), View.Singleton(mainView)
            VM.Create<ViewModels.CounterViewModel>(), View.Singleton<Views.CounterView>()
            VM.Create<ViewModels.AboutViewModel>(), View.Singleton<Views.AboutView>()
            VM.Create<ViewModels.ChartViewModel>(), View.Singleton<Views.ChartView>()
            VM.Create<ViewModels.FilePickerViewModel>(), View.Singleton<Views.FilePickerView>()
        ]
        
