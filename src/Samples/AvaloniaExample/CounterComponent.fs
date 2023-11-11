namespace AvaloniaExample

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Markup.Xaml

module CounterComponent =

    let view =
        Component (fun ctx ->
            let state = ctx.useState 0

            DockPanel.create [
                DockPanel.children [
                    Button.create [
                        Button.dock Dock.Bottom
                        Button.onClick (fun _ -> state.Current - 1 |> state.Set)
                        Button.content "-"
                        Button.horizontalAlignment HorizontalAlignment.Stretch
                    ]
                    Button.create [
                        Button.dock Dock.Bottom
                        Button.onClick (fun _ -> state.Current + 1 |> state.Set)
                        Button.content "+"
                        Button.horizontalAlignment HorizontalAlignment.Stretch
                    ]
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.fontSize 48.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text (string state.Current)
                    ]
                ]
            ]
        )

type CounterComponentUC() as this =
    inherit UserControl()

    do 
        this.InitializeComponent()
        base.Height <- 400.0
        base.Width <- 400.0
        base.Content <- CounterComponent.view

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)


    