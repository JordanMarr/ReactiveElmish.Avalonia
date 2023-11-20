namespace AvaloniaXPlatExample

open Elmish.Avalonia
open AvaloniaXPlatExample.ViewModels
open AvaloniaXPlatExample.Views
open AvaloniaXPlatExample.Services
open Microsoft.Extensions.DependencyInjection
open ReactiveUI

type AppCompositionRoot() =
    inherit CompositionRoot()

    let mainWindow = MainWindow()

    override this.RegisterServices services = 
        services.AddSingleton<FileService>(FileService(mainWindow))

    override this.RegisterViews() = 
        Map [
            VM.Key<MainViewModel>(), View.Singleton<MainView>()
            VM.Key<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
        ]
        
