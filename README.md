# Basic dependency injection framework for .NET Core

This is a prototype for a DI Framework (+ Service Locator kinda).

Big Inspiration from ASP.NET Core's DI with Services

Please don't use this in production, many flaws

## Features
- Register services and retrieve Instances
- Dependencies of services are resolved (if dependency is registered aswell)
- Registering with Initializer Delegates
- Working with Generic Services

## TODO
- [ ] Singletons
- [ ] Caching (one "real" instantiation, then copies)
- [ ] Detect Circular references (lol)
