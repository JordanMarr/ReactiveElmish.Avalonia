﻿#nowarn "0064" // FS0064: This construct causes code to be less generic than indicated by the type annotations.
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

type ReactiveElmishViewModel() = 
    inherit ReactiveUI.ReactiveObject()

    let disposables = ResizeArray<IDisposable>()
    let propertySubscriptions = Dictionary<string, IDisposable>()

    /// Fires the `PropertyChanged` event for the given property name. Uses the caller's name if no property name is given.
    member this.OnPropertyChanged([<CallerMemberName; Optional; DefaultParameterValue("")>] ?propertyName: string) =
        this.RaisePropertyChanged(propertyName.Value)

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

    /// Binds a model collection property to a DynamicData.ISourceList<'T>.
    member this.BindList<'Model, 'Msg, 'ModelProjection>(
            store: IStore<'Model, 'Msg>, 
            modelProjectionSeq: 'Model -> 'ModelProjection seq,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable readOnlyList: ReadOnlyObservableCollection<'T> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            let sourceList = SourceList.createFrom (modelProjectionSeq store.Model)
            readOnlyList <- this.BindSourceList(sourceList, vmPropertyName)
            let disposable = 
                store.Observable
                    .Select(modelProjectionSeq)
                    //.DistinctUntilChanged()
                    .Subscribe(sourceList.EditDiff)

            this.AddDisposable(sourceList)
            this.AddDisposable(disposable)
        readOnlyList

    /// Binds a model Map property to an ObservableCollection.
    member this.BindMap<'Model, 'Msg, 'Key, 'Value, 'Transformed when 'Transformed : not struct and 'Key : comparison>(
            store: IStore<'Model, 'Msg>, 
            modelProjection: 'Model -> Map<'Key, 'Value>,
            create: 'Value -> 'Transformed,
            getKey: 'Transformed -> 'Key,
            //?update,
            ?sortBy: 'Transformed -> IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable lastModelMap: Map<'Key, 'Value> = Unchecked.defaultof<_>
        let mutable observableCollection: ObservableCollection<'Transformed> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            observableCollection <- ObservableCollection()
            lastModelMap <- Map.empty
            
            let mapToObservableCollection (currentModelMap: Map<'Key, 'Value>) = 
                // Get items from the currentModelMap that are missing from the lastModelMap
                let newItems = 
                    currentModelMap
                    |> Map.filter (fun k _ -> not (lastModelMap.ContainsKey k))
                    |> Map.toSeq
                    |> Seq.map (snd >> create)

                //update 
                //|> Option.iter (fun update -> 
                //    // Get items from the lastModelMap that exist in the currentModelMap
                //    let existingItems = 
                //        lastModelMap
                //            |> Map.filter (fun k _ -> currentModelMap.ContainsKey k)
                //            |> Map.toSeq

                //)

                // Add new items to the observableCollection
                newItems |> Seq.iter observableCollection.Add

                // Get items that have been removed from the currentModelMap
                let removedKeys = 
                    lastModelMap
                    |> Map.filter (fun k _ -> not (currentModelMap.ContainsKey k))
                    |> Map.toArray
                    |> Seq.map fst
                    |> Set.ofSeq

                if removedKeys.Count > 0 then 
                    // Remove items from the observableCollection in reverse loop
                    //observableCollection |> Seq.findIndex (fun item -> getKey item = removedKeys.[0])
                    //observableCollection.Where(fun item idx -> getKey item = key)

                    let indexesToRemove = 
                        observableCollection
                        |> Seq.mapi (fun idx item -> idx, getKey item)
                        |> Seq.filter (fun (_, key) -> removedKeys.Contains(key))
                        |> Seq.map fst
                        |> Set.ofSeq
                    
                    for idx = observableCollection.Count - 1 downto 0 do
                        if indexesToRemove.Contains(idx) then
                            observableCollection.RemoveAt(idx)

                // Sort the observableCollection
                sortBy
                |> Option.iter (fun sortBy ->
                    observableCollection
                    |> Seq.sortBy sortBy
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

            propertySubscriptions.Add(vmPropertyName, disposable)
        observableCollection

    /// Binds a model Map property to an ObservableCollection.
    member this.BindMap<'Model, 'Msg, 'Key, 'Value when 'Value : not struct and 'Key : comparison>(
            store: IStore<'Model, 'Msg>, 
            modelProjection: 'Model -> Map<'Key, 'Value>,
            getKey: 'Value -> 'Key,
            ?sortBy: 'Value -> IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        match sortBy with 
        | Some sortBy ->
            this.BindMap(store, modelProjection, id, getKey, sortBy, vmPropertyName = vmPropertyName.Value)
        | None ->
            this.BindMap(store, modelProjection, id, getKey, vmPropertyName = vmPropertyName.Value)


    /// Binds a VM property to a 'Model DynamicData.ISourceList<'T> property.
    member this.BindSourceList<'T>(
            sourceList: ISourceList<'T>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable readOnlyList: ReadOnlyObservableCollection<'T> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to a ISourceList<'T> and stores it in a dictionary.
            let disposable = 
                sourceList
                    .Connect()
                    .Bind(&readOnlyList)
                    .Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        readOnlyList

    /// Binds a VM property to a 'Model DynamicData.ISourceList<'T> property.
    member this.BindSourceList<'T, 'Transformed>(
            sourceList: ISourceList<'T>, 
            map: 'T -> 'Transformed,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable readOnlyList: ReadOnlyObservableCollection<'Transformed> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to a ISourceList<'T> and stores it in a dictionary.
            let disposable = 
                sourceList
                    .Connect()
                    .Transform(map)
                    .Bind(&readOnlyList)
                    .Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        readOnlyList

    /// Binds a VM property to a 'Model DynamicData.IObservableCache<'Value, 'Key> property.
    member this.BindSourceCache(
            sourceCache: IObservableCache<'Value, 'Key>, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable readOnlyList: ReadOnlyObservableCollection<'Value> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to a SourceCache and stores it in a dictionary.
            let disposable = 
                sourceCache
                    .Connect()
                    |> _.Bind(&readOnlyList)
                    |> _.Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        readOnlyList

    /// Binds a VM property to a 'Model DynamicData.IObservableCache<'Value, 'Key> property.
    member this.BindSourceCache<'Value, 'Key>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            sortBy: 'Value -> 'IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable readOnlyList: ReadOnlyObservableCollection<'Value> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to a SourceCache and stores it in a dictionary.
            let disposable = 
                sourceCache
                    .Connect()
                    |> fun x -> x.SortBy(sortBy)
                    |> _.Bind(&readOnlyList)
                    |> _.Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        readOnlyList


    /// Binds a VM property to a 'Model DynamicData.IObservableCache<'Value, 'Key> property.
    member this.BindSourceCache<'Value, 'Key, 'Transformed>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            map: 'Value -> 'Transformed,
            ?sortBy: 'Transformed -> 'IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = 
        let vmPropertyName = vmPropertyName.Value
        let mutable readOnlyList: ReadOnlyObservableCollection<'Transformed> = Unchecked.defaultof<_>
        if not (propertySubscriptions.ContainsKey vmPropertyName) then
            // Creates a subscription to a SourceCache and stores it in a dictionary.
            let disposable = 
                sourceCache
                    .Connect()
                    .Transform(map)
                    |> fun x -> 
                        match sortBy with
                        | Some sortBy ->
                            x.SortBy(sortBy)
                        | None -> 
                            x.Sort(Comparer.Create(fun _ _ -> 0))
                    |> _.Bind(&readOnlyList)
                    |> _.Subscribe()
            propertySubscriptions.Add(vmPropertyName, disposable)
        readOnlyList

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
