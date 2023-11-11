namespace AvaloniaExample

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open Avalonia.Markup.Xaml


type CounterComponentUC() as this =
    inherit UserControl()

    let view =
        Component (fun ctx ->
            let state = ctx.useState 0

            DockPanel.create [
                DockPanel.children [
                    TextBlock.create [
                        TextBlock.dock Dock.Top
                        TextBlock.fontSize 24
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text "Powered by Avalonia.FuncUI"
                    ]
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
                        TextBlock.fontSize 20.0
                        TextBlock.verticalAlignment VerticalAlignment.Center
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.text (string state.Current)
                    ]
                ]
            ]
        )

    do 
        this.InitializeComponent()
        base.Height <- 300.0
        base.Width <- 300.0
        base.VerticalAlignment <- VerticalAlignment.Top
        base.Content <- view

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)


    