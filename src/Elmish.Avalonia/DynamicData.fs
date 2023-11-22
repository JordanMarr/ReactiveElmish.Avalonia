namespace Elmish.Avalonia

open System
open DynamicData

/// Functional helpers for DynamicData.SourceList.
module SourceList = 
    let create<'T>() = 
        new SourceList<'T>()

    let createFrom<'T> (items: seq<'T>) = 
        let sourceList = create<'T>()
        sourceList.AddRange items
        sourceList

    let add<'T> (item: 'T) (sourceList: SourceList<'T>) = 
        sourceList.Add item
        sourceList

    let addRange<'T> (items: seq<'T>) (sourceList: SourceList<'T>) =
        sourceList.AddRange items
        sourceList

    let remove<'T> (item: 'T) (sourceList: SourceList<'T>) =
        sourceList.Remove item |> ignore
        sourceList

/// Functional helpers for DynamicData.SourceCache.
module SourceCache = 
    let create<'TObject, 'TKey> (keySelector: 'TObject -> 'TKey) = 
        new SourceCache<'TObject, 'TKey>(Func<_,_>(keySelector))

    let addOrUpdate<'TObject, 'TKey> (item: 'TObject) (sourceCache: SourceCache<'TObject, 'TKey>) =
        sourceCache.AddOrUpdate item |> ignore
        sourceCache

    let addOrUpdateRange<'TObject, 'TKey> (items: seq<'TObject>) (sourceCache: SourceCache<'TObject, 'TKey>) =
        sourceCache.AddOrUpdate items |> ignore
        sourceCache

    let remove<'TObject, 'TKey> (item: 'TObject) (sourceCache: SourceCache<'TObject, 'TKey>) =
        sourceCache.Remove item |> ignore
        sourceCache

    let removeKey<'TObject, 'TKey> (key: 'TKey) (sourceCache: SourceCache<'TObject, 'TKey>) =
        sourceCache.RemoveKey key |> ignore
        sourceCache

    let removeKeys<'TObject, 'TKey> (keys: seq<'TKey>) (sourceCache: SourceCache<'TObject, 'TKey>) =
        sourceCache.RemoveKeys keys |> ignore
        sourceCache
