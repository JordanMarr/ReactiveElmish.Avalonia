namespace AvaloniaExample

open Elmish.Avalonia
open Microsoft.Extensions.DependencyInjection
open AvaloniaExample.ViewModels
open AvaloniaExample.Views

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainView = MainView()

    override this.RegisterServices services = 
        base.RegisterServices services |> ignore
        services.AddSingleton<FileService>(FileService(mainView))

    override this.RegisterViews() = 
        Map [
            VM.Key<MainViewModel>(), View.Singleton(mainView)
            VM.Key<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
        ]
        
