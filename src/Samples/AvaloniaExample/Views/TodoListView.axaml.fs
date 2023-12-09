namespace AvaloniaExample.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml

type TodoListView () as this = 
    inherit UserControl ()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)
