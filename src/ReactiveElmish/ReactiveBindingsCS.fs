namespace ReactiveElmish

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open DynamicData

/// Allows using an IStore<'Model> + ReactiveElmishViewModel binding methods from an existing view model using C# Action and Func delegates.
type ReactiveBindingsCS(onPropertyChanged: Action<string>) = 
    let vm = new ReactiveElmishViewModel(onPropertyChanged.Invoke)
    let opt = Option.ofObj

    member this.Bind<'Model, 'ModelProjection>(
            store: IStore<'Model>, 
            modelProjection: Func<'Model, 'ModelProjection>, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.Bind(store, modelProjection.Invoke, vmPropertyName)

    member this.BindOnChanged<'Model, 'OnChanged, 'ModelProjection>(
            store: IStore<'Model>, 
            onChanged: Func<'Model, 'OnChanged>,
            modelProjection: Func<'Model, 'ModelProjection>, 
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) =  vm.BindOnChanged(store, onChanged.Invoke, modelProjection.Invoke, vmPropertyName)

    member this.BindList<'Model, 'ModelProjection>(
            store: IStore<'Model>, 
            modelProjectionSeq: Func<'Model, 'ModelProjection seq>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindList(store, modelProjectionSeq.Invoke, vmPropertyName)

    member this.BindList<'Model, 'ModelProjection, 'Mapped>(
            store: IStore<'Model>, 
            modelProjectionSeq: Func<'Model, 'ModelProjection seq>,
            map: Func<'ModelProjection, 'Mapped>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindList(store, modelProjectionSeq.Invoke, map.Invoke, vmPropertyName)

    member this.BindKeyedList<'Model, 'Key, 'Value, 'Mapped when 'Value : equality and 'Mapped : not struct and 'Key : comparison>(
            store: IStore<'Model>, 
            modelProjection: Func<'Model, Map<'Key, 'Value>>,
            map: Func<'Value, 'Mapped>,
            getKey: Func<'Mapped, 'Key>,
            [<Optional>] update: Action<'Value, 'Mapped>,
            [<Optional>] sortBy: Func<'Mapped, IComparable>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindKeyedList(store, modelProjection.Invoke, map.Invoke, getKey.Invoke, ?update = opt update, ?sortBy = opt sortBy, vmPropertyName = vmPropertyName)

    member this.BindKeyedList<'Model, 'Key, 'Value when 'Value: equality and 'Value : not struct and 'Key : comparison>(
            store: IStore<'Model>, 
            modelProjection: Func<'Model, Map<'Key, 'Value>>,
            getKey: Func<'Value, 'Key>,
            [<Optional>] update: Action<'Value, 'Value>,
            [<Optional>] sortBy: Func<'Value, IComparable>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName: string
        ) = vm.BindKeyedList(store, modelProjection.Invoke, getKey.Invoke, ?update = opt update, ?sortBy = opt sortBy, vmPropertyName = vmPropertyName)

    member this.BindSourceList<'T>(
            sourceList: ISourceList<'T>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindSourceList(sourceList, vmPropertyName)

    member this.BindSourceList<'T, 'Mapped>(
            sourceList: ISourceList<'T>, 
            map: Func<'T, 'Mapped>,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindSourceList(sourceList, map.Invoke, vmPropertyName)

    member this.BindSourceCache<'Value, 'Key>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            [<Optional>] sortBy,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindSourceCache(sourceCache, ?sortBy = opt sortBy, vmPropertyName = vmPropertyName)

    member this.BindSourceCache<'Value, 'Key, 'Mapped when 'Value : not struct and 'Mapped : not struct>(
            sourceCache: IObservableCache<'Value, 'Key>, 
            map: Func<'Value, 'Mapped>,
            [<Optional>] update: Action<'Value, 'Mapped>,
            [<Optional>] sortBy,
            [<CallerMemberName; Optional; DefaultParameterValue("")>] vmPropertyName
        ) = vm.BindSourceCache(sourceCache, map.Invoke, ?update = opt update, ?sortBy = opt sortBy, vmPropertyName = vmPropertyName)

    member this.Subscribe(observable, handler) = 
        vm.Subscribe(observable, handler)
    
    interface IDisposable with
        member this.Dispose() = (vm :> IDisposable).Dispose()
