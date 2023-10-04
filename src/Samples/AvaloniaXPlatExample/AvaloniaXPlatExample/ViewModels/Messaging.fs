module AvaloniaXPlatExample.ViewModels.Messaging

open System.Reactive

type GlobalMsg =
    | GoCounter
    | GoListBox
    | GoHome

/// Provides pub/sub messaging between view models.
let bus = new Subjects.Subject<GlobalMsg>()
