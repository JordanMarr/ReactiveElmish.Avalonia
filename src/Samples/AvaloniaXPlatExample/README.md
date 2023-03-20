# Elmish.Avalonia Counter App cross platform

This is adapted from the wonderful [Avalonia cross platform solution](https://docs.avaloniaui.net/tutorials/developing-for-mobile/create-a-cross-platform-solution) to use Elmish.Avalonia.  Thanks to @JordanMarr / @AngelMunoz for lots of help getting it running 

The core application is in `AvaloniaXPlatExample` with the platform specific folders just being lightweight wrappers.  Desktop and Web have been tested.  The iOS/Android versions have not been deployed but should work in theory.

- Darren

## Requirements 

For various platforms e.g. Android,  Web, additional tools are required.

The projects are configured for `net7`

## Workloads
The following workload must be installed (once) prior to compilation

- `dotnet workload install ios android wasm-experimental wasm-tools`

## Addition web information 
- See the [avalonia web docs](https://docs.avaloniaui.net/tutorials/running-in-the-browser) for more information.

## Building

`dotnet build` should compile each platform.  They can be built separately in the individual platform folders (.e.g `AvaloniaXPlatExample.Desktop` ).  

## Desktop

`dotnet run --project `AvaloniaXPlatExample.Desktop`

## Web

`dotnet run --project `AvaloniaXPlatExample.Web`

Follow the instructions to open the browser once program starts.
