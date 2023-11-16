namespace AvaloniaExample

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open Elmish.Avalonia

type ViewLocator() =
    interface IDataTemplate with
        
        member this.Build(data) =
            let t = data.GetType()
            let viewName = t.FullName.Replace("ViewModels", "Views").Replace("ViewModel", "View")
            let parts = viewName.Split([|'['; '+'|], StringSplitOptions.RemoveEmptyEntries)
            let name = 
                if parts.Length > 2 
                then parts[1]
                else parts[0]
            let viewType = Type.GetType(name)
            if isNull viewType then
                upcast TextBlock(Text = sprintf "Not Found: %s" name)
            else
                let view = downcast Activator.CreateInstance(viewType)
                match data with 
                | :? IElmishViewModel as vm -> 
                    let vm = data :?> IElmishViewModel
                    vm.StartElmishLoop(view)
                    view
                | :? ReactiveUI.ReactiveObject as vm ->
                    let vm = data :?> ReactiveUI.ReactiveObject
                    view.DataContext <- vm
                    view
                
        member this.Match(data) = 
            // Only apply this IDataTemplate when data is an IElmishViewModel
            data :? IElmishViewModel || data :? ReactiveUI.ReactiveObject
