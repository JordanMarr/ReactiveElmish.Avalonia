namespace AvaloniaExample

open Avalonia
open Avalonia.Markup.Xaml
open AvaloniaExample.Views
open Avalonia.Controls.ApplicationLifetimes
open Elmish.Avalonia

type App() =
    inherit Application()

    override this.Initialize() =
        // Initialize Avalonia controls from NuGet packages:
        let _ = typeof<Avalonia.Controls.DataGrid>

        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->         
            let view = MainView()
            desktop.MainWindow <- view
            Services.Init view
            let vm = new ViewModels.MainViewModel()
            ViewBinder.bindViewModel vm view
        | _ -> 
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
