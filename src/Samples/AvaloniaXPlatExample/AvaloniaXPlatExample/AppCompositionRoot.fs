namespace AvaloniaXPlatExample

open Avalonia.Controls
open ReactiveElmish.Avalonia
open AvaloniaXPlatExample.ViewModels
open AvaloniaXPlatExample.Views
open AvaloniaXPlatExample.Services
open Microsoft.Extensions.DependencyInjection

type AppCompositionRoot(isSingleWindow:bool) =
    inherit CompositionRoot()


    override this.RegisterServices services =
        base.RegisterServices services |> ignore
        // For web version, we can't pass a window to the file service
        if isSingleWindow then
            services
        else
            let mainWindow = MainWindow()
            services.AddSingleton<FileService>(FileService(mainWindow))

    override this.RegisterViews() =
        Map [
            VM.Key<MainViewModel>(), View.Singleton<MainView>()
            VM.Key<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
            VM.Key<ListBoxViewModel>(), View.Singleton<ListBoxView>()
        ]

