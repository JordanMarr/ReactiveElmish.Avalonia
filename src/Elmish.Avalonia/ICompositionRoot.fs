namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System
open Microsoft.Extensions.DependencyInjection
open ReactiveUI

type ICompositionRoot = 
    abstract ServiceProvider: IServiceProvider with get
    abstract GetView: Type -> Control

module internal ICompositionRoot = 
    /// Provides an internal reference to the composition root instance.
    let mutable instance : ICompositionRoot = Unchecked.defaultof<_>