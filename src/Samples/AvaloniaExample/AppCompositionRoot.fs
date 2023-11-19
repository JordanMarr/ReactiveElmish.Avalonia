namespace AvaloniaExample

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open Elmish.Avalonia
open Microsoft.Extensions.DependencyInjection
open AvaloniaExample.ViewModels
open AvaloniaExample.Views

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainView = MainView()

    override this.RegisterServices(services) = 
        services.AddSingleton<FileService>(FileService(mainView))

    override this.RegisterViews() = 
        Map [
            VM.Create<MainViewModel>(), View.Singleton(mainView)
            VM.Create<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Create<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Create<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Create<FilePickerViewModel>(), View.Singleton<FilePickerView>()
        ]
        
