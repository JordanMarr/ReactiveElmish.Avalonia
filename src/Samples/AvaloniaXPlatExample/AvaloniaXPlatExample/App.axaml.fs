namespace AvaloniaXPlatExample

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open AvaloniaXPlatExample.Views
open Avalonia.Controls.ApplicationLifetimes

type App() =
    inherit Application()

    override this.Initialize() =
        // Initialize Avalonia controls from NuGet packages:
        printfn "Initializing Avalonia"
        let _ = typeof<Avalonia.Controls.DataGrid>
        printfn "loading Avalonia"

        AvaloniaXamlLoader.Load(this)
        printfn "loaded - end of initialize"

    override this.OnFrameworkInitializationCompleted() =


        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            let appRoot = AppCompositionRoot()
            desktop.MainWindow <-
                MainWindow(Content = appRoot.GetView<ViewModels.MainViewModel>())
            base.OnFrameworkInitializationCompleted()

        | :? ISingleViewApplicationLifetime as singleViewLifetime ->
            printfn "OnFrameworkInitializationCompleted - ISingleViewApplicationLifetime"
            try
                printfn "get mainview"
                let appRoot = AppCompositionRoot()
                // let view = MainView()
                singleViewLifetime.MainView <- appRoot.GetView<ViewModels.MainViewModel>()
                // let x = ViewModels.MainViewModel.DesignVM
                printfn "start elmish loop"
                // x.StartElmishLoop(view)
                base.OnFrameworkInitializationCompleted()
            with x ->
                printfn $"Exception: {x.Message} \n {x.StackTrace}"

        | _ ->
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
