namespace AvaloniaExample

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open AvaloniaExample.ViewModels

type ViewLocator() =
    interface IDataTemplate with
        
        member this.Build(data) =
            let t = data.GetType()
            let viewName = t.FullName.Replace("ViewModels", "Views").Replace("ViewModel", "View")
            let parts = viewName.Split([|'['; '+'|], StringSplitOptions.RemoveEmptyEntries)
            let name = parts[1]
            let typ = Type.GetType(name)
            if isNull typ then
                upcast TextBlock(Text = sprintf "Not Found: %s" name)
            else
                let vm = data :?> IStart
                let view = downcast Activator.CreateInstance(typ)
                vm.Start(view)
                view
                
        member this.Match(data) = 
            data :? IStart
