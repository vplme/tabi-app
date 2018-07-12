Tabi App
========

The Tabi App is written in [C#](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/) using the [Xamarin Framework](https://developer.xamarin.com/guides/cross-platform/getting_started/) and [Xamarin Forms](https://developer.xamarin.com/guides/xamarin-forms/). The written code compiles to an Intermediate Language for Android, which is then Just-in-Time (JIT) compiled to native assembly on the phone. For iOS the very same code is Ahead-of-Time (AOT) compiled into native ARM assembly code. This results in near-native performance of the app, while keeping access to all platform-specific APIs.

As an Integrated Development Environment (IDE) we recommend Visual Studio for Windows or Mac, available for free in the [Xamarin Store](https://store.xamarin.com/). Other versions of Visual Studio -- i.e. Professional or Enterprise -- can be used as well.


<!-- blank line -->
<figure class="video_container">
  <iframe src="https://www.youtube.com/embed/5j1-abSuQxI" frameborder="0" allowfullscreen="true"> </iframe>
</figure>
<!-- blank line -->

## Getting started

### Setup config files
In `src/Tabi.Shared/Config` duplicate and rename the following files:
-  `config.debug.sample.xml` to `config.debug.xml`
-  `config.release.sample.xml` to `config.release.xml`

For setting up the backend api visit the repository: [`tabi-backend`](https://gitlab.com/tabi/tabi-backend)
