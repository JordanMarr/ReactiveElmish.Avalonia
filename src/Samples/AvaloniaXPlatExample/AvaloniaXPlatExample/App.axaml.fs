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
            let mainWindow = MainWindow()
            let appRoot = AppCompositionRoot(mainWindow)
            mainWindow.Content <- appRoot.GetView<ViewModels.MainViewModel>()
            desktop.MainWindow <- mainWindow

        | :? ISingleViewApplicationLifetime as singleViewLifetime ->
            printfn "OnFrameworkInitializationCompleted - ISingleViewApplicationLifetime"
            try
                printfn "get appRoot"
                let appRoot = AppCompositionRoot()
                printfn "set mainview"
                singleViewLifetime.MainView <- appRoot.GetView<ViewModels.MainViewModel>()
                printfn "call base"
            with x ->
                printfn $"Exception: {x.Message} \n {x.StackTrace}"

        | _ ->
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
