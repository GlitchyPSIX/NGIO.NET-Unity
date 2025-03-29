# Newgrounds.io .NET:Unity

Unity Coroutine implementation of [NGIO.NET](https://github.com/GlitchyPSIX/NGIO.NET) to empower Unity WebGL and standalone games with the [Newgrounds.io API](https://newgrounds.io).

# Installation

Open the Unity Package Manager. (Window > Package Manager)

You have two options:

1. Install through the **git** url.
   - Copy the HTTP clone URL from the Code dropdown, then use it in the Package Manager by going to [ + ] → *Add package from git URL...*

   You can update at anytime by clicking the Update button in the Package Manager.
  
2. Install locally
   - Download the repository as a ZIP using the URL (or clone locally using git, skipping the unzipping) from the Code dropdown, extract it somewhere easy to access, and in Package Manager, go to [ + ] → *Add package from disk...* and locate the ``package.json``. Installing locally this way allows you to use the version of the package you downloaded across several projects.

# Usage

The wiki is a work in progress!

The way of working with the NGIO.NET Communicator stays the same, except you must have a GameObject with the `NGIO.NET Tank Engine` script attached to have it readily available. Once it's all set up in runtime, you can access the NGIO.NET Communicator from anywhere in your code by accessing the singleton ``NGIONet.Engine.Comms`` . From there onwards, it works just like you'd work with NGIO.NET.

You can eventually read the Quickstart in the wiki.

**NOTE: The scripts currently don't remember sessions nor do they take the session information given by Newgrounds when hosted over there. This will be added soon.**

# License

For the code, MIT. For the audio and graphics assets, it depends. It's a good idea to ask.

# Other resources

[Newgrounds.IO Discord](https://discord.gg/wcsCk2ErhH)