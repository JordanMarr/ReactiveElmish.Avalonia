namespace AvaloniaExample

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Controls.ApplicationLifetimes

type App() =
    inherit Application()

    override this.Initialize() =
        // Initialize Avalonia controls from NuGet packages:
        let _ = typeof<Avalonia.Controls.DataGrid>

        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->         
            let appRoot = AppCompositionRoot.Instance
            desktop.MainWindow <- appRoot.GetView<ViewModels.MainViewModel>() :?> Window
        | _ -> 
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
