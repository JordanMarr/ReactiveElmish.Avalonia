namespace AvaloniaXPlatExample

open Avalonia.Controls
open ReactiveElmish.Avalonia
open AvaloniaXPlatExample.ViewModels
open AvaloniaXPlatExample.Views
open AvaloniaXPlatExample.Services
open Microsoft.Extensions.DependencyInjection

type AppCompositionRoot(?mainWindow: MainWindow) =
    inherit CompositionRoot()


    override this.RegisterServices services =
        base.RegisterServices services |> ignore
        
        // For web version, we can't pass a window to the file service
        match mainWindow with
        | Some mainWindow ->
            services.AddSingleton<IFileService>(FileService(mainWindow))
        | None ->
            services.AddSingleton<IFileService>(WebFileService())

    override this.RegisterViews() =
        Map [
            VM.Key<MainViewModel>(), View.Singleton<MainView>()
            VM.Key<CounterViewModel>(), View.Singleton<CounterView>()
            VM.Key<AboutViewModel>(), View.Singleton<AboutView>()
            VM.Key<ChartViewModel>(), View.Singleton<ChartView>()
            VM.Key<FilePickerViewModel>(), View.Singleton<FilePickerView>()
            VM.Key<ListBoxViewModel>(), View.Singleton<ListBoxView>()
        ]

