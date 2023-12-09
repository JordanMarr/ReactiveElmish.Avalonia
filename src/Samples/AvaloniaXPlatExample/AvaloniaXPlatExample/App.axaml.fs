namespace AvaloniaXPlatExample

open Avalonia
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
            let view = MainWindow()
            desktop.MainWindow <- view
            Services.Init view
            try
                ViewModels.MainViewModel.vm.StartElmishLoop(view)
            with x ->
                printfn $"Exception: {x.Message} \n {x.StackTrace}"
        | :? ISingleViewApplicationLifetime as singleViewLifetime ->
            printfn "OnFrameworkInitializationCompleted - ISingleViewApplicationLifetime"
            try
                printfn "get mainview"
                let view = MainView()
                singleViewLifetime.MainView <- view
                let x = ViewModels.MainViewModel.vm
                printfn "start elmish loop"
                x.StartElmishLoop(view)
            with x ->
                printfn $"Exception: {x.Message} \n {x.StackTrace}"

        | _ ->
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
