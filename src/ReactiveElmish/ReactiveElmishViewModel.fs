namespace ReactiveElmish

open System.ComponentModel
open System.Reactive.Linq
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open DynamicData
open System.Collections.ObjectModel

type ReactiveElmishViewModel() = 
    inherit ReactiveUI.ReactiveObject()

    let disposables = ResizeArray<IDisposable>()
    let propertyChanged = Event<_, _>()
    let propertySubscriptions = Dictionary<string, IDisposable>()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = propertyChanged.Publish

    /// Fires the `PropertyChanged` event for the given property name. Uses the caller's name if no property name is given.
    member this.OnPropertyChanged([<CallerMemberName; Optional; DefaultParameterValue("")>] ?propertyName: string) =
        propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName.Value))

    /// Binds a VM property to a `modelProjection` value and refreshes the VM property when the `modelProjection` value changes.
    member this.Bind<'Model, 'Msg, 'ModelProjection>(store: IStore<'Model, 'Msg>, 
                                                        modelProjection: 'Model -> 'ModelProjection, 
                                                        [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to the 'Model projection and stores it in a dictionary.
            let disposable = 
                store.Observable
                    .DistinctUntilChanged(modelProjection)
                    .Subscribe(fun _ -> 
                        // Alerts the view that the 'Model projection / VM property has changed.
                        this.OnPropertyChanged(vmPropertyName)
                        #if DEBUG
                        printfn $"PropertyChanged: {vmPropertyName} by {this}"
                        #endif
                    )
            propertySubscriptions.Add(vmPropertyName, disposable)

        // Returns the initial value from the model projection.
        modelProjection store.Model

    /// Binds a VM property to a `modelProjection` value and refreshes the VM property when the `onChanged` value changes.
    /// The `modelProjection` function will only be called when the `onChanged` value changes.
    /// `onChanged` usually returns a property value or a tuple of property values.
    member this.BindOnChanged<'Model, 'Msg, 'OnChanged, 'ModelProjection>(store: IStore<'Model, 'Msg>, 
                                                        onChanged: 'Model -> 'OnChanged,
                                                        modelProjection: 'Model -> 'ModelProjection, 
                                                        [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to the 'Model projection and stores it in a dictionary.
            let disposable = 
                store.Observable
                    .DistinctUntilChanged(onChanged)
                    .Subscribe(fun x -> 
                        // Alerts the view that the 'Model projection / VM property has changed.
                        this.OnPropertyChanged(vmPropertyName)
                        #if DEBUG
                        printfn $"PropertyChanged: {vmPropertyName} by {this}"
                        #endif
                    )
            (store :?> ReactiveElmishStore<'Model, 'Msg>).Subject.OnNext(store.Model) // prime the pump
            propertySubscriptions.Add(vmPropertyName, disposable)

        modelProjection store.Model

    /// Binds a VM property to a 'Model DynamicData.ISourceList<'T> property.
    member this.BindSourceList<'Model, 'Msg, 'T>(store: IStore<'Model, 'Msg>, selectSourceList: 'Model -> ISourceList<'T>, [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable sourceList: ReadOnlyObservableCollection<'T> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to the 'Model SourceList<'T> property and stores it in a dictionary.
            let disposable = selectSourceList store.Model |> _.Connect().Bind(&sourceList).Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        sourceList

    /// Binds a VM property to a 'Model DynamicData.IObservableCache<'Value, 'Key> property.
    member this.BindSourceCache<'Model, 'Msg, 'Value, 'Key>(store: IStore<'Model, 'Msg>, selectSourceCache: 'Model -> IObservableCache<'Value, 'Key>, [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable sourceList: ReadOnlyObservableCollection<'Value> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to the 'Model SourceList<'T> property and stores it in a dictionary.
            let disposable = selectSourceCache store.Model |> _.Connect().Bind(&sourceList).Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        sourceList

    /// Subscribes to an IObservable<> and adds the subscription to the list of disposables.
    member this.Subscribe(observable: IObservable<'T>, handler: 'T -> unit) =
        observable 
        |> Observable.subscribe handler
        |> this.AddDisposable

    member this.AddDisposable(disposable: IDisposable) =
        disposables.Add(disposable)

    interface IDisposable with
        member this.Dispose() =
            disposables |> Seq.iter _.Dispose()
            propertySubscriptions.Values |> Seq.iter _.Dispose()
            propertySubscriptions.Clear()
