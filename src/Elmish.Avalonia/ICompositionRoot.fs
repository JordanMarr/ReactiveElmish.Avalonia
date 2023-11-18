namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System
open Microsoft.Extensions.DependencyInjection
open ReactiveUI

type ICompositionRoot = 
    abstract ServiceProvider: IServiceProvider with get
    abstract GetView: Type -> Control
    abstract GetView: IReactiveObject -> Control

