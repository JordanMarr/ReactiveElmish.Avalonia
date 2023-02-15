namespace AvaloniaExample

open Avalonia
open Avalonia.Markup.Xaml
open AvaloniaExample.Views
open Avalonia.Controls.ApplicationLifetimes

type App() =
    inherit Application()

    override this.Initialize() =
        // Initialize Avalonia controls from NuGet packages:
        let dataGridType = typeof<System.Windows.Controls.DataGrid>

        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->         
            let view = MainView()
            desktop.MainWindow <- view
            ViewModels.MainViewModel.vm.Start(view)
        | _ -> 
            // leave this here for design view re-renders
            ()

        base.OnFrameworkInitializationCompleted()
