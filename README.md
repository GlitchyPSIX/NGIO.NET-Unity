# Newgrounds.io .NET:Unity

Unity Coroutine implementation of [NGIO.NET](https://github.com/GlitchyPSIX/NGIO.NET) to empower Unity WebGL and standalone games with the [Newgrounds.io API](https://newgrounds.io).

# Installation

Open the Unity Package Manager. (Window > Package Manager)

You have two options:

1. Install through the **git** url.
   - Copy the HTTP clone URL from the Code dropdown, then use it in the Package Manager by going to [ + ] → *Add package from git URL...*
  
2. Install locally
   - Download the repository as a ZIP (or clone locally using git, skipping the unzipping) from the Code drop down, extract it somewhere, and in Package Manager, go to [ + ] → *Add package from disk...* and locate the ``package.json``.
     - If you clone locally using git, make sure to [update the submodules too](https://stackoverflow.com/a/49427199/6913022).

# Usage

The wiki is a work in progress!

The way of working with the NGIO.NET Communicator stays the same, except you must have a GameObject with the `NGIO.NET Tank Engine` script attached to have it readily available. Once it's all set up in runtime, you can access the NGIO.NET Communicator from anywhere in your code by accessing the singleton ``NGIONet.Engine.Comms`` . From there onwards, it works just like you'd work with NGIO.NET.

# License

For the code, MIT. For the assets, it depends. It's a good idea to ask.

# Other resources

[Newgrounds.IO Discord](https://discord.gg/wcsCk2ErhH)