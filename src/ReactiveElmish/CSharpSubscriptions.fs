namespace ReactiveElmish

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open DynamicData

/// Allows using an IStore<'Model> + ReactiveElmishViewModel binding methods from an existing view model using C# Action and Func delegates.
type ReactiveBindingsCS(onPropertyChanged: Func<string, unit>) = 
    let vm = new ReactiveElmishViewModel(onPropertyChanged.Invoke)

    member this.Bind<'Model, 'ModelProjection>(
            store: IStore<'Model>, 
            modelProjection: Func<'Model, 'ModelProjection>, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.Bind(store, modelProjection.Invoke, ?vmPropertyName = vmPropertyName)

    member this.BindOnChanged<'Model, 'OnChanged, 'ModelProjection>(
            store: IStore<'Model>, 
            onChanged: 'Model -> 'OnChanged,
            modelProjection: Func<'Model, 'ModelProjection>, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) =  vm.BindOnChanged(store, onChanged, modelProjection.Invoke, ?vmPropertyName = vmPropertyName)

    member this.BindList<'Model, 'ModelProjection>(
            store: IStore<'Model>, 
            modelProjectionSeq: Func<'Model, 'ModelProjection seq>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindList(store, modelProjectionSeq.Invoke, ?vmPropertyName = vmPropertyName)

    member this.BindList'<'Model, 'ModelProjection, 'Mapped>(
            store: IStore<'Model>, 
            modelProjectionSeq: Func<'Model, 'ModelProjection seq>,
            map: 'ModelProjection -> 'Mapped,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindList'(store, modelProjectionSeq.Invoke, map, ?vmPropertyName = vmPropertyName)

    member this.BindKeyedList<'Model, 'Key, 'Value, 'Mapped when 'Value : equality and 'Mapped : not struct and 'Key : comparison>(
            store: IStore<'Model>, 
            modelProjection: Func<'Model, Map<'Key, 'Value>>,
            map: 'Value -> 'Mapped,
            getKey: 'Mapped -> 'Key,
            ?update: 'Value -> 'Mapped -> unit,
            ?sortBy: 'Mapped -> IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindKeyedList(store, modelProjection.Invoke, map, getKey, ?update = update, ?sortBy = sortBy, ?vmPropertyName = vmPropertyName)

    member this.BindKeyedList<'Model, 'Key, 'Value when 'Value: equality and 'Value : not struct and 'Key : comparison>(
            store: IStore<'Model>, 
            modelProjection: 'Model -> Map<'Key, 'Value>,
            getKey: 'Value -> 'Key,
            ?update: 'Value -> 'Value -> unit,
            ?sortBy: 'Value -> IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName: string
        ) = vm.BindKeyedList(store, modelProjection, getKey, ?update = update, ?sortBy = sortBy, ?vmPropertyName = vmPropertyName)

    member this.BindSourceList<'T>(
            sourceList: ISourceList<'T>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindSourceList(sourceList, ?vmPropertyName = vmPropertyName)

    member this.BindSourceList<'T, 'Mapped>(
            sourceList: ISourceList<'T>, 
            map: 'T -> 'Mapped,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindSourceList(sourceList, map, ?vmPropertyName = vmPropertyName)

    member this.BindSourceCache<'Value, 'Key>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            ?sortBy: 'Value -> 'IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindSourceCache(sourceCache, ?sortBy = sortBy, ?vmPropertyName = vmPropertyName)

    member this.BindSourceCache<'Value, 'Key, 'Mapped when 'Value : not struct and 'Mapped : not struct>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            map: 'Value -> 'Mapped,
            ?update: 'Value -> 'Mapped -> unit,
            ?sortBy: 'Mapped -> 'IComparable,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] ?vmPropertyName
        ) = vm.BindSourceCache(sourceCache, map, ?update = update, ?sortBy = sortBy, ?vmPropertyName = vmPropertyName)

    member this.Subscribe(observable, handler) = 
        vm.Subscribe(observable, handler)
    
    interface IDisposable with
        member this.Dispose() = (vm :> IDisposable).Dispose()
