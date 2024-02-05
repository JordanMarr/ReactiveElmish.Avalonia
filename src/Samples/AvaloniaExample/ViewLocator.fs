namespace AvaloniaExample

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open Avalonia
open ReactiveElmish
open ReactiveElmish.Avalonia

type ViewLocator() =
    interface IDataTemplate with
        
        member this.Build(data) =
            match data with
            | :? ReactiveElmishViewModel as reactiveViewModel -> AppCompositionRoot.Instance.GetViewFor(reactiveViewModel)
            | _ ->
                let t = data.GetType()
                let viewName = t.FullName.Replace("ViewModels", "Views").Replace("ViewModel", "View")
                let parts = viewName.Split([|'['; '+'|], StringSplitOptions.RemoveEmptyEntries)
                let name = 
                    if parts.Length > 2 
                    then parts[1]
                    else parts[0]
                let viewType = Type.GetType(name)
                if isNull viewType then
                    TextBlock(Text = sprintf "Not Found: %s" name)
                else
                    let view = downcast Activator.CreateInstance(viewType)
                    match data with 
                    | :? ReactiveUI.ReactiveObject as vm ->
                        ViewBinder.bindWithDisposeOnViewUnload (vm, view) |> snd
                    | _ ->
                        TextBlock(Text = sprintf $"Not found: %s{name}")
                
        member this.Match(data) = 
            match data with
            | :? ReactiveElmishViewModel as vm -> AppCompositionRoot.Instance.HasViewFor(vm)
            | :? ReactiveUI.ReactiveObject -> true
            | _ -> false

