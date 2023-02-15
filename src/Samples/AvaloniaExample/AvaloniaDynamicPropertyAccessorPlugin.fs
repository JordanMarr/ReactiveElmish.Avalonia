namespace AvaloniaFSharp

open System
open System.Collections.Generic
open System.ComponentModel
open System.Diagnostics.Contracts
open Avalonia.Data
open Avalonia.Data.Core.Plugins
open Avalonia.Utilities

type AvaloniaDynamicPropertyAccessorPlugin() =
    interface IPropertyAccessorPlugin with
        member __.Match(obj, propertyName) = obj :? IDictionary<string, obj>
        member __.Start(reference, propertyName) = 
            Contract.Requires<ArgumentNullException>(reference <> null)
            Contract.Requires<ArgumentNullException>(propertyName <> null)
            new Accessor(reference, propertyName)

and Accessor(reference : WeakReference<obj>, property : string) = 
    inherit PropertyAccessorBase()

    let mutable eventRaised = false

    interface IWeakEventSubscriber<PropertyChangedEventArgs> with
        member __.OnEvent(sender, ev, e) = 
            if e.PropertyName = property || String.IsNullOrEmpty(e.PropertyName) then
                eventRaised <- true
                __.SendCurrentValue()

    override __.PropertyType = typeof<unit>
    override __.Value with get() =
        match reference.TryGetTarget() with
        | true, target -> 
            match target with
            | :? IDictionary<string, obj> as o ->
                match o.TryGetValue(property) with
                | true, value -> value
                | _ -> null
            | _ -> null
        | _ -> null

    override __.SetValue(value, priority) = 
        match reference.TryGetTarget() with
        | true, target -> 
            match target with
            | :? IDictionary<string, obj> as o -> 
                o.[property] <- value
                true
            | _ -> false
        | _ -> false

    override __.SubscribeCore() = 
        __.SendCurrentValue()
        __.SubscribeToChanges()

    override __.UnsubscribeCore() =
        match reference.TryGetTarget() with
        | true, target -> 
            match target with
            | :? INotifyPropertyChanged as inpc -> 
                //let wes = __ :> IWeakEventSubscriber<PropertyChangedEventArgs>
                //let eh = wes.OnEvent |> unbox<EventHandler<PropertyChangedEventArgs>>
                WeakEventHandlerManager.Unsubscribe(
                    inpc,
                    nameof(inpc.PropertyChanged),
                    (fun sender e -> ()))
            | _ -> ()
                
        | _ -> ()

    member __.SendCurrentValue() = 
        try
            __.PublishValue(__.Value)
        with _ -> ()

    member __.SubscribeToChanges() = 
        match reference.TryGetTarget() with
        | true, target -> 
            match target with
            | :? INotifyPropertyChanged as inpc ->
                //let wes = __ :> IWeakEventSubscriber<PropertyChangedEventArgs>
                //let eh = wes.OnEvent |> unbox<EventHandler<PropertyChangedEventArgs>>
                WeakEventHandlerManager.Subscribe(
                    inpc,
                    nameof(inpc.PropertyChanged),
                    (fun sender e -> ()))
            | _ -> ()
        | _ -> ()