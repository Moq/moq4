moq
===

[![Version](https://img.shields.io/nuget/vpre/Moq.svg)](https://www.nuget.org/packages/Moq)
[![Downloads](https://img.shields.io/nuget/dt/Moq.svg)](https://www.nuget.org/packages/Moq)
[![Discord Chat](https://img.shields.io/badge/chat-on%20discord-7289DA.svg)](https://discord.gg/8PtpGdu)
[![Sponsors](https://img.shields.io/github/sponsors/devlooped?style=social)](https://github.com/sponsors/devlooped)

<!-- #content -->
The most popular and friendly mocking library for .NET

```csharp
  var mock = new Mock<ILoveThisLibrary>();

  // WOW! No record/replay weirdness?! :)
  mock.Setup(library => library.DownloadExists("2.0.0.0"))
      .Returns(true);

  // Use the Object property on the mock to get a reference to the object
  // implementing ILoveThisLibrary, and then exercise it by calling
  // methods on it
  ILoveThisLibrary lovable = mock.Object;
  bool download = lovable.DownloadExists("2.0.0.0");

  // Verify that the given method was indeed called with the expected value at most once
  mock.Verify(library => library.DownloadExists("2.0.0.0"), Times.AtMostOnce());
```

Moq also is the first and only library so far to provide Linq to Mocks, so that the 
same behavior above can be achieved much more succinctly:

```csharp
  ILoveThisLibrary lovable = Mock.Of<ILoveThisLibrary>(l =>
    l.DownloadExists("2.0.0.0") == true);

  // Exercise the instance returned by Mock.Of by calling methods on it...
  bool download = lovable.DownloadExists("2.0.0.0");

  // Simply assert the returned state:
  Assert.True(download);
  
  // If you want to go beyond state testing and want to 
  // verify the mock interaction instead...
  Mock.Get(lovable).Verify(library => library.DownloadExists("2.0.0.0"));
```

You can think of Linq to Mocks as "from the universe of mocks, give me one whose behavior 
matches this expression".

Check out the [Quickstart](https://github.com/moq/moq/wiki/Quickstart) for more examples!

<!-- #content -->

## What?

Moq (pronounced "Mock-you" or just "Mock") is the only mocking library for .NET developed from scratch to take full advantage of .NET Linq expression trees and lambda expressions, which makes it the most productive, type-safe and refactoring-friendly mocking library available. And it supports mocking interfaces as well as classes. Its API is extremely simple and straightforward, and doesn't require any prior knowledge or experience with mocking concepts.

## Why?

The library was created mainly for developers who aren't currently using any mocking library (or are displeased with the complexities of some other implementation), and who are typically [manually writing their own mocks](https://web.archive.org/web/20200920165817/http://blogs.clariusconsulting.net/kzu/mocks-stubs-and-fakes-its-a-continuum/) (with more or less "fanciness"). Most developers in this situation also happen to be [quite pragmatic and adhere to state](https://web.archive.org/web/20200414170510/http://blogs.clariusconsulting.net/kzu/state-testing-vs-interaction-testing/) (or classic) TDD. It's the result of feeling that the barrier of entry from other mocking libraries is a bit high, and a simpler, more lightweight and elegant approach is possible. Moq achieves all this by taking full advantage of the elegant and compact C# and VB language features collectively known as LINQ (they are not just for queries, as the acronym implies).

Moq is designed to be a very practical, unobtrusive and straight-forward way to quickly setup dependencies for your tests. Its API design helps even novice users to fall in the "pit of success" and avoid most common misuses/abuses of mocking. 

When it was conceived, it was the only mocking library that went against the generalized and somewhat unintuitive (especially for novices) Record/Replay approach from all other libraries (and [that might have been a good thing](https://web.archive.org/web/20200920165939/http://blogs.clariusconsulting.net/kzu/whats-wrong-with-the-recordreplyverify-model-for-mocking-frameworks/) ;)).

Not using Record/Replay also means that it's straightforward to move common expectations to a fixture setup method and even override those expectations when needed in a specific unit test.

You can read more about the "why" and see some nice screenshots at [kzu's blog](https://web.archive.org/web/20200920164302/http://blogs.clariusconsulting.net/kzu/why-do-we-need-yet-another-net-mocking-framework/).

## Where?

See our [Quickstart](https://github.com/moq/moq/wiki/Quickstart) examples to get a feeling of the extremely simple API and install from [NuGet](http://nuget.org/packages/moq).

Read about the announcement at [kzu's blog](https://web.archive.org/web/20201130233544/http://blogs.clariusconsulting.net/kzu/linq-to-mock-moq-is-born/). Get some background on [the state of mock libraries from Scott Hanselman](http://www.hanselman.com/blog/MoqLinqLambdasAndPredicatesAppliedToMockObjects.aspx).

In-depth documentation is being added to the [documentation website](https://github.com/moq/moq4/wiki).


## Who?

Moq was originally developed by [Clarius](http://www.clariusconsulting.net), [Manas](http://www.manas.com.ar) and [InSTEDD](http://www.instedd.org).

Moq uses [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/) internally as the interception mechanism to enable mocking.

<!-- #features -->

## Features at a glance
Moq offers the following features:
* Strong-typed: no strings for expectations, no object-typed return values or constraints
* Unsurpassed VS IntelliSense integration: everything supports full VS IntelliSense, from setting expectations, to specifying method call arguments, return values, etc.
* No Record/Replay idioms to learn. Just construct your mock, set it up, use it and optionally verify calls to it (you may not verify mocks when they act as stubs only, or when you are doing more classic state-based testing by checking returned values from the object under test)
* VERY low learning curve as a consequence of the previous three points. For the most part, you don't even need to ever read the documentation.
* Granular control over mock behavior with a simple [MockBehavior](https://www.fuget.org/packages/Moq/4.16.1/lib/netstandard2.1/Moq.dll/Moq/MockBehavior) enumeration (no need to learn what's the theoretical difference between a mock, a stub, a fake, a dynamic mock, etc.)
* Mock both interfaces and classes
* Override expectations: can set default expectations in a fixture setup, and override as needed on tests
* Pass constructor arguments for mocked classes
* Intercept and raise events on mocks
* Intuitive support for ```out/ref``` arguments

We appreciate deeply any feedback that you may have! Feel free to participate in the [chat], or report an issue in the [issue tracker].

 [chat]:
 https://discord.gg/8PtpGdu
 "Moq channel on Discord"

 [issue tracker]:
 https://github.com/moq/moq/issues
 "Moq issue tracker on GitHub"

<!-- #features -->
<!-- #sponsors -->
<!-- include https://raw.githubusercontent.com/devlooped/sponsors/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/clarius.png "Clarius Org")](https://github.com/clarius)
[![Kirill Osenkov](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KirillOsenkov.png "Kirill Osenkov")](https://github.com/KirillOsenkov)
[![MFB Technologies, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MFB-Technologies-Inc.png "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![Stephen Shaw](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/decriptor.png "Stephen Shaw")](https://github.com/decriptor)
[![Torutek](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/torutek-gh.png "Torutek")](https://github.com/torutek-gh)
[![DRIVE.NET, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/drivenet.png "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Ashley Medway](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/AshleyMedway.png "Ashley Medway")](https://github.com/AshleyMedway)
[![Keith Pickford](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Keflon.png "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tbolon.png "Thomas Bolon")](https://github.com/tbolon)
[![Kori Francis](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/kfrancis.png "Kori Francis")](https://github.com/kfrancis)
[![Toni Wenzel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/twenzel.png "Toni Wenzel")](https://github.com/twenzel)
[![Giorgi Dalakishvili](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Giorgi.png "Giorgi Dalakishvili")](https://github.com/Giorgi)
[![Uno Platform](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/unoplatform.png "Uno Platform")](https://github.com/unoplatform)
[![Dan Siegel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/dansiegel.png "Dan Siegel")](https://github.com/dansiegel)
[![Reuben Swartz](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/rbnswartz.png "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jfoshee.png "Jacob Foshee")](https://github.com/jfoshee)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Mrxx99.png "")](https://github.com/Mrxx99)
[![Eric Johnson](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/eajhnsn1.png "Eric Johnson")](https://github.com/eajhnsn1)
[![Ix Technologies B.V.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/IxTechnologies.png "Ix Technologies B.V.")](https://github.com/IxTechnologies)
[![David JENNI](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/davidjenni.png "David JENNI")](https://github.com/davidjenni)
[![Jonathan ](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Jonathan-Hickey.png "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Oleg Kyrylchuk](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/okyrylchuk.png "Oleg Kyrylchuk")](https://github.com/okyrylchuk)
[![Charley Wu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/akunzai.png "Charley Wu")](https://github.com/akunzai)
[![Jakob Tikjøb Andersen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jakobt.png "Jakob Tikjøb Andersen")](https://github.com/jakobt)
[![Seann Alexander](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/seanalexander.png "Seann Alexander")](https://github.com/seanalexander)
[![Tino Hager](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tinohager.png "Tino Hager")](https://github.com/tinohager)
[![Mark Seemann](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/ploeh.png "Mark Seemann")](https://github.com/ploeh)
[![Ken Bonny](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KenBonny.png "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/SimonCropp.png "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/agileworks-eu.png "agileworks-eu")](https://github.com/agileworks-eu)
[![sorahex](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/sorahex.png "sorahex")](https://github.com/sorahex)
[![Zheyu Shen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/arsdragonfly.png "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/vezel-dev.png "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/ChilliCream.png "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/4OTC.png "4OTC")](https://github.com/4OTC)


<!-- sponsors.md -->

[![Sponsor this project](https://raw.githubusercontent.com/devlooped/sponsors/main/sponsor.png "Sponsor this project")](https://github.com/sponsors/devlooped)
&nbsp;

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://raw.githubusercontent.com/devlooped/sponsors/main/footer.md -->
