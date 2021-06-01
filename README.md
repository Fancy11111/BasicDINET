# Basic dependency injection framework for .NET Core

This is a prototype for a DI Framework (+ Service Locator kinda).

Big Inspiration from ASP.NET Core's DI with Services

Please don't use this POS (Piece of Software) in production, many flaws

## Features
- Register services and retrieve Instances
- Dependencies of services are resolved (if dependency is registered aswell)
- Registering with Initializer Delegates
- Working with Generic Services
- Singletons
- Caches (kind of)

## TODO
- [x] Singletons
- [x] Generics
- [ ] Caching (one "real" instantiation, then deep copies)
- [x] reflection caching
- [ ] Detect Circular references (lol)
