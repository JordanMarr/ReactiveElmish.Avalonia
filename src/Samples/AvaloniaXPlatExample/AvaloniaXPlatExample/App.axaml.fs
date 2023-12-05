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
        let _ = typeof<Avalonia.Controls.DataGrid>

        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        
        let appRoot = AppCompositionRoot()

        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            desktop.MainWindow <- 
                MainWindow(Content = appRoot.GetView<ViewModels.MainViewModel>())

        | :? ISingleViewApplicationLifetime as singleViewLifetime ->
            try
                singleViewLifetime.MainView <- appRoot.GetView<ViewModels.MainViewModel>()
            with x ->
                printfn $"Exception: {x.Message} \n {x.StackTrace}"

        | _ ->
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
