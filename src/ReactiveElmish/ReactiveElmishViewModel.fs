namespace ReactiveElmish

open System.ComponentModel
open System.Reactive.Linq
open System
open System.Linq
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open DynamicData
open System.Collections.ObjectModel
open ReactiveUI

[<AutoOpen>]
module private Utils = 
    let readOnlyCollection<'T>() = 
        new ReadOnlyObservableCollection<'T>(new ObservableCollection<'T>())

[<AllowNullLiteral>]
type ReactiveElmishViewModel(onPropertyChanged: string -> unit) = 
    inherit ReactiveUI.ReactiveObject()

    let disposables = ResizeArray<IDisposable>()
    let propertySubscriptions = Dictionary<string, IDisposable>()
    let propertyCollections = Dictionary<string, obj>()

    new() as this =
        let opc propertyName = this.RaisePropertyChanged(propertyName)
        new ReactiveElmishViewModel(opc)

    /// Fires the `PropertyChanged` event for the given property name. Uses the caller's name if no property name is given.
    member this.OnPropertyChanged([<CallerMemberName; Optional; DefaultParameterValue("")>] ?propertyName: string) =
        this.RaisePropertyChanged(propertyName.Value)

    /// Binds a VM property to a `modelProjection` value and refreshes the VM property when the `modelProjection` value changes.
    member this.Bind<'Model, 'ModelProjection>(
            store: IStore<'Model>, 
            modelProjection: 'Model -> 'ModelProjection, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to the 'Model projection and stores it in a dictionary.
            let disposable = 
                store.Observable
                    .DistinctUntilChanged(modelProjection)                    
                    .Subscribe(fun _ -> 
                        // Alerts the view that the 'Model projection / VM property has changed.
                        onPropertyChanged(vmPropertyName)
                        #if DEBUG
                        printfn $"PropertyChanged: {vmPropertyName}"
                        #endif
                    )
            propertySubscriptions.Add(vmPropertyName, disposable)

        // Returns the initial value from the model projection.
        modelProjection store.Model

    /// Binds a VM property to a `modelProjection` value and refreshes the VM property when the `onChanged` value changes.
    /// The `modelProjection` function will only be called when the `onChanged` value changes.
    /// `onChanged` usually returns a property value or a tuple of property values.
    member this.BindOnChanged<'Model, 'OnChanged, 'ModelProjection>(
            store: IStore<'Model>, 
            onChanged: 'Model -> 'OnChanged,
            modelProjection: 'Model -> 'ModelProjection, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to the 'Model projection and stores it in a dictionary.
            let disposable = 
                store.Observable
                    .DistinctUntilChanged(onChanged)
                    .Subscribe(fun x -> 
                        // Alerts the view that the 'Model projection / VM property has changed.
                        onPropertyChanged(vmPropertyName)
                        #if DEBUG
                        printfn $"PropertyChanged: {vmPropertyName}"
                        #endif
                    )
            
            propertySubscriptions.Add(vmPropertyName, disposable)

        modelProjection store.Model

    /// <summary>
    /// Binds a model collection property to a DynamicData SourceList.
    /// </summary>
    /// <param name="store">The reactive store to bind to.</param>
    /// <param name="modelProjectionSeq">The model projection.</param>
    member this.BindList<'Model, 'ModelProjection>(
            store: IStore<'Model>, 
            modelProjectionSeq: 'Model -> 'ModelProjection seq,
            ?sortBy: Func<'ModelProjection, IComparable>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        if not (propertyCollections.ContainsKey vmPropertyName.Value) then
            let mutable readOnlyList = readOnlyCollection<'ModelProjection>()
            let sourceList = SourceList.createFrom (modelProjectionSeq store.Model)
            sourceList
                .Connect()
                |> fun x -> 
                    match sortBy with
                    | Some sortBy -> x.Sort(DynamicData.Binding.SortExpressionComparer.Ascending(sortBy))
                    | None -> x.Sort(Comparer.Create(fun _ _ -> 0))
                |> _.Bind(&readOnlyList)
                .Subscribe()
                |> this.AddDisposable

            store.Observable
                .Select(modelProjectionSeq)
                //.DistinctUntilChanged()
                .Subscribe(sourceList.EditDiff)
                |> this.AddDisposable

            this.AddDisposable(sourceList)
            propertyCollections.Add(vmPropertyName.Value, readOnlyList)
            readOnlyList
        else 
            propertyCollections[vmPropertyName.Value] :?> ReadOnlyObservableCollection<'ModelProjection>

    /// <summary>
    /// Binds a model collection property to a DynamicData SourceList.
    /// </summary>
    /// <param name="store">The reactive store to bind to.</param>
    /// <param name="modelProjectionSeq">The model projection.</param>
    /// <param name="map">A function that transforms each item in the collection when it is added to the SourceList.</param>
    member this.BindList<'Model, 'ModelProjection, 'Mapped>(
            store: IStore<'Model>, 
            modelProjectionSeq: 'Model -> 'ModelProjection seq,
            map: 'ModelProjection -> 'Mapped,
            ?sortBy: Func<'Mapped, IComparable>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertyCollections.ContainsKey vmPropertyName) then
            let mutable readOnlyList = readOnlyCollection<'Mapped>()
            let sourceList = SourceList.createFrom (modelProjectionSeq store.Model)
            
            sourceList
                .Connect()
                .Transform(map)
                |> fun x -> 
                    match sortBy with
                    | Some sortBy -> x.Sort(DynamicData.Binding.SortExpressionComparer.Ascending(sortBy))
                    | None -> x.Sort(Comparer.Create(fun _ _ -> 0))
                |> _.Bind(&readOnlyList)
                .Subscribe()
                |> this.AddDisposable

            store.Observable
                .Select(modelProjectionSeq)
                //.DistinctUntilChanged()
                .Subscribe(sourceList.EditDiff)
                |> this.AddDisposable

            this.AddDisposable(sourceList)
            propertyCollections.Add(vmPropertyName, readOnlyList)
            readOnlyList
        else 
            propertyCollections[vmPropertyName] :?> ReadOnlyObservableCollection<'Mapped>

    /// <summary>
    /// Binds a model Map<'Key, 'Value> property to an ObservableCollection.
    /// </summary>
    /// <param name="store">The reactive store to bind to.</param>
    /// <param name="modelProjection">The model projection.</param>
    /// <param name="map">A function that transforms the item when it is added.</param>
    /// <param name="getKey">A function that returns the identifier of the item.</param>
    /// <param name="update">An optional function that updates the transformed item when it is updated in the model. NOTE: This is expensive as it requires all items to be compared.</param>
    /// <param name="sortBy">An optional function that returns a sort expression.</param>
    member this.BindKeyedList<'Model, 'Key, 'Value, 'Mapped when 'Value : equality and 'Mapped : not struct and 'Key : comparison>(
            store: IStore<'Model>, 
            modelProjection: 'Model -> Map<'Key, 'Value>,
            map: 'Value -> 'Mapped,
            getKey: 'Mapped -> 'Key,
            ?update: Action<'Value, 'Mapped>,
            ?sortBy: Func<'Mapped, IComparable>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertyCollections.ContainsKey vmPropertyName) then
            let mutable lastModelMap: Map<'Key, 'Value> = Map.empty
            let mutable observableCollection = ObservableCollection()
            let mapToObservableCollection (currentModelMap: Map<'Key, 'Value>) = 

                // Apply updates to existing items (excluding new or removed items)
                update 
                |> Option.iter (fun update -> 
                    let existingItemVms = 
                        // Get items from the lastModelMap that exist in the currentModelMap
                        observableCollection
                        |> Seq.map (fun item -> getKey item, item)
                        |> Seq.filter (fun (key, _) -> 
                            // Only include items that exist in the currentModelMap
                            currentModelMap.ContainsKey key
                        )
                        |> Seq.toArray

                    for idx = existingItemVms.Length - 1 downto 0 do
                        let (key, itemVM) = existingItemVms[idx]
                        let newItem = currentModelMap[key]
                        let oldItem = lastModelMap[key]
                        if newItem <> oldItem then
                            update.Invoke(newItem, itemVM)
                )

                // Get items from the currentModelMap that are missing from the lastModelMap
                let newItems = 
                    currentModelMap
                    |> Map.filter (fun k _ -> not (lastModelMap.ContainsKey k))
                    |> Map.toSeq

                // Add new items to the observableCollection
                newItems 
                |> Seq.map (snd >> map)
                |> Seq.iter observableCollection.Add

                // Get items that have been removed from the currentModelMap
                let removedKeys = 
                    lastModelMap
                    |> Map.filter (fun k _ -> not (currentModelMap.ContainsKey k))
                    |> Map.toArray
                    |> Seq.map fst
                    |> Set.ofSeq

                if removedKeys.Count > 0 then 
                    let indexesToRemove = 
                        observableCollection
                        |> Seq.mapi (fun idx item -> idx, getKey item)
                        |> Seq.filter (fun (_, key) -> removedKeys.Contains(key))
                        |> Seq.map fst
                        |> Set.ofSeq
                    
                    for idx = observableCollection.Count - 1 downto 0 do
                        if indexesToRemove.Contains(idx) then
                            let removedItem = observableCollection[idx]
                            observableCollection.RemoveAt(idx)
                            match removedItem :> obj with
                            | :? IDisposable as disposable -> 
                                this.AddDisposable(disposable)
                            | _ -> ()
                            

                // Sort the observableCollection
                sortBy
                |> Option.iter (fun sortBy ->
                    observableCollection
                    |> Seq.sortBy sortBy.Invoke
                    |> Seq.iteri (fun idx item -> observableCollection[idx] <- item)
                )

                // Finally, update the lastModelMap
                lastModelMap <- currentModelMap

            // Create an initial observable collection from the modelProjection
            modelProjection store.Model |> mapToObservableCollection

            // Creates a subscription to a SourceCache and stores it in a dictionary.
            let disposable = 
                store.Observable
                    .Select(modelProjection)
                    .DistinctUntilChanged()
                    .Subscribe(mapToObservableCollection)

            this.AddDisposable disposable
            propertyCollections.Add(vmPropertyName, observableCollection)
            observableCollection
        else 
            propertyCollections[vmPropertyName] :?> ObservableCollection<'Mapped>

    /// <summary>
    /// Binds a model Map<'Key, 'Value> property to an ObservableCollection.
    /// </summary>
    /// <param name="store">The reactive store to bind to.</param>
    /// <param name="modelProjection">The model projection.</param>
    /// <param name="getKey">A function that returns the identifier of the item.</param>
    /// <param name="update">An optional function that updates the transformed item when it is updated in the model. NOTE: This is expensive as it requires all items to be compared.</param>
    /// <param name="sortBy">A function that returns a sort expression.</param>
    member this.BindKeyedList<'Model, 'Key, 'Value when 'Value: equality and 'Value : not struct and 'Key : comparison>(
            store: IStore<'Model>, 
            modelProjection: 'Model -> Map<'Key, 'Value>,
            getKey: 'Value -> 'Key,
            ?update: Action<'Value, 'Value>,
            ?sortBy,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName: string
        ) = 
        this.BindKeyedList(store = store, modelProjection = modelProjection, map = id, getKey = getKey, ?update = update, ?sortBy = sortBy, ?vmPropertyName = vmPropertyName)

    /// Binds a VM property to a 'Model DynamicData.ISourceList<'T> property.
    member this.BindSourceList<'T>(
            sourceList: ISourceList<'T>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertyCollections.ContainsKey vmPropertyName) then
            let mutable readOnlyList = readOnlyCollection<'T>()
            // Creates a subscription to a ISourceList<'T> and stores it in a dictionary.
            let disposable = 
                sourceList
                    .Connect()
                    .Bind(&readOnlyList)
                    .Subscribe()
            this.AddDisposable disposable
            propertyCollections.Add(vmPropertyName, readOnlyList)
            readOnlyList
        else
            propertyCollections[vmPropertyName] :?> ReadOnlyObservableCollection<'T>

    /// Binds a VM property to a 'Model DynamicData.ISourceList<'T> property.
    member this.BindSourceList<'T, 'Mapped>(
            sourceList: ISourceList<'T>, 
            map: 'T -> 'Mapped,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertyCollections.ContainsKey vmPropertyName) then
            let mutable readOnlyList = readOnlyCollection<'Mapped>()
            // Creates a subscription to a ISourceList<'T> and stores it in a dictionary.
            let disposable = 
                sourceList
                    .Connect()
                    .Transform(map)
                    .Bind(&readOnlyList)
                    .Subscribe()
            this.AddDisposable disposable
            propertyCollections.Add(vmPropertyName, readOnlyList)
            readOnlyList
        else
            propertyCollections[vmPropertyName] :?> ReadOnlyObservableCollection<'Mapped>

    /// Binds a VM property to a 'Model DynamicData.IObservableCache<'Value, 'Key> property.
    member this.BindSourceCache<'Value, 'Key>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            ?sortBy: Func<'Value, IComparable>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertyCollections.ContainsKey vmPropertyName) then
            let mutable readOnlyList = readOnlyCollection<'Value>()
            // Creates a subscription to a SourceCache and stores it in a dictionary.
            let disposable = 
                sourceCache
                    .Connect()
                    |> fun x -> 
                        match sortBy with
                        | Some sortBy ->
                            x.SortBy(sortBy)
                        | None -> 
                            x.Sort(Comparer.Create(fun _ _ -> 0))
                    |> _.Bind(&readOnlyList)
                    |> _.Subscribe()
            this.AddDisposable disposable
            propertyCollections.Add(vmPropertyName, readOnlyList)
            readOnlyList
        else
            propertyCollections[vmPropertyName] :?> ReadOnlyObservableCollection<'Value>

    /// <summary>
    /// Binds a VM property to a 'Model DynamicData.IObservableCache<'Value, 'Key> property.
    /// </summary>
    /// <param name="sourceCache">A SourceCache to bind.</param>
    /// <param name="map">A function that transforms the item when it is added.</param>
    /// <param name="update">An optional function that updates the transformed item when it is updated in the model.</param>
    /// <param name="sortBy">An optional function that returns a sort expression.</param>
    member this.BindSourceCache<'Value, 'Key, 'Mapped when 'Value : not struct and 'Mapped : not struct>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            map: 'Value -> 'Mapped,
            ?filter: Func<'Value, bool>,
            ?update: Action<'Value, 'Mapped>,
            ?sortBy,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        if not (propertyCollections.ContainsKey vmPropertyName) then
            let mutable readOnlyList = readOnlyCollection<'Mapped>()
            // Creates a subscription to a SourceCache and stores it in a dictionary.
            let disposable = 
                sourceCache
                    .Connect()
                    |> fun x -> 
                        match filter with
                        | Some filter ->
                            x.Filter(filter)
                        | None -> 
                            x
                    |> fun x -> 
                        match update with
                        | Some update ->
                            x.TransformWithInlineUpdate(map, fun mapped value -> update.Invoke(value, mapped))
                        | None ->
                            x.Transform(map)
                    |> fun x -> 
                        match sortBy with
                        | Some sortBy ->
                            x.SortBy(sortBy)
                        | None -> 
                            x.Sort(Comparer.Create(fun _ _ -> 0))
                    |> _.Bind(&readOnlyList)
                    |> _.Subscribe()
            this.AddDisposable disposable
            propertyCollections.Add(vmPropertyName, readOnlyList)
            readOnlyList
        else 
            propertyCollections[vmPropertyName] :?> ReadOnlyObservableCollection<'Mapped>

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

