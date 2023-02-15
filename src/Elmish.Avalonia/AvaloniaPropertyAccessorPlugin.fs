namespace Elmish.Avalonia

open System
open System.ComponentModel
open Avalonia.Data.Core.Plugins
open Avalonia.Utilities

/// Allows DictionaryViewModel to be bound to Avalonia views.
type AvaloniaPropertyAccessorPlugin() =
    interface IPropertyAccessorPlugin with
        member __.Match(obj, _) =
            obj :? IDictionaryViewModel

        member __.Start(reference, propertyName) =
            if isNull reference then nullArg (nameof reference)
            if isNull propertyName then nullArg (nameof propertyName)
            new Accessor(reference, propertyName)

and Accessor(reference : WeakReference<obj>, property : string) = 
    inherit PropertyAccessorBase()

    let mutable eventRaised = false
    let mutable propertyChangedSubscription : IDisposable option = None

    interface IWeakEventSubscriber<PropertyChangedEventArgs> with
        member __.OnEvent(_, _, e) = 
            if e.PropertyName = property || String.IsNullOrEmpty(e.PropertyName) then
                eventRaised <- true
                __.SendCurrentValue()

    override __.PropertyType =
      __.Value.GetType()

    override __.Value with get() =
        match reference.TryGetTarget() with
        | true, target -> 
            match target with
            | :? IDictionaryViewModel as vm ->
                vm.GetMemberByName(property)
            | _ -> null
        | _ -> null

    override __.SetValue(value, _) = 
        match reference.TryGetTarget() with 
        | true, target -> 
            match target with
            | :? IDictionaryViewModel as vm ->
                vm.SetMemberByName(property, value)
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
            | :? INotifyPropertyChanged -> 
                propertyChangedSubscription |> Option.iter (fun sub -> sub.Dispose())
            | _ -> ()                
        | _ -> ()

    member __.SendCurrentValue() = 
        try
            let value = __.Value
            __.PublishValue(value)
        with _ -> ()

    member __.SubscribeToChanges() = 
        match reference.TryGetTarget() with
        | true, target ->
            match target with
            | :? INotifyPropertyChanged as inpc ->
                propertyChangedSubscription <-
                    inpc.PropertyChanged.Subscribe(fun _ ->
                        __.SendCurrentValue()
                    )
                    |> Some
            | _ -> ()
        | _ -> ()

module AppBuilder =

    type Avalonia.AppBuilder with

    /// Uses the Elmish.Avalonia bindings.
    member appBuilder.UseElmishBindings() =
        BindingPlugins.PropertyAccessors.Add(AvaloniaPropertyAccessorPlugin())
        appBuilder